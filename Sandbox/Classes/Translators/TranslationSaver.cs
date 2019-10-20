using System;
using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace Sandbox.Classes.Translators {
    public class TranslationSaver : IDisposable {
        private readonly string _fileName;
        private readonly XmlWriter _xmlWriter = new XmlWriter();

        private bool _isSaved;

        public TranslationSaver(string fileName) {
            _fileName = fileName;
        }

        #region IDisposable Members

        public void Dispose() {
            Save();
        }

        #endregion

        public void Write(SourceWithTranslation sourceWithTranslation, List<string> best, List<string> other) {
            var element = new XElement("item",
                                       new XElement("source", sourceWithTranslation.Source.Text),
                                       new XElement("translation", sourceWithTranslation.Translation.Text)
                );

            TranslationsToElement(element, "best", best);
            TranslationsToElement(element, "other", other);
            _xmlWriter.Write(element);
        }

        private static void TranslationsToElement(XElement container, string name, List<string> translations) {
            if (EnumerableValidator.IsEmpty(translations)) {
                return;
            }

            foreach (string translation in translations) {
                container.Add(new XElement(name, translation));
            }
        }

        public void Save() {
            if (_isSaved) {
                return;
            }

            _isSaved = true;
            _xmlWriter.Save(_fileName);
        }
    }
}