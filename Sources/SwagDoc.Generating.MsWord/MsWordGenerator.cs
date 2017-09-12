using System;
using System.Linq;
using System.Collections.Generic;

using SwagDoc.Parsing;
using SwagDoc.Generating.MsWord.Configuration;

namespace SwagDoc.Generating.MsWord
{
    public class MsWordGenerator : IDocumentGenerator
    {
        public static class Placeholders
        {
            public const string Title = "{{swagdoc.title}}";
            public const string Version = "{{swagdoc.version}}";
            public const string Description = "{{swagdoc.description}}";
            public const string Resources = "{{swagdoc.resources}}";
        }

        private readonly IMsWordGeneratorConfiguration _configuration;

        public MsWordGenerator(IMsWordGeneratorConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _configuration = configuration;
        }

        public event EventHandler<string> LogProgress;

        public void GenerateDocumentation(SwagApi api, string outputPath)
        {
            Progress("Creating document");

            using (var winword = new MsWordApplication(_configuration.ShowApplication))
            using (var document = !string.IsNullOrEmpty(_configuration.Template)
                ? winword.OpenDocument(_configuration.Template)
                : winword.CreateDocument())
            {
                Progress("Writing document title");

                document.Replace(Placeholders.Title, api.Title);
                document.Replace(Placeholders.Version, api.Version);
                document.Replace(Placeholders.Description, api.Description);

                document.InsertLocation = document.LocatePlaceholder(Placeholders.Resources) ?? document.EndOfDocument;

                int resourceIndex = 1;
                if (_configuration.GroupResources)
                {
                    Progress("Processing resource groups");

                    var groups = api.Resources.GroupBy(x => x.Tags.FirstOrDefault() ?? string.Empty).ToList();

                    var emptyGroup = groups.FirstOrDefault(g => string.IsNullOrEmpty(g.Key));
                    if (emptyGroup != null)
                    {
                        Progress("Processing untagged resource group");
                        WriteResourceGroup(document, _configuration.Captions.CommonGroup, emptyGroup, ref resourceIndex, api.Resources.Count);
                    }

                    foreach (var group in groups.Where(g => !string.IsNullOrEmpty(g.Key)))
                    {
                        Progress($"Processing resource group [{group.Key}]");
                        WriteResourceGroup(document, group.Key, group, ref resourceIndex, api.Resources.Count);
                    }
                }
                else
                {
                    Progress("Processing resources without groupping");

                    foreach (var resource in api.Resources)
                    {
                        WriteResource(document, resource, ref resourceIndex, api.Resources.Count);
                    }
                }

                document.InsertLocation.Delete();
                document.SaveAs(outputPath);
            }
        }

        private void Progress(string message) => LogProgress?.Invoke(this, message);

        private void WriteResourceGroupTitle(MsWordDocument document, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            document.InsertParagraph(text, _configuration.Styles.ResourceGroupTitle);
        }

        private void WriteResourceGroup(MsWordDocument document, string title, IEnumerable<SwagResource> resources,
            ref int resourceIndex, int resourceCount)
        {
            WriteResourceGroupTitle(document, title);

            foreach (var resource in resources)
            {
                WriteResource(document, resource, ref resourceIndex, resourceCount);
            }
        }

        private void WriteResource(MsWordDocument document, SwagResource resource, ref int resourceIndex, int resourceCount)
        {
            Progress($"Processing {resourceIndex}/{resourceCount} {resource}");
            resourceIndex++;

            document.InsertParagraph(resource.Description, _configuration.Styles.ResourceTitle)
                .InsertParagraph(resource.ToString(), _configuration.Styles.ResourceRequest);

            WriteDescription(document, resource.Description);
            WriteParameters(document, _configuration.Captions.PathParameters, resource.PathParameters);
            WriteParameters(document, _configuration.Captions.QueryParameters, resource.QueryParameters);
            WriteBody(document, _configuration.Captions.RequestBody, resource.Request);

            foreach (var response in resource.Responses)
            {
                document.InsertParagraph(response.ToString(), _configuration.Styles.ResponseTitle);
                WriteDescription(document, response.Description);
                WriteBody(document, _configuration.Captions.ResponseBody, response.Body);
            }
        }

        private void WriteDescription(MsWordDocument document, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            document.InsertParagraph(text, _configuration.Styles.Description);
        }

        private void WriteCaption(MsWordDocument document, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            document.InsertParagraph(text, _configuration.Styles.Caption);
        }

        private void WriteParameters(MsWordDocument document, string caption, IReadOnlyList<SwagParameter> parameters)
        {
            if (parameters.Count == 0) return;

            WriteCaption(document, caption);

            var tableValues = new string[parameters.Count, 3];
            for (int ii = 0; ii < parameters.Count; ii++)
            {
                var parameter = parameters[ii];
                tableValues[ii, 0] = parameter.Name;
                tableValues[ii, 1] = parameter.Type;
                tableValues[ii, 2] = parameter.Description;
            }
            document.InsertTable(tableValues);
        }

        private void WriteBody(MsWordDocument document, string caption, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            WriteCaption(document, caption);
            document.InsertParagraph(text, _configuration.Styles.Content);
        }
    }
}
