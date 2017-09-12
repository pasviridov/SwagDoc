using System;
using SwagDoc.Parsing;

namespace SwagDoc.Generating
{
    public interface IDocumentGenerator
    {
        event EventHandler<string> LogProgress;

        void GenerateDocumentation(SwagApi api, string outputPath);
    }
}
