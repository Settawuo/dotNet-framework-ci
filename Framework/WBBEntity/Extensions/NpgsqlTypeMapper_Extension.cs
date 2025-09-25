using Npgsql;
using NpgsqlTypes;
using System;
using System.Linq;
using System.Reflection;

namespace WBBEntity.Extensions
{
    public static class NpgsqlTypeMapper_Extension
    {
        public static void MapCompositeFactory(this NpgsqlConnection npgsql, string MapType)
        {
            var dotnetType = Assembly.GetExecutingAssembly()
                                        .GetTypes()
                                        .FirstOrDefault(x => x.Name == MapType);

            //ArgumentNullException.ThrowIfNull(dotnetType, $"{MapType} unable to find composite type class.");

            var compositeTypeName = dotnetType.GetAttributeValue((PostgresqlCustomTypeMappingAttribute attr) => attr.Text);

            //ArgumentNullException.ThrowIfNull(compositeTypeName, $"{typeof(PostgresqlCustomTypeMappingAttribute)} is null or not set.");

            //npgsql.TypeMapper.MapComposite(dotnetType, compositeTypeName);

            ////Config MapComposite
            switch (compositeTypeName)
            {
                case "fbb_list_order_record":
                    npgsql.MapComposite<HvrListOrderRecord>(compositeTypeName);
                    break;
                case "fbb_list_card_no_record":
                    npgsql.MapComposite<HvrListCardNoRecord>(compositeTypeName);
                    break;
                case "fbb_list_non_mobile_no_record":
                    npgsql.MapComposite<HvrListNonMobileNoRecord>(compositeTypeName);
                    break;
                case "fbb_list_contact_mobile_record":
                    npgsql.MapComposite<HvrListContactMobileNoRecord>(compositeTypeName);
                    break;
                case "fbbadm.patch_sn_rec":
                    npgsql.MapComposite<patch_sn_rec>("fbbadm.patch_sn_rec");
                    break;
                case "fbbadm.search_order_rec":
                    npgsql.MapComposite<search_order_rec>("fbbadm.search_order_rec");
                    break;
            }

        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector)
         where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }
    }

    #region Config TypeMapping

    [PostgresqlCustomTypeMapping("fbb_list_order_record")]
    public class HvrListOrderRecord
    {
        [PgName("orderno")]
        public string orderno { get; set; }
    }

    [PostgresqlCustomTypeMapping("fbb_list_card_no_record")]
    public class HvrListCardNoRecord
    {
        [PgName("cardno")]
        public string cardno { get; set; }
    }

    [PostgresqlCustomTypeMapping("fbb_list_non_mobile_no_record")]
    public class HvrListNonMobileNoRecord
    {
        [PgName("nonmobileno")]
        public string nonmobileno { get; set; }
    }

    [PostgresqlCustomTypeMapping("fbb_list_contact_mobile_record")]
    public class HvrListContactMobileNoRecord
    {
        [PgName("contactmobileno")]
        public string contactmobileno { get; set; }
    }
    [PostgresqlCustomTypeMappingAttribute("fbbadm.patch_sn_rec")]
    public partial class patch_sn_rec
    {
        [PgName("internet_no")]
        public string internet_no { get;  set; }

        [PgName("sn")]
        public string sn { get; set; }

        [PgName("status")]
        public string status { get; set; }

        [PgName("foa_code")]
        public string foa_code { get; set; }

        [PgName("created_date")]
        public string created_date { get; set; }

        [PgName("posting_date")]
        public string posting_date { get; set; }

        [PgName("movement_type")]
        public string movement_type { get; set; }

        [PgName("location_code")]
        public string location_code { get; set; }
    }

    [PostgresqlCustomTypeMappingAttribute("fbbadm.search_order_rec")]
    public partial class search_order_rec
    {
        [PgName("sn")]
        public string sn { get; set; }

    }

    #endregion Config TypeMapping
}
