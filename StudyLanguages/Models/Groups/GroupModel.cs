using System.Collections.Generic;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using StudyLanguages.Helpers;

namespace StudyLanguages.Models.Groups {
    public class GroupModel : BaseSeriesModel<SourceWithTranslation> {
        private readonly GroupModelOptions _options;

        public GroupModel(GroupForUser group,
                          SpeakerDataType speakerDataType,
                          KnowledgeDataType knowledgeDataType,
                          GroupModelOptions options,
                          UserLanguages userLanguages,
                          List<SourceWithTranslation> groupElemsWithTranslations)
            : base(userLanguages, groupElemsWithTranslations) {
            SpeakerDataType = speakerDataType;
            KnowledgeDataType = knowledgeDataType;
            Id = group.Id;
            GroupName = group.Name;
            _options = options;
        }

        /// <summary>
        /// Что произносит слово или фразу
        /// </summary>
        public SpeakerDataType SpeakerDataType { get; private set; }

        /// <summary>
        /// Что добавлять на обучение слово или фразу
        /// </summary>
        public KnowledgeDataType KnowledgeDataType { get; private set; }

        /// <summary>
        /// Первая ссылка
        /// </summary>
        public LinkInfo GetFirstLink(string patternUrl) {
            SourceWithTranslation first = GetFirst();
            return _options.GetLink(patternUrl, LinkId.First, first);
        }

        /// <summary>
        /// Предыдущая ссылка
        /// </summary>
        public LinkInfo GetPrevLink(string patternUrl) {
            SourceWithTranslation item = GetPrev();
            return _options.GetLink(patternUrl, LinkId.Prev, item);
        }

        /// <summary>
        /// Следующая ссылка
        /// </summary>
        public LinkInfo GetNextLink(string patternUrl) {
            SourceWithTranslation next = GetNext();
            return _options.GetLink(patternUrl, LinkId.Next, next);
        }

        /// <summary>
        /// Последняя ссылка
        /// </summary>
        public LinkInfo GetLastLink(string patternUrl) {
            SourceWithTranslation last = GetLast();
            return _options.GetLink(patternUrl, LinkId.Last, last);
        }

        /// <summary>
        /// Имя группы
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// Есть ли картинка у текущего элемента
        /// </summary>
        public bool HasImage { get; private set; }

        /// <summary>
        /// Идентификатор текущего элемента
        /// </summary>
        public long Id { get; private set; }
        public CrossReferencesModel CrossReferencesModel { get; set; }

        public void SetCurrent(SourceWithTranslation cur) {
            SourceWithTranslation current = GetCurrent();
            if (current != null) {
                current.IsCurrent = false;
            }

            cur.IsCurrent = true;
            //TODO: исправить чтобы не ломалось
            //NOTE: не удалять иначе ломается:( 
            current = GetCurrent();
        }

        /// <summary>
        /// Корректные ли данные
        /// </summary>
        /// <returns>true - корректные, false - некорректные</returns>
        public bool IsInvalid() {
            return ElemsWithTranslations == null || EnumerableValidator.IsEmpty(ElemsWithTranslations);
        }
    }
}