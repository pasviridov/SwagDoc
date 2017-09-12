using System.Collections.Generic;

using SwagDoc.Generating.MsWord.Configuration;

namespace SwagDoc.Generating.MsWord
{
    public class MsWordGeneratorFactory : IDocumentGeneratorFactory
    {
        private readonly IMsWordGeneratorConfiguration _configuration;

        public MsWordGeneratorFactory() : this(null) { }

        public MsWordGeneratorFactory(IMsWordGeneratorConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name => "Microsoft Word";

        public IReadOnlyList<string> Extensions { get; } = new[] { ".docx" };

        public IDocumentGenerator CreateGenerator() => new MsWordGenerator(_configuration ?? MsWordGeneratorConfigurationSection.Default);
    }
}
