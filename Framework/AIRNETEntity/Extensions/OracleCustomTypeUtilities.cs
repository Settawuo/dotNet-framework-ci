using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace AIRNETEntity.Extensions
{
    public class OracleCustomTypeUtilities
    {
        public static OracleParameter CreateUDTArrayParameter<T>(string param_name, string udt_type_name, T value)
           where T : IOracleCustomType, INullable
        {
            OracleParameter param_customer = new OracleParameter();
            param_customer.ParameterName = param_name;
            param_customer.OracleDbType = OracleDbType.Array;
            param_customer.Direction = ParameterDirection.Input;
            param_customer.UdtTypeName = udt_type_name;
            param_customer.Value = value;
            return param_customer;
        }
    }
}
