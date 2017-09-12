using System.Collections.Generic;

namespace SwagDoc.Generating
{
    public interface IDocumentGeneratorFactory
    {
        string Name { get; }

        IReadOnlyList<string> Extensions { get; }

        IDocumentGenerator CreateGenerator();
    }
}
