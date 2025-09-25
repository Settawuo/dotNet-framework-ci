using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace WBBData.DbIteration
{
    public static class EnitiyRepExtension
    {
        public static int ExecuteSqlCommandSmart(this Database self, string storedProcedure, object parameters = null)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (string.IsNullOrEmpty(storedProcedure))
                throw new ArgumentException("storedProcedure");

            if (null != parameters)
            {
                var arguments = PrepareOraArguments(storedProcedure, parameters);
                return self.ExecuteSqlCommand(arguments.Item1, arguments.Item2);
            }
            else
                return self.ExecuteSqlCommand(storedProcedure);
        }

        public static int ExecuteSqlCommandSmart(this Database self, string storedProcedure, out object[] paramOut, object parameters = null)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (string.IsNullOrEmpty(storedProcedure))
                throw new ArgumentException("storedProcedure");

            var arguments = PrepareOraArguments(storedProcedure, parameters);
            var result = self.ExecuteSqlCommand(arguments.Item1, arguments.Item2);
            paramOut = arguments.Item2;
            return result;
        }

        public static IQueryable<T> SqlQuerySmart<T>(this Database self, string storedProcedure, object parameters = null)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (string.IsNullOrEmpty(storedProcedure))
                throw new ArgumentException("storedProcedure");

            if (null != parameters)
            {
                var arguments = PrepareOraArguments(storedProcedure, parameters);
                return self.SqlQuery<T>(arguments.Item1, arguments.Item2).AsQueryable<T>();
            }
            else
                return self.SqlQuery<T>(storedProcedure).AsQueryable<T>();
        }

        public static IQueryable SqlQuerySmart(this Database self, Type elementType, string storedProcedure, object parameters = null)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            if (string.IsNullOrEmpty(storedProcedure))
                throw new ArgumentException("storedProcedure");
            if (null != parameters)
            {
                var arguments = PrepareOraArguments(storedProcedure, parameters);
                return self.SqlQuery(elementType, arguments.Item1, arguments.Item2).AsQueryable();
            }
            else
                return self.SqlQuery(elementType, storedProcedure).AsQueryable();
        }

        private static Tuple<string, object[]> PrepareArguments(string storedProcedure, object parameters)
        {
            var parameterNames = new List<string>();
            var parameterParameters = new List<object>();
            var haveOutParam = false;

            if (parameters != null)
            {
                foreach (PropertyInfo propertyInfo in parameters.GetType().GetProperties())
                {
                    string name = "@" + propertyInfo.Name;
                    object value = propertyInfo.GetValue(parameters, null);

                    parameterNames.Add(name);
                    if (propertyInfo.Name.ToLower().Contains("isout"))
                    {
                        haveOutParam = true;
                        var outParam = new OracleParameter();
                        outParam.ParameterName = name;
                        outParam.Direction = ParameterDirection.Output;
                        outParam.OracleDbType = OracleDbType.RefCursor;
                        parameterParameters.Add(outParam);
                    }
                    else
                        parameterParameters.Add(new OracleParameter(name, value ?? DBNull.Value));
                }
            }

            if (parameterNames.Count > 0)
                storedProcedure += " " + string.Join(", ", parameterNames);
            if (haveOutParam)
                storedProcedure += " OUT";

            return new Tuple<string, object[]>(storedProcedure, parameterParameters.ToArray());
        }

        private static Tuple<string, object[]> PrepareOraArguments(string storedProcedure, object parameters)
        {
            var parameterNames = new List<string>();
            var arguments = new List<object>();
            var isStored = true;

            if (parameters != null)
            {
                foreach (PropertyInfo propertyInfo in parameters.GetType().GetProperties())
                {

                    string name = ":" + propertyInfo.Name;
                    object value = propertyInfo.GetValue(parameters, null);

                    parameterNames.Add(name);
                    if ((value != null) && (value.GetType() == typeof(OracleParameter)))
                    {
                        OracleParameter outParam = (OracleParameter)value;
                        outParam.ParameterName = propertyInfo.Name;
                        arguments.Add(outParam);
                    }

                    //if (value.ToString().ToLower().Equals("p_cursor"))
                    //{
                    //    var outParam = new OracleParameter();
                    //    outParam.ParameterName = propertyInfo.Name;
                    //    outParam.Direction = ParameterDirection.Output;
                    //    outParam.OracleDbType = OracleDbType.RefCursor;
                    //    parameterParameters.Add(outParam);
                    //}
                    //if (propertyInfo.Name.ToLower().Equals("result"))
                    //{
                    //    isStored = false;
                    //    var resultParam = new OracleParameter();
                    //    resultParam.Direction = ParameterDirection.ReturnValue;
                    //    resultParam.ParameterName = propertyInfo.Name;
                    //    resultParam.Size = 5000;
                    //    parameterParameters.Add(resultParam);
                    //}
                    else
                        arguments.Add(new OracleParameter(name, value ?? DBNull.Value));

                }
            }

            if (isStored)
            {
                storedProcedure = string.Format("begin {0} (", storedProcedure);

                if (parameterNames.Count > 0)
                    storedProcedure += " " + string.Join(", ", parameterNames);

                storedProcedure += "); end;";
            }
            else
            {
                storedProcedure = string.Format("begin :result := {0} (", storedProcedure);
                parameterNames.Remove(":result");
                if (parameterNames.Count > 0)
                    storedProcedure += " " + string.Join(", ", parameterNames);

                storedProcedure += "); end;";
            }

            return new Tuple<string, object[]>(storedProcedure, arguments.ToArray());
        }

        //private static object ExtractParamValue(object paramValues)
        //{
        //    foreach (PropertyInfo pInfo in paramValues.GetType().GetProperties())
        //    {
        //        object value = pInfo.GetValue(paramValues, null);

        //    }
        //    var value = new
        //    {
        //        paramDirection = "",
        //        paramDbType = "",
        //        paramsize = "",
        //    };

        //    return value;
        //}


        #region R23.06 For Shareplex to HVR PostgreSQL

        public static List<string> GetCursor(this List<NpgsqlParameter> item2)
        {
            return item2.Where(w => w.DbType != DbType.Object && w.NpgsqlDbType == NpgsqlTypes.NpgsqlDbType.Refcursor).Select(s => s.ParameterName).ToList();
        }

        public static void AddParameter(this NpgsqlCommand command, List<NpgsqlParameter> npgsqlParameters)
        {
            foreach (var item in npgsqlParameters)
            {
                command.Parameters.Add(item);
            }
        }

        public static List<object> FetchExecuteReader(this NpgsqlCommand cmd, List<string> refcur)
        {
            var _array = new List<object>();
            try
            {
                foreach (var item in refcur)
                {
                    var dt = new DataTable(item);
                    cmd.CommandText = $"fetch all in \"{item}\";";
                    cmd.CommandType = CommandType.Text;
                    cmd.AllResultTypesAreUnknown = true;

                    var reader = cmd.ExecuteReader();
                    var columns = Enumerable.Range(0, reader.FieldCount)
                        .Select((x) =>
                        {
                            var colName = reader.GetName(x);
                            return new DataColumn(colName);
                        })
                        .ToArray();
                    dt.Columns.AddRange(columns);
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var row = dt.NewRow();
                            foreach (var column in columns)
                            {
                                var fieldValue = reader.GetValue(reader.GetOrdinal(column.ColumnName));
                                row[column] = fieldValue;
                            }
                            dt.Rows.Add(row);
                        }
                    }
                    _array.Add(dt);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Error occured FetchExecuteReader");
            }
            return _array;
        }
        #endregion R23.06 For Shareplex to HVR PostgreSQL

    }
}