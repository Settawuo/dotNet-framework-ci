using System.IO;

namespace FBBQueryOrderPendingPayment.Extension
{
    public class BatchRestSharpJsonSerializer : RestSharp.Serializers.ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _jsonSerializer;

        public BatchRestSharpJsonSerializer()
        {
            ContentType = "application/json";

            _jsonSerializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Include,
                DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include
            };
        }

        public BatchRestSharpJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            ContentType = "application/json";
            _jsonSerializer = serializer;
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new Newtonsoft.Json.JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';
                    _jsonSerializer.Serialize(jsonTextWriter, obj);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }

        public string DateFormat { get; set; }

        public string RootElement { get; set; }

        public string Namespace { get; set; }

        public string ContentType { get; set; }

    }
}