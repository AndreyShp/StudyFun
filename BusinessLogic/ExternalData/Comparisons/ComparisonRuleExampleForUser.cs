using BusinessLogic.Data.Comparison;

namespace BusinessLogic.ExternalData.Comparisons {
    public class ComparisonRuleExampleForUser {
        internal ComparisonRuleExampleForUser(ComparisonRuleExample ruleExample, SourceWithTranslation example)
            : this(example, ruleExample.Description) {
            Id = ruleExample.Id;
        }

        public ComparisonRuleExampleForUser(SourceWithTranslation example, string description) {
            Example = example;
            Description = description;
        }

        internal long Id { get; private set; }

        public SourceWithTranslation Example { get; private set; }

        public string Description { get; private set; }

        internal bool IsValid() {
            return Example != null;
        }
    }
}