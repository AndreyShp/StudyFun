using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery {
    public class LanguagesQuery : BaseQuery, ILanguagesQuery {
        private readonly LanguageShortName _defaultFrom;
        private readonly LanguageShortName _defaultTo;

        private List<Language> _languages;
        private Dictionary<long, Language> _languagesByIds;
        private Dictionary<LanguageShortName, Language> _languagesByShortNames;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="defaultFrom">по-умолчанию язык, с которого нужно переводить</param>
        /// <param name="defaultTo">по-умолчанию язык, на который нужно переводить</param>
        public LanguagesQuery(LanguageShortName defaultFrom, LanguageShortName defaultTo) {
            _defaultFrom = defaultFrom;
            _defaultTo = defaultTo;
        }

        #region ILanguagesQuery Members

        public UserLanguages GetLanguages(List<long> preferLanguagesIds) {
            LoadLanguages();

            var result = new UserLanguages();
            if (preferLanguagesIds != null && preferLanguagesIds.Count == 2
                && preferLanguagesIds[0] != preferLanguagesIds[1]) {
                result.From = GetById(preferLanguagesIds[0]);
                result.To = GetById(preferLanguagesIds[1]);
            }

            if (IsInvalid(result)) {
                result.From = GetByShortName(_defaultFrom);
                result.To = GetByShortName(_defaultTo);
            }
            return result;
        }

        public UserLanguages GetLanguagesByShortNames(LanguageShortName source, LanguageShortName translation) {
            if (!DefaultContainsLanguage(source) || !DefaultContainsLanguage(translation) || source == translation) {
                return new UserLanguages();
            }

            LoadLanguages();

            var result = new UserLanguages {
                From = GetByShortName(source),
                To = GetByShortName(translation)
            };
            return result;
        }

        public Language GetById(long id) {
            if (IdValidator.IsInvalid(id)) {
                return null;
            }

            LoadLanguages();

            Language result;
            return _languagesByIds.TryGetValue(id, out result) ? result : null;
        }

        public Language GetByShortName(LanguageShortName shortName) {
            if (EnumValidator.IsInvalid(shortName) || shortName == LanguageShortName.Unknown) {
                return null;
            }

            LoadLanguages();
            Language result;
            return _languagesByShortNames.TryGetValue(shortName, out result) ? result : null;
        }

        public LanguageShortName GetShortNameById(long id) {
            Language language = GetById(id);
            if (language == null) {
                return LanguageShortName.Unknown;
            }

            return ParseShortName(language.ShortName);
        }

        #endregion

        private bool IsInvalid(UserLanguages userLanguages) {
            if (UserLanguages.IsInvalid(userLanguages)) {
                return true;
            }

            if (!IsShortNameValid(userLanguages.From)) {
                return true;
            }

            return !IsShortNameValid(userLanguages.To);
        }

        private bool IsShortNameValid(Language language) {
            LanguageShortName shortName = ParseShortName(language.ShortName);
            return shortName != LanguageShortName.Unknown && DefaultContainsLanguage(shortName);
        }

        private bool DefaultContainsLanguage(LanguageShortName shortName) {
            return _defaultFrom == shortName || _defaultTo == shortName;
        }

        private void LoadLanguages() {
            if (EnumerableValidator.IsNotNullAndNotEmpty(_languages)) {
                //ранее уже загрузили языки
                return;
            }

            Adapter.ActionByContext(c => {
                _languages = c.Language.ToList();
                _languagesByIds = new Dictionary<long, Language>(_languages.Count);
                _languagesByShortNames = new Dictionary<LanguageShortName, Language>(_languages.Count);
                foreach (Language language in _languages) {
                    LanguageShortName parsedShortName = ParseShortName(language.ShortName);
                    if (parsedShortName == LanguageShortName.Unknown) {
                        continue;
                    }

                    _languagesByIds.Add(language.Id, language);
                    _languagesByShortNames.Add(parsedShortName, language);
                }
            });
        }

        public static LanguageShortName ParseShortName(string shortName) {
            LanguageShortName parsedShortName;
            if (!string.IsNullOrWhiteSpace(shortName) && Enum.TryParse(shortName, true, out parsedShortName)) {
                return parsedShortName;
            }
            //TODO: логировать
            return LanguageShortName.Unknown;
        }
    }
}