using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable InconsistentNaming

namespace YaasServicePatterns.PatternSupport
{
    public class QueryableParameters {
        public QueryableParameters() : this(null){}

        public QueryableParameters(params KeyValuePair<string, string>[] queryParameters) {
            queryParameters = queryParameters ?? new KeyValuePair<string, string>[]{};

            var nameValueCollection = new NameValueCollection();
            foreach (var pair in queryParameters)
                nameValueCollection.Add(pair.Key, pair.Value);
            QueryParameters = nameValueCollection;
        }

        public string q {
            get {
                return BuildQueryStringParameter(QueryParameters);
            }
            set {
                var regex = new Regex("(^|[ \\+])((?<key>\\w+):(?<value>[^\" \\+]*(\"[^\"]*\"[^\" \\+]*)*))");

                var matches = regex.Matches(value);
                if (matches.Count == 0 && !string.IsNullOrEmpty(value)) {
                    // TODO: Set up validation to handle this case
                }

                var nameValueCollection = new NameValueCollection();

                foreach (Match match in matches) {
                    var attribute = match.Groups["key"].Value;
                    var attributeValue = match.Groups["value"].Value;

                    if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeValue)) {
                        nameValueCollection.Add(attribute, attributeValue);
                    }
                }

                QueryParameters = nameValueCollection;
            }
        }

        public NameValueCollection QueryParameters { get; set; }

        private string BuildQueryStringParameter(NameValueCollection queryParameters) {
            var pairs = from key in queryParameters.AllKeys
                        from value in queryParameters.GetValues(key)
                        select $"{key}:{value}";
            return string.Join(" ", pairs);
        }

        public IEnumerable<KeyValuePair<string, string>> GetUrlQueryParameters() {
            if (!string.IsNullOrEmpty(q))
                yield return new KeyValuePair<string, string>("q", q);
        }
    }
}