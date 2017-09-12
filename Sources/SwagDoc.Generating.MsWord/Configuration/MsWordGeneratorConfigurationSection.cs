using System;
using System.Configuration;

namespace SwagDoc.Generating.MsWord.Configuration
{
    public class MsWordGeneratorConfigurationSection : ConfigurationSection, IMsWordGeneratorConfiguration
    {
        public const string SectionName = "swagDoc/msword";

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<MsWordGeneratorConfigurationSection> _default = new Lazy<MsWordGeneratorConfigurationSection>(
            () => (MsWordGeneratorConfigurationSection)ConfigurationManager.GetSection(SectionName) ?? new MsWordGeneratorConfigurationSection());

        public static MsWordGeneratorConfigurationSection Default => _default.Value;

        [ConfigurationProperty("template")]
        public string Template => (string)base["template"];

        [ConfigurationProperty("showApplication")]
        public bool ShowApplication => (bool)base["showApplication"];

        [ConfigurationProperty("groupResources")]
        public bool GroupResources => (bool)base["groupResources"];

        [ConfigurationProperty("captions")]
        public MsWordCaptionsConfiguration Captions => (MsWordCaptionsConfiguration)base["captions"];

        [ConfigurationProperty("styles")]
        public MsWordStylesConfiguration Styles => (MsWordStylesConfiguration)base["styles"];

        IMsWordCaptionsConfiguration IMsWordGeneratorConfiguration.Captions => Captions;

        IMsWordStylesConfiguration IMsWordGeneratorConfiguration.Styles => Styles;
    }
}
