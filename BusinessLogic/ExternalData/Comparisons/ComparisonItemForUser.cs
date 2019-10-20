using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Comparison;
using BusinessLogic.Validators;

namespace BusinessLogic.ExternalData.Comparisons {
    public class ComparisonItemForUser {
        internal ComparisonItemForUser(ComparisonItem comparisonItem)
            : this(comparisonItem.Title, comparisonItem.TitleTranslation, comparisonItem.Description) {
            Id = comparisonItem.Id;
        }

        public ComparisonItemForUser(string title, string titleTranslated, string description) {
            Title = title;
            TitleTranslated = titleTranslated;
            Description = description;
            Rules = new List<ComparisonRuleForUser>();
        }

        internal long Id { get; private set; }

        public string Title { get; private set; }

        public bool HasTranslatedTitle {
            get { return !string.IsNullOrWhiteSpace(TitleTranslated); }
        }

        public string TitleTranslated { get; private set; }

        public string Description { get; private set; }

        public List<ComparisonRuleForUser> Rules { get; private set; }

        public bool IsOneRule {
            get { return Rules.Count == 1; }
        }

        public string GetRuleHeader() {
            return IsOneRule ? "Правило употребления:" : "Правила употребления:";
        }

        public void AddRule(ComparisonRuleForUser rule) {
            Rules.Add(rule);
        }

        internal bool IsValid() {
            return !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(TitleTranslated)
                   && EnumerableValidator.IsNotEmpty(Rules) && Rules.All(e => e.IsValid());
        }
    }
}