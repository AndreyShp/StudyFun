using BusinessLogic;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace Sandbox.Classes.GroupFiller {
    internal class GroupSentenceCreator : BaseGroupCreator {
        public GroupSentenceCreator(LanguageShortName from) : base(from) {}

        protected override GroupType GroupType {
            get { return GroupType.BySentence; }
        }

        protected override bool Create(GroupForUser groupForUser,
                                       string[] line,
                                       Language english,
                                       Language russian,
                                       byte[] image,
                                       int? rating) {
            IGroupSentencesQuery groupWordsQuery = new GroupSentencesQuery();
            SourceWithTranslation sentenceWithTranslation =
                groupWordsQuery.GetOrCreate(groupForUser, CreateSentenceForUser(line[0], english),
                                            CreateSentenceForUser(line[1], russian), image, rating);
            bool isSuccess = sentenceWithTranslation != null;
            return isSuccess;
        }

        private static PronunciationForUser CreateSentenceForUser(string text, Language english) {
            return new PronunciationForUser(IdValidator.INVALID_ID, text, false, english.Id);
        }
    }
}