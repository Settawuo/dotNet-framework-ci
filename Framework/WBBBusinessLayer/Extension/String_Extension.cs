using Newtonsoft.Json;

namespace WBBBusinessLayer.Extension
{
    public static class String_Extension
    {
        public static T Json2Obj<T>(this string jsonText)
        {
            var obj = JsonConvert.DeserializeObject<T>(jsonText);
            return obj;
        }

        public static string Obj2Json<T>(this T v, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.None)
        {
            return JsonConvert.SerializeObject(v, formatting);
        }
    }
}