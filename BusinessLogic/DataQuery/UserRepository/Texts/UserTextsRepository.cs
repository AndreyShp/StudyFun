using System.Collections.Generic;
using BusinessLogic.DataQuery.NoSql;

namespace BusinessLogic.DataQuery.UserRepository.Texts {
    public class UserTextsRepository {
        private const string TEXT_TABLE = "Text";
        private readonly KeyValueRepository _repository;

        public UserTextsRepository(KeyValueRepository repository) {
            _repository = repository;
        }

        public bool WriteText(UserText userText) {
            if (string.IsNullOrEmpty(userText.Title)) {
                userText.Title = GenerateAppropriateTitle();
            }
            return _repository.Set(TEXT_TABLE, userText.Title, userText);
        }

        private string GenerateAppropriateTitle() {
            const string TITLE_PATTERN = "Без темы {0}";
            Dictionary<string, UserText> titles = GetUserTexts();
            int i = 1;
            do {
                string result = string.Format(TITLE_PATTERN, i);
                if (!titles.ContainsKey(result)) {
                    return result;
                }
                i++;
            } while (true);
        }

        public Dictionary<string, UserText> GetUserTexts() {
            return _repository.SelectAsDictionary<string, UserText>(TEXT_TABLE) ?? new Dictionary<string, UserText>(0);
        }

        public UserText GetUserText(string title) {
            return _repository.Select<string, UserText>(TEXT_TABLE, title, null);
        }

        public void MovePointer(string title, long pointer) {
            _repository.SyncSet<string, UserText>(TEXT_TABLE, title, userText => {
                if (pointer > userText.Pointer) {
                    userText.Pointer = pointer;
                }
            });
        }
    }
}