using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SwagDoc.Parsing
{
    public class SwagApi
    {
        public SwagApi(JObject spec)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            Spec = spec;
            BasePath = Spec.Value<string>("basePath") ?? string.Empty;
            Resources = ((JObject)spec["paths"]).Properties().SelectMany(
                path => ((JObject)path.Value).Properties().Select(
                    method => new SwagResource(this, method.Name, path.Name, (JObject)method.Value)
                    )).ToArray();

            var info = (JObject)Spec["info"];
            Title = info.Value<string>("title") ?? string.Empty;
            Description = info.Value<string>("description") ?? string.Empty;
            Version = info.Value<string>("version") ?? string.Empty;
        }

        public string Title { get; }

        public string Description { get; }

        public string Version { get; }

        public string BasePath { get; }

        public IReadOnlyList<SwagResource> Resources { get; }

        public JObject Spec { get; }

        public override string ToString()
        {
            var result = Title;
            if (string.IsNullOrEmpty(result))
                return base.ToString();

            if (!string.IsNullOrEmpty(Version))
            {
                result = $"{result} v{Version}";
            }

            return result;
        }

        public string GetAbsolutePath(string path) => Regex.Replace("/" + BasePath + "/" + path, "/+", "/");

        public JObject GetRef(string @ref)
        {
            if (@ref == null) throw new ArgumentNullException(nameof(@ref));
            if (!@ref.StartsWith("#/")) throw new FormatException($"Invalid ref [{@ref}]");

            JObject result = Spec;
            foreach (var segment in @ref.Substring(2).Split('/'))
            {
                result = (JObject)result[segment];
                if (result == null) throw new FormatException($"Invalid ref [{@ref}]");
            }
            return result;
        }

        public static string GetSwagType(JObject spec)
        {
            var schema = (JObject)spec["schema"];
            if (schema != null)
                return GetSwagType(schema);

            var @enum = (JArray)spec["enum"];
            if (@enum != null)
                return string.Join("|", @enum);

            var type = spec.Value<string>("type");
            if (string.IsNullOrEmpty(type))
                return "???";

            var format = spec.Value<string>("format");
            if (!string.IsNullOrEmpty(format))
            {
                type = $"{type}({format})";
            }

            return type;
        }

        public string BuildSample(JObject typeSpec)
        {
            if (typeSpec == null)
                return string.Empty;

            using (var text = new StringWriter())
            using (var json = new JsonTextWriter(text) { Formatting = Formatting.Indented, Indentation = 4 })
            {
                BuildSample(typeSpec, json);
                json.Flush();
                return text.ToString();
            }
        }

        public static SwagApi LoadFromJson(JsonReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            var spec = JObject.Load(reader);
            return new SwagApi(spec);
        }

        public static SwagApi LoadFromJson(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            using (var jsonReader = new JsonTextReader(reader) { CloseInput = false })
            {
                return LoadFromJson(jsonReader);
            }
        }

        public static SwagApi LoadFromJson(string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            using (var reader = new StringReader(json))
            {
                return LoadFromJson(reader);
            }
        }

        public static SwagApi LoadFromStream(Stream stream, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));

            using (var reader = new StreamReader(stream, encoding, false, 1024, true))
            {
                return LoadFromJson(reader);
            }
        }

        public static SwagApi LoadFromStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                return LoadFromJson(reader);
            }
        }

        public static SwagApi LoadFromFile(string filePath, Encoding encoding)
        {
            using (var stream = OpenFile(filePath))
            {
                return LoadFromStream(stream);
            }
        }

        public static SwagApi LoadFromFile(string filePath)
        {
            using (var stream = OpenFile(filePath))
            {
                return LoadFromStream(stream);
            }
        }

        private static FileStream OpenFile(string filePath) => new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        private void BuildSample(JObject typeSpec, JsonWriter target, bool innerLevel = false, bool merge = false)
        {
            var schema = (JObject)typeSpec["schema"];
            if (schema != null)
            {
                BuildSample(schema, target, true, merge);
                return;
            }

            var allOf = (JArray)typeSpec["allOf"];
            if (allOf != null)
            {
                if (!merge)
                {
                    target.WriteStartObject();
                }

                foreach (var jobj in allOf.Cast<JObject>())
                {
                    BuildSample(jobj, target, true, true);
                }

                if (!merge)
                {
                    target.WriteEndObject();
                }

                return;
            }

            var @ref = typeSpec.Value<string>("$ref");
            if (!string.IsNullOrEmpty(@ref))
            {
                BuildSample(GetRef(@ref), target, true, merge);
                return;
            }

            var type = typeSpec.Value<string>("type");
            if (type == "array")
            {
                target.WriteStartArray();
                BuildSample((JObject)typeSpec["items"], target, true);
                target.WriteEndArray();
            }
            else if (type == "object")
            {
                if (!merge)
                {
                    target.WriteStartObject();
                }

                foreach (var prop in ((JObject)typeSpec["properties"]).Properties())
                {
                    target.WritePropertyName(prop.Name);
                    BuildSample((JObject)prop.Value, target, true);
                }

                if (!merge)
                {
                    target.WriteEndObject();
                }
            }
            else if (innerLevel || !string.IsNullOrEmpty(type))
            {
                target.WriteValue(GetSwagType(typeSpec));
            }
        }
    }
}
