namespace SwagDoc.Generating.MsWord.Configuration
{
    public interface IMsWordGeneratorConfiguration
    {
        string Template { get; }

        bool ShowApplication { get; }

        bool GroupResources { get; }

        IMsWordCaptionsConfiguration Captions { get; }

        IMsWordStylesConfiguration Styles { get; }
    }
}
