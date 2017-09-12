using System.Configuration;

namespace SwagDoc.Generating.MsWord.Configuration
{
    public class MsWordStylesConfiguration : ConfigurationElement, IMsWordStylesConfiguration
    {
        [ConfigurationProperty("resourceGroupTitle")]
        public string ResourceGroupTitle => (string)base["resourceGroupTitle"];

        [ConfigurationProperty("resourceTitle")]
        public string ResourceTitle => (string)base["resourceTitle"];

        [ConfigurationProperty("resourceRequest")]
        public string ResourceRequest => (string)base["resourceRequest"];

        [ConfigurationProperty("responseTitle")]
        public string ResponseTitle => (string)base["responseTitle"];

        [ConfigurationProperty("description")]
        public string Description => (string)base["description"];

        [ConfigurationProperty("caption")]
        public string Caption => (string)base["caption"];

        [ConfigurationProperty("content")]
        public string Content => (string)base["content"];
    }
}
