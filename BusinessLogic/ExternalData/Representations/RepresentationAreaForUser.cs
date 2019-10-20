using System.Web.Script.Serialization;
using BusinessLogic.Data.Representation;

namespace BusinessLogic.ExternalData.Representations {
    public class RepresentationAreaForUser : ISourceWithTranslation {
        internal RepresentationAreaForUser(RepresentationArea representationArea)
            : this(
                representationArea.Id, new Point(representationArea.LeftUpperX, representationArea.LeftUpperY),
                new Point(representationArea.RightBottomX, representationArea.RightBottomY)) {
            WordTranslationId = representationArea.WordTranslationId;
        }

        public RepresentationAreaForUser(long id, Point leftUpperCorner, Point rightBottomCorner) {
            Id = id;
            LeftUpperCorner = leftUpperCorner;
            RightBottomCorner = rightBottomCorner;
        }

        public Point LeftUpperCorner { get; private set; }
        public Point RightBottomCorner { get; private set; }
        [ScriptIgnore]
        public long WordTranslationId { get; private set; }

        #region ISourceWithTranslation Members

        public long Id { get; private set; }
        public PronunciationForUser Source { get; set; }
        public PronunciationForUser Translation { get; set; }

        #endregion
    }
}