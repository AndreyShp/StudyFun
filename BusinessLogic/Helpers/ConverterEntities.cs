using System;
using BusinessLogic.Data;
using BusinessLogic.Data.Word;
using BusinessLogic.ExternalData;

namespace BusinessLogic.Helpers {
    internal static class ConverterEntities {
        /// <summary>
        /// Из двух энтитей создает энтити с переводом
        /// </summary>
        /// <param name="id">уникальный идентификатор энтити с переводом</param>
        /// <param name="image">изображение, если есть</param>
        /// <param name="sourceLanguageId">язык, с которого нужно переводить</param>
        /// <param name="pronunciation1">первая энтити</param>
        /// <param name="pronunciation2">втора энтити</param>
        /// <returns>энтити с переводом</returns>
        public static SourceWithTranslation ConvertToSourceWithTranslation(long id,
                                                                           byte[] image,
                                                                           long sourceLanguageId,
                                                                           PronunciationEntity pronunciation1,
                                                                           PronunciationEntity pronunciation2) {
            var result = new SourceWithTranslation();
            PronunciationEntity source;
            PronunciationEntity translation;
            if (pronunciation1.LanguageId == sourceLanguageId) {
                source = pronunciation1;
                translation = pronunciation2;
            } else {
                source = pronunciation2;
                translation = pronunciation1;
            }
            result.Set(id, source, translation);
            result.HasImage = image != null && image.Length > 0;
            return result;
        }

        public static SourceWithTranslation ConvertToSourceWithTranslation(long id,
                                                                           byte[] image,
                                                                           PronunciationForUser pronunciation1,
                                                                           PronunciationForUser pronunciation2) {
            var result = new SourceWithTranslation();
            result.Set(id, pronunciation1, pronunciation2);
            result.HasImage = image != null && image.Length > 0;
            return result;
        }

        /// <summary>
        /// Из двух слов делает, создает слово с переводом
        /// </summary>
        /// <param name="id">уникальный идентификатор слова с переводом</param>
        /// <param name="image">изображение для слова с переводом</param>
        /// <param name="sourceLanguageId">язык, с которого нужно переводить</param>
        /// <param name="word1">первое слово</param>
        /// <param name="word2">второе слово</param>
        /// <returns>слово с переводом</returns>
        public static SourceWithTranslation ConvertToSourceWithTranslation(long id,
                                                                           byte[] image,
                                                                           long sourceLanguageId,
                                                                           Word word1,
                                                                           Word word2) {
            Tuple<PronunciationForUser, PronunciationForUser> tuple = GroupingHelper.GroupByLanguages(sourceLanguageId,
                                                                                                      word1, word2);
            return ConvertToSourceWithTranslation(id, image, tuple.Item1, tuple.Item2);
        }
    }
}