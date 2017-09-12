using System.Configuration;

namespace SwagDoc.Generating.MsWord.Configuration
{
    public class MsWordCaptionsConfiguration : ConfigurationElement, IMsWordCaptionsConfiguration
    {
        [ConfigurationProperty("commonGroup")]
        public string CommonGroup => (string)base["commonGroup"];

        [ConfigurationProperty("pathParameters")]
        public string PathParameters => (string)base["pathParameters"];

        [ConfigurationProperty("queryParameters")]
        public string QueryParameters => (string)base["queryParameters"];

        [ConfigurationProperty("requestBody")]
        public string RequestBody => (string)base["requestBody"];

        [ConfigurationProperty("responseBody")]
        public string ResponseBody => (string)base["responseBody"];
    }
}
