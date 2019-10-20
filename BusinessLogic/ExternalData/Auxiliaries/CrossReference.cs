using BusinessLogic.Data.Enums;

namespace BusinessLogic.ExternalData.Auxiliaries {
    public class CrossReference {
        public CrossReference(long id, string referenceName, CrossReferenceType type) {
            Id = id;
            ReferenceName = referenceName;
            Type = type;
        }

        public long Id { get; private set; }
        public string ReferenceName { get; private set; }
        public CrossReferenceType Type { get; private set; }
    }
}