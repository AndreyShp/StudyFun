using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using BusinessLogic.Data;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Comparisons;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Comparisons;
using BusinessLogic.Validators;

namespace Sandbox.Classes {
    public class GroupComparisonFiller {
        private long _englishId = IdValidator.INVALID_ID;
        private bool _isLoaded;
        private long _russianId = IdValidator.INVALID_ID;

        public void Create(string fileName) {
            if (!_isLoaded) {
                _isLoaded = true;
                Load();
            }

            var reader = new StreamReader(fileName, Encoding.UTF8);
            XDocument xDocument = XDocument.Load(reader);
            XElement xComparison = xDocument.Element("comparison");
            string title = GetChildElementValue(xComparison, "title");
            string description = GetChildElementValue(xComparison, "description");

            var comparisonForUser = new ComparisonForUser(TextFormatter.FirstUpperCharAndTrim(title),
                                                          TextFormatter.FirstUpperCharAndTrim(description));

            IEnumerable<XElement> info = GetChildElements(xComparison, "additionalInfo");
            foreach (XElement xElement in info.Elements()) {
                var type = (AdditionalType) Enum.Parse(typeof (AdditionalType), xElement.Name.ToString());
                string value = TextFormatter.FirstUpperCharAndTrim(xElement.Value);
                comparisonForUser.AddAdditional(type, value);
            }

            XElement xItems = GetChildElement(xComparison, "items");
            foreach (XElement xItem in GetChildElements(xItems, "item")) {
                ComparisonItemForUser comparisonItem = GetComparisonItemForUser(xItem);
                comparisonForUser.AddItem(comparisonItem);
            }

            var comparisonsQuery = new ComparisonsQuery(_englishId);
            ComparisonForUser savedComparisonForUser = comparisonsQuery.GetOrCreate(comparisonForUser);
            if (savedComparisonForUser != null) {
                Console.WriteLine("Сохранена группа сравнений {0}", fileName);
            } else {
                Console.WriteLine("Не удалось сохранить группу сравнений {0}", fileName);
            }
        }

        private void Load() {
            var languages = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            Language english = languages.GetByShortName(LanguageShortName.En);
            _englishId = english.Id;
            Language russian = languages.GetByShortName(LanguageShortName.Ru);
            _russianId = russian.Id;
        }

        private ComparisonItemForUser GetComparisonItemForUser(XElement xItem) {
            string title = GetChildElementValue(xItem, "title");
            string titleTranslated = GetChildElementValue(xItem, "titleTranslated");
            string description = GetChildElementValue(xItem, "description");

            var comparisonItem = new ComparisonItemForUser(TextFormatter.ToLowerAndNotTouchFirstCharWords(title),
                                                           TextFormatter.ToLowerAndNotTouchFirstCharWords(
                                                               titleTranslated),
                                                           TextFormatter.FirstUpperCharAndTrim(description));
            XElement xRules = GetChildElement(xItem, "rules");
            foreach (XElement xRule in GetChildElements(xRules, "rule")) {
                ComparisonRuleForUser rule = GetComparisonRuleForUser(xRule);
                comparisonItem.AddRule(rule);
            }

            return comparisonItem;
        }

        private ComparisonRuleForUser GetComparisonRuleForUser(XElement xRule) {
            string description = GetChildElementValue(xRule, "description");
            var rule = new ComparisonRuleForUser(TextFormatter.FirstUpperCharAndTrim(description));

            XElement xExamples = GetChildElement(xRule, "examples");
            foreach (XElement xExample in GetChildElements(xExamples, "example")) {
                ComparisonRuleExampleForUser example = GetComparisonExampleForUser(xExample);
                rule.AddExample(example);
            }

            return rule;
        }

        private ComparisonRuleExampleForUser GetComparisonExampleForUser(XElement xExample) {
            string source = GetChildElementValue(xExample, "source");
            string translation = GetChildElementValue(xExample, "translation");
            string descriptionExample = GetChildElementValue(xExample, "description");

            var sourceWithTranslation = new SourceWithTranslation();
            sourceWithTranslation.Set(IdValidator.INVALID_ID,
                                      new PronunciationForUser(IdValidator.INVALID_ID,
                                                               TextFormatter.FirstUpperCharAndTrim(source), false,
                                                               _englishId),
                                      new PronunciationForUser(IdValidator.INVALID_ID,
                                                               TextFormatter.FirstUpperCharAndTrim(translation), false,
                                                               _russianId));
            return new ComparisonRuleExampleForUser(sourceWithTranslation,
                                                    TextFormatter.FirstUpperCharAndTrim(descriptionExample));
        }

        private static string GetChildElementValue(XElement xParent, string name) {
            XElement xChildElement = GetChildElement(xParent, name);
            return xChildElement != null ? xChildElement.Value : null;
        }

        private static XElement GetChildElement(XElement xParent, string name) {
            return xParent != null ? xParent.Element(name) : null;
        }

        private static IEnumerable<XElement> GetChildElements(XElement xParent, string name) {
            return xParent != null ? xParent.Elements(name) : new XElement[0];
        }
    }
}