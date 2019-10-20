using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace Sandbox.Classes.GroupFiller {
    internal class GroupWordCreator : BaseGroupCreator {
        public GroupWordCreator(LanguageShortName from) : base(from) {}

        protected override GroupType GroupType {
            get { return GroupType.ByWord; }
        }

        protected override bool Create(GroupForUser groupForUser,
                                       string[] line,
                                       Language english,
                                       Language russian,
                                       byte[] image,
                                       int? rating) {
            IGroupWordsQuery groupWordsQuery = new GroupWordsQuery();
            SourceWithTranslation wordWithTranslation =
                groupWordsQuery.GetOrCreate(groupForUser, CreateWordForUser(line[0], english),
                                            CreateWordForUser(line[1], russian), image, rating);
            bool isSuccess = wordWithTranslation != null;
            return isSuccess;
        }

        private static PronunciationForUser CreateWordForUser(string text, Language language) {
            return new PronunciationForUser(IdValidator.INVALID_ID, text, false, language.Id);
        }
    }
}