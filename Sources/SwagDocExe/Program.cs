using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Common.Logging;

using SwagDoc.Generating;
using SwagDoc.Parsing;
using SwagDoc.Generating.MsWord;

namespace SwagDoc
{
    public class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger<Program>();

        private delegate void OptionParser(Program program, string[] args, ref int index);

        private static readonly Dictionary<string, OptionParser> Options = new Dictionary<string, OptionParser>
        {
            { "o", OutputPathParser }
        };

        private static readonly IDocumentGeneratorFactory[] Generators =
        {
            new MsWordGeneratorFactory()
        };

        public Program(string[] args)
        {
            for (int ii = 0; ii < args.Length;)
            {
                var arg = args[ii++];

                if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    var option = arg.Substring(1);
                    OptionParser optionParser;
                    if (!Options.TryGetValue(option, out optionParser))
                        throw new InvalidOperationException($"Invalid option [{option}]");

                    optionParser(this, args, ref ii);
                }
                else
                {
                    SpecPath = Path.GetFullPath(arg);
                }
            }

            if (string.IsNullOrEmpty(SpecPath))
                throw new InvalidOperationException("No specification path specified");

            if (string.IsNullOrEmpty(OutputPath))
            {
                OutputPath = Path.ChangeExtension(SpecPath, Generators.First().Extensions.First());
            }
        }

        public string SpecPath { get; set; }

        public string OutputPath { get; set; }

        public void Run()
        {

            Logger.Info($"Loading swagger specification from [{SpecPath}].");

            var api = SwagApi.LoadFromFile(SpecPath);

            Logger.Info("Specification successfully loaded and parsed.");

            Logger.Info($"Target path is [{OutputPath}]");

            var ext = Path.GetExtension(OutputPath) ?? string.Empty;

            var generatorFactory = Generators.FirstOrDefault(g => g.Extensions.Contains(ext, StringComparer.OrdinalIgnoreCase));
            if (generatorFactory == null)
                throw new InvalidOperationException("Cannot detect the target file format");

            Logger.Info($"Target format is {generatorFactory.Name}.");

            var generator = generatorFactory.CreateGenerator();
            // ReSharper disable once SuspiciousTypeConversion.Global
            using (generator as IDisposable)
            {
                generator.LogProgress += (obj, message) => Logger.Info(message);
                generator.GenerateDocumentation(api, OutputPath);
            }
        }

        public static int Main(string[] args)
        {
            try
            {
                var program = new Program(args);

                program.Run();

                Logger.Info($"Spec [{program.SpecPath}] successfully formatted to [{program.OutputPath}].");
                return 0;
            }
            catch (Exception error)
            {
                Logger.Fatal("FATAL ERROR.", error);
                return 1;
            }
        }

        private static void OutputPathParser(Program program, string[] args, ref int index)
        {
            if (index >= args.Length)
                throw new InvalidOperationException("No output path specified");

            program.OutputPath = Path.GetFullPath(args[index++]);
        }
    }
}
