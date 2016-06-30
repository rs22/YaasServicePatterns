using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace YaasServicePatterns.PatternSupport {
    public class PermissionAwareParameters {
        public PermissionAwareParameters() : this(null){}
        
        public PermissionAwareParameters(IEnumerable<KeyValuePair<string, string>> evaluateRoleAssignmentsParameters) {
            evaluateRoleAssignmentsParameters = evaluateRoleAssignmentsParameters ?? Enumerable.Empty<KeyValuePair<string, string>>();
            
            var nameValueCollection = new NameValueCollection();
            foreach (var pair in evaluateRoleAssignmentsParameters)
                nameValueCollection.Add(pair.Key, pair.Value);
            EvaluateRoleAssignmentsParameters = nameValueCollection;
        }
        
        public string[] evaluateRoleAssignments { 
            get {
                return BuildEvaluateRoleAssignmentsParameter(EvaluateRoleAssignmentsParameters);
            } 
            set {
                // TODO: This has to be improved (e.g. with regex)
                var pairs = from condition in value
                            let pairValues = condition.Split(new [] { ':' }, StringSplitOptions.RemoveEmptyEntries)
                            where pairValues.Length == 2
                            select new { Name = pairValues[0], Value = pairValues[1] };
                var nameValueCollection = new NameValueCollection();
                foreach (var pair in pairs)
                    nameValueCollection.Add(pair.Name, pair.Value);
                EvaluateRoleAssignmentsParameters = nameValueCollection;
            } 
        }
        
        public NameValueCollection EvaluateRoleAssignmentsParameters { get; set; }
        
        private string[] BuildEvaluateRoleAssignmentsParameter(NameValueCollection evaluateRoleAssignmentsParameters) {
            return (from key in evaluateRoleAssignmentsParameters.AllKeys
                    from value in evaluateRoleAssignmentsParameters.GetValues(key)
                    select $"{key}:{value}").ToArray();
        }
        
        public IEnumerable<KeyValuePair<string, string>> GetUrlQueryParameters() {
            if (evaluateRoleAssignments != null && evaluateRoleAssignments.Length > 0)
                return evaluateRoleAssignments.Select(x => new KeyValuePair<string, string>("evaluateRoleAssignments", x));
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }
    }
}