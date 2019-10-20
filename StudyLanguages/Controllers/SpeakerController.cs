using System.Web.Mvc;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.Logger;
using BusinessLogic.Validators;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;

namespace StudyLanguages.Controllers {
    public class SpeakerController : Controller {
        //
        // GET: /Speaker/

        [Cache]
        [UserLanguages]
        public FileResult Speak(UserLanguages userLanguages, long id, SpeakerDataType type, bool mp3Support) {
            byte[] audioBytes = null;
            if (UserLanguages.IsValid(userLanguages) && IdValidator.IsValid(id)) {
                audioBytes = GetPronunciation(id, type);
            }
            if (audioBytes == null) {
                audioBytes = new byte[0];
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "SpeakerController.Speak. Для типа {0} с идентификатором {1} НЕ найдено произношение!",
                    type, id);
            }

            /*if (!mp3Support) {
                var audioConverter = new AudioConverter();
                byte[] result = audioConverter.ConvertMp3ToWav(audioBytes);
                if (result != null) {
                    return File(result, "audio/wav");
                }
            }*/

            return File(audioBytes, "audio/mp3");
        }

        private static byte[] GetPronunciation(long id, SpeakerDataType type) {
            IPronunciationQuery pronunciationQuery = null;
            if (type == SpeakerDataType.Word) {
                pronunciationQuery = new WordsQuery();
            } else if (type == SpeakerDataType.Sentence) {
                pronunciationQuery = new SentencesQuery();
            }
            if (pronunciationQuery == null) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "SpeakerController.GetPronunciation передан некорректный тип {0}", type);
                return null;
            }
            IPronunciation pronunciationEntity = pronunciationQuery.GetById(id);
            return pronunciationEntity != null ? pronunciationEntity.Pronunciation : null;
        }
    }
}