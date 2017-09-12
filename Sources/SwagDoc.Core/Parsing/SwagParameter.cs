using System;
using Newtonsoft.Json.Linq;

namespace SwagDoc.Parsing
{
    public class SwagParameter
    {
        public SwagParameter(JObject spec)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            Spec = spec;
            Name = Spec.Value<string>("name");
            Type = SwagApi.GetSwagType(Spec);
            Description = spec.Value<string>("description") ?? string.Empty;
        }

        public string Name { get; }

        public string Type { get; }

        public string Description { get; }

        public JObject Spec { get; }

        public override string ToString() => $"{Name} {Type}";
    }
}
