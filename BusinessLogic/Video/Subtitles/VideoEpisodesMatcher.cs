using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.ExternalData.Videos;

namespace BusinessLogic.Video.Subtitles {
    public class VideoEpisodesMatcher {
        private const int NOT_FOUND_INDEX = -1;
        private static readonly HashSet<char> _fastEndSentenceChars = new HashSet<char>(new[] {'.', '!', '?', '…'});
        private static readonly char[] _endSentenceChars;

        private readonly List<Subtitle> _source;
        private readonly List<Subtitle> _translation;

        static VideoEpisodesMatcher() {
            _endSentenceChars = _fastEndSentenceChars.ToArray();
        }

        public VideoEpisodesMatcher(List<Subtitle> source, List<Subtitle> translation) {
            _source = source;
            _translation = translation;
        }

        public List<Tuple<Subtitle, Subtitle>> Match() {
            int minTranslationIndex = 0;

            var sourceTimeTo = 0d;
            var translationTimeTo = 0d;
            var result = new List<Tuple<Subtitle, Subtitle>>();
            foreach (Subtitle sourceEpisode in _source) {
                int translationIndex = GetAppropriateTranslationIndex(sourceEpisode, sourceTimeTo, translationTimeTo, minTranslationIndex);
                if (translationIndex == NOT_FOUND_INDEX) {
                    Console.WriteLine("Не найден перевод для:\r\n\r\n{0}", sourceEpisode.Text);
                    continue;
                }

                minTranslationIndex = translationIndex + 1;
                Subtitle translation = _translation[translationIndex];

                sourceTimeTo = sourceEpisode.TimeTo;
                translationTimeTo = translation.TimeTo;
                result.Add(new Tuple<Subtitle, Subtitle>(sourceEpisode, translation));
            }

            return result;
        }

        private static double GetTimeFromPrev(Subtitle subtitle, double timeTo) {
            return timeTo > 0 ? subtitle.TimeFrom - timeTo : timeTo;
        }

        public int GetAppropriateTranslationIndex(Subtitle sourceSubtitle, double sourcePrevTimeTo, double translationPrevTimeTo, int startIndex) {
            var sourceTimeFromPrev = GetTimeFromPrev(sourceSubtitle, sourcePrevTimeTo);
            char endCharSource = GetCharEndSentence(sourceSubtitle);
            int sourceCountSentences = GetCountSentences(sourceSubtitle);
            for (int i = startIndex; i < _translation.Count; i++) {
                Subtitle translation = _translation[i];

                var translationTimeFromPrev = GetTimeFromPrev(translation, translationPrevTimeTo);
                var timePrev = translationTimeFromPrev - sourceTimeFromPrev;

                char endCharTranslation = GetCharEndSentence(translation);
                int translationCountSentences = GetCountSentences(sourceSubtitle);
                if (translation.TimeFrom >= sourceSubtitle.TimeFrom/* && timePrev.TotalSeconds >= 0*/
                    && (endCharTranslation == endCharSource || sourceCountSentences == translationCountSentences)) {
                    return i;
                }
            }

            return NOT_FOUND_INDEX;
        }

        //CountSentences = text.Split(new[] { '.', '!', '?', '…' }, StringSplitOptions.RemoveEmptyEntries).Length - 1

        private static char GetCharEndSentence(Subtitle subtitle) {
            string text = subtitle.Text;
            char result = text[text.Length - 1];
            if (!_fastEndSentenceChars.Contains(result)) {
                result = '.';
            }
            return result;
        }

        private static int GetCountSentences(Subtitle subtitle) {
            string text = subtitle.Text;
            return text.Split(_endSentenceChars, StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}