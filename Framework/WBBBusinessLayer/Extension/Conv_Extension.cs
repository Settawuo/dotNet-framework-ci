using Newtonsoft.Json;

namespace WBBBusinessLayer.Extension
{
    public static class Conv_Extension
    {
        public static string ObjToJson<T>(this T source, bool isIndented = false, bool isIgnoreNull = false)
        {
            var indented = isIndented ? Formatting.Indented : Formatting.None;
            if (isIgnoreNull)
            {
                return JsonConvert.SerializeObject(source, new JsonSerializerSettings()
                {
                    Formatting = indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            return JsonConvert.SerializeObject(source, indented);
        }

        public static T JsonToObj<T>(this string jsonText)
        {
            var obj = JsonConvert.DeserializeObject<T>(jsonText);
            return obj;
        }
    }
}
