using System;
using System.Net;
using Humanizer;
using Newtonsoft.Json.Linq;

namespace SwagDoc.Parsing
{
    public class SwagResponse
    {
        public SwagResponse(SwagResource resource, string statusCode, JObject spec)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            if (string.IsNullOrEmpty(statusCode)) throw new ArgumentNullException(nameof(statusCode));
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            Resource = resource;
            Spec = spec;
            StatusCode = int.Parse(statusCode);
            Status = NormalizeStatus(Enum.GetName(typeof(HttpStatusCode), StatusCode));
            Description = spec.Value<string>("description") ?? string.Empty;
            Body = resource.Api.BuildSample(Spec);
        }

        public SwagResource Resource { get; }

        public int StatusCode { get; }

        public string Status { get; }

        public string Description { get; }

        public string Body { get; }

        public JObject Spec { get; }

        public override string ToString()
        {
            var res = StatusCode.ToString();

            if (!string.IsNullOrEmpty(Status))
            {
                res += " " + Status;
            }

            return res;
        }

        private static string NormalizeStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return string.Empty;

            return status.Titleize();
        }
    }
}
