namespace SwagDoc.Generating.MsWord.Configuration
{
    public interface IMsWordStylesConfiguration
    {
        string ResourceGroupTitle { get; }

        string ResourceTitle { get; }

        string ResourceRequest { get; }

        string ResponseTitle { get; }

        string Description { get; }

        string Caption { get; }

        string Content { get; }
    }
}
