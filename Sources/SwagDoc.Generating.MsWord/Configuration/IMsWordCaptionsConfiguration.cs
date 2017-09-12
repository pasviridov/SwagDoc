namespace SwagDoc.Generating.MsWord.Configuration
{
    public interface IMsWordCaptionsConfiguration
    {
        string CommonGroup { get; }

        string PathParameters { get; }

        string QueryParameters { get; }

        string RequestBody { get; }

        string ResponseBody { get; }
    }
}
