using BusinessLogic.Export;
using BusinessLogic.ExternalData.Comparisons;
using BusinessLogic.Helpers;
using BusinessLogic.Validators;

namespace BusinessLogic.Downloaders {
    public class ComparisonDownloader {
        private readonly string _domain;
        private readonly string _fontPath;

        public ComparisonDownloader(string domain, string fontPath) {
            _domain = domain;
            _fontPath = fontPath;
        }

        public DocumentationGenerator Download(DocumentType docType, string fileName, ComparisonForUser comparisonForUser) {
            string header = comparisonForUser.Title;
            DocumentationGenerator documentGenerator = DocumentationGenerator.Create(_domain, _fontPath, docType, fileName);

            documentGenerator.AddHeader(header, false);
            if (comparisonForUser.HasDescription) {
                documentGenerator.AddParagraph(TextFormatter.AppendCharIfNeed(comparisonForUser.Description));
            }
            foreach (ComparisonItemForUser item in comparisonForUser.Items) {
                string title = item.Title;
                if (item.HasTranslatedTitle) {
                    title += " – " + item.TitleTranslated;
                }
                documentGenerator.AddSubheader(title);
                documentGenerator.AddParagraph(item.GetRuleHeader());
                int ruleNumber = 1;
                foreach (ComparisonRuleForUser rule in item.Rules) {
                    string ruleDescription = TextFormatter.AppendCharIfNeed(rule.Description);
                    if (!item.IsOneRule) {
                        ruleDescription = ruleNumber + ". " + ruleDescription;
                        ruleNumber++;
                    }
                    documentGenerator.AddParagraph(ruleDescription);
                    if (EnumerableValidator.IsNotEmpty(rule.Examples)) {
                        foreach (ComparisonRuleExampleForUser example in @rule.Examples) {
                            documentGenerator.AddParagraph(example.Example.Source.Text + " – "
                                                           + example.Example.Translation.Text,
                                                           new TextFormat {CountLeftPaddings = 1});
                            if (!string.IsNullOrWhiteSpace(example.Description)) {
                                documentGenerator.AddParagraph("Пояснение к примеру: "
                                                               + TextFormatter.AppendCharIfNeed(example.Description),
                                                               new TextFormat {IsItalic = true, CountLeftPaddings = 1});
                            }
                        }
                    }
                }
            }

            return documentGenerator;
        }
    }
}