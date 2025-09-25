using System.Collections.Generic;
using System.Data;

namespace FBBConfig
{
    public static class CommonConfig
    {

        public static DataTable ConvertToDatatable<T>(this IList<T> data)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
                System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                System.ComponentModel.PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }


}
