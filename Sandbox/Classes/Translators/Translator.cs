using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace Sandbox.Classes.Translators {
    internal class Translator {
        private readonly Dictionary<string, int> _bestTranslations = new Dictionary<string, int>();
        private readonly ITranslator[] _extraTranslators = new ITranslator[] {
            new GoogleTranslator()
        };
        private readonly Dictionary<string, string> _initialsByNormalized = new Dictionary<string, string>();
        private readonly Dictionary<string, int> _otherTranslations = new Dictionary<string, int>();

        private readonly ITranslator[] _translators = new ITranslator[] {
            new MicrosoftTranslator(),
            new YandexTranslator()
        };

        public int CountExtraCalls { get; private set; }

        /// <summary>
        /// Получить другие переводы, которые не являются лучшими
        /// </summary>
        /// <returns>все переводы</returns>
        public List<string> GetOtherTranslations() {
            return _otherTranslations.Select(e => GetInitial(e.Key)).ToList();
        }

        /// <summary>
        /// Переводит на язык to, тексты которые указаны в textsByFromLanguages
        /// </summary>
        /// <param name="to">язык на который нужно перевести</param>
        /// <param name="textsWithLanguages">список текстов на разных, которые означают одно и то же и значение перевод у которых будет одно и тоже</param>
        /// <returns>наилучшии общие переводы для текстов на разных языках; пустой список если не удалось определить наилучшие переводы</returns>
        public List<string> Translate(LanguageShortName to,
                                      IEnumerable<Tuple<LanguageShortName, string>> textsWithLanguages) {
            _bestTranslations.Clear();
            _otherTranslations.Clear();
            _initialsByNormalized.Clear();

            Translate(_translators, to, textsWithLanguages);
            if (EnumerableValidator.IsEmpty(_bestTranslations) && EnumerableValidator.IsNotEmpty(_extraTranslators)) {
                //наилучшие переводы не найдены - перевести с помощью дополнительных переводчиков
                Console.WriteLine("Воспользуемся дополнительными переводчиками!!!");
                CountExtraCalls++;
                Translate(_extraTranslators, to, textsWithLanguages);
            }

            return GetBestTranslations();
        }

        private void Translate(ITranslator[] translators,
                               LanguageShortName to,
                               IEnumerable<Tuple<LanguageShortName, string>> textsWithLanguages) {
            foreach (var textWithLanguage in textsWithLanguages) {
                LanguageShortName from = textWithLanguage.Item1;
                string text = textWithLanguage.Item2;

                //перевести текст с языка на другой язык
                Translate(translators, from, to, text);
                if (EnumerableValidator.IsNotEmpty(_bestTranslations)) {
                    //найдены лучшие переводы - вернуть
                    return;
                }

                //лучшие переводы не найдены - попробовать перевести это же слово, но уже с другого языка
            }
        }

        private List<string> GetBestTranslations() {
            List<string> bestTranslations =
                _bestTranslations.OrderByDescending(e => e.Value).Select(e => GetInitial(e.Key)).ToList();
            return bestTranslations;
        }

        private string GetInitial(string text) {
            return _initialsByNormalized[text];
        }

        private void Translate(IEnumerable<ITranslator> translators,
                               LanguageShortName from,
                               LanguageShortName to,
                               string text) {
            foreach (ITranslator translator in translators) {
                List<string> translations = translator.Translate(from, to, text);
                if (translations == null) {
                    //TODO: логировать
                    continue;
                }

                //считаем что перевод не может совпадать с исходным текстом(т.к. переводчики возвращают исходный текст, когда не знают как его перевести)
                translations =
                    translations.Where(e => !string.Equals(e, text, StringComparison.InvariantCultureIgnoreCase)).Select(e => e).ToList();
                if (EnumerableValidator.IsEmpty(translations)) {
                    //TODO: логировать
                    continue;
                }

                foreach (string translation in translations) {
                    string normalizedTranslation = translation.Trim().ToLowerInvariant();
                    if (_bestTranslations.ContainsKey(normalizedTranslation)) {
                        _bestTranslations[normalizedTranslation]++;
                        continue;
                    }

                    if (_otherTranslations.ContainsKey(normalizedTranslation)) {
                        _bestTranslations.Add(normalizedTranslation, 2);
                        _otherTranslations.Remove(normalizedTranslation);
                    } else {
                        _initialsByNormalized.Add(normalizedTranslation, translation);
                        _otherTranslations.Add(normalizedTranslation, 1);
                    }
                }
            }
        }
    }
}