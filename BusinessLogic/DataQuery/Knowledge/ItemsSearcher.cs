using System.Collections.Generic;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Knowledge {
    public class ItemsSearcher {
        private readonly HashSet<KnowledgeDataType> _types;
        private Dictionary<long, SourceWithTranslation> _sourceWithTranslations =
            new Dictionary<long, SourceWithTranslation>();

        public ItemsSearcher(params KnowledgeDataType[] types) {
            _types = new HashSet<KnowledgeDataType>(types);
            Ids = new List<long>();
        }

        public List<long> Ids { get; private set; }
        public bool HasIds {
            get { return EnumerableValidator.IsNotEmpty(Ids); }
        }

        public SourceWithTranslation Find(KnowledgeDataType dataType, long dataId) {
            if (!IsSupportType(dataType)) {
                return null;
            }
            SourceWithTranslation result;
            return _sourceWithTranslations.TryGetValue(dataId, out result) ? result : null;
        }

        private bool IsSupportType(KnowledgeDataType dataType) {
            return _types.Contains(dataType);
        }

        public void AddIfNeed(KnowledgeDataType dataType, long dataId) {
            if (IsSupportType(dataType)) {
                Ids.Add(dataId);
            }
        }

        public void Set(Dictionary<long, SourceWithTranslation> sourceWithTranslations) {
            _sourceWithTranslations = sourceWithTranslations;
        }
    }
}