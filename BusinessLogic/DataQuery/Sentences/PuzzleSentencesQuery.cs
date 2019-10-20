using BusinessLogic.Data.Enums;

namespace BusinessLogic.DataQuery.Sentences {
    public class PuzzleSentencesQuery : BaseQuery, IPuzzleSentencesQuery {
        private readonly long _languageId;

        public PuzzleSentencesQuery(long languageId) {
            _languageId = languageId;
        }

        public void GetByCount(PuzzleSentenceSource source) {
            
        }
    }
}