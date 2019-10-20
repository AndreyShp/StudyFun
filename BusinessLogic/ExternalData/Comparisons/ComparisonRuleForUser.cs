using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Comparison;
using BusinessLogic.Validators;

namespace BusinessLogic.ExternalData.Comparisons {
    public class ComparisonRuleForUser {
        public ComparisonRuleForUser(ComparisonRule comparisonRule) : this(comparisonRule.Description) {
            Id = comparisonRule.Id;
        }

        public ComparisonRuleForUser(string description) {
            Description = description;
            Examples = new List<ComparisonRuleExampleForUser>();
        }

        internal long Id { get; private set; }

        public string Description { get; private set; }

        public List<ComparisonRuleExampleForUser> Examples { get; private set; }

        public void AddExample(ComparisonRuleExampleForUser sourceWithTranslation) {
            Examples.Add(sourceWithTranslation);
        }

        internal bool IsValid() {
            return !string.IsNullOrWhiteSpace(Description) && EnumerableValidator.IsNotEmpty(Examples)
                   && Examples.All(e => e.IsValid());
        }
    }
}