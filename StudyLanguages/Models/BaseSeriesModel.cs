using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Models {
    public class BaseSeriesModel<T> : BaseLanguageModel where T : class, ISeries {
        private const int DEFAULT_INDEX = -1;

        private int _currentIndex = DEFAULT_INDEX;

        public BaseSeriesModel(UserLanguages userLanguages, List<T> elemsWithTranslations)
            : base(userLanguages) {
            ElemsWithTranslations = elemsWithTranslations ?? new List<T>(0);
        }

        /// <summary>
        /// Элементы пользователя
        /// </summary>
        public List<T> ElemsWithTranslations { get; private set; }

        /// <summary>
        /// Получает текущий элемент
        /// </summary>
        /// <returns></returns>
        public T GetCurrent() {
            for (int i = 0; i < ElemsWithTranslations.Count; i++) {
                T currentGroupWord = ElemsWithTranslations[i];
                if (currentGroupWord.IsCurrent) {
                    _currentIndex = i;
                    return currentGroupWord;
                }
            }
            return null;
        }

        public T GetPrev() {
            return GetElemByIndexIfNotFirst(_currentIndex - 1);
        }

        public T GetNext() {
            return GetElemByIndexIfNotLast(_currentIndex + 1);
        }

        public T GetFirst() {
            return GetElemByIndexIfNotFirst(0);
        }

        private void SetCurrentIfNeed() {
            if (_currentIndex == DEFAULT_INDEX) {
                GetCurrent();
            }
        }

        public T GetLast() {
            return GetElemByIndexIfNotLast(ElemsWithTranslations.Count - 1);
        }

        private T GetElemByIndexIfNotFirst(int index) {
            SetCurrentIfNeed();
            return _currentIndex > 0 ? ElemsWithTranslations[index] : null;
        }

        private T GetElemByIndexIfNotLast(int index) {
            SetCurrentIfNeed();
            return _currentIndex < ElemsWithTranslations.Count - 1 ? ElemsWithTranslations[index] : null;
        }
    }
}