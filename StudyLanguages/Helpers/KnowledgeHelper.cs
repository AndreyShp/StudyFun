using System;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Downloaders;

namespace StudyLanguages.Helpers {
    public class KnowledgeHelper {
        public static string GetHeader(KnowledgeDataType knowledgeDataType) {
            return GeneratedKnowledgeDownloader.GetHeader(knowledgeDataType);
        }

        public static SpeakerDataType GetSpeakerType(KnowledgeDataType knowledgeDataType) {
            if (knowledgeDataType == KnowledgeDataType.WordTranslation) {
                return SpeakerDataType.Word;
            }
            return SpeakerDataType.Sentence;
        }

        public static long ConvertDateTimeToJsTicks(DateTime dateTime) {
            return (long) (dateTime.ToUniversalTime()
                              .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                              .TotalMilliseconds);
        }
    }
}