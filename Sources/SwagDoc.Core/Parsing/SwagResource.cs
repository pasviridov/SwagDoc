using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SwagDoc.Parsing
{
    public class SwagResource
    {
        public SwagResource(SwagApi api, string method, string path, JObject spec)
        {
            if (api == null) throw new ArgumentNullException(nameof(api));
            if (string.IsNullOrEmpty(method)) throw new ArgumentNullException(nameof(method));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            Api = api;
            Method = method.ToUpperInvariant();
            Path = path;
            Spec = spec;
            Description = spec.Value<string>("description");
            Tags = ((JArray)spec["tags"]).Cast<JValue>().Select(x => (string)x.Value).ToArray();
            PathParameters = GetParameters(spec, "path");
            QueryParameters = GetParameters(spec, "query");

            var body = ((JArray)spec["parameters"] ?? new JArray()).Cast<JObject>().FirstOrDefault(p => p.Value<string>("in") == "body");
            Request = Api.BuildSample(body);

            Responses = ((JObject)spec["responses"] ?? new JObject()).Properties()
                .Select(p => new SwagResponse(this, p.Name, (JObject)p.Value))
                .ToArray();
        }

        public SwagApi Api { get; }

        public string Method { get; }

        public string Path { get; }

        public string Description { get; }

        public string[] Tags { get; }

        public IReadOnlyList<SwagParameter> PathParameters { get; }

        public IReadOnlyList<SwagParameter> QueryParameters { get; }

        public string Request { get; }

        public IReadOnlyList<SwagResponse> Responses { get; }

        public JObject Spec { get; set; }

        public override string ToString() => $"{Method} {Api.GetAbsolutePath(Path)}";

        private static IReadOnlyList<SwagParameter> GetParameters(JObject spec, string @in)
        {
            return ((JArray)spec["parameters"] ?? new JArray()).Cast<JObject>()
                .Where(p => p.Value<string>("in") == @in)
                .Select(p => new SwagParameter(p))
                .ToArray();
        }
    }
}
