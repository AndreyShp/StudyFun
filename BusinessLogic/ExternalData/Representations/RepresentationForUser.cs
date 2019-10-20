using System.Collections.Generic;
using System.Web.Script.Serialization;
using BusinessLogic.Data.Representation;

namespace BusinessLogic.ExternalData.Representations {
    public class RepresentationForUser {
        internal RepresentationForUser(Representation representation)
            : this(
                representation.Id, representation.Title, representation.Image,
                new Size(representation.Width, representation.Height), representation.WidthPercent) {
            SortInfo = new SortInfo(representation);
        }

        public RepresentationForUser(long id,
                                     string title,
                                     byte[] image,
                                     Size size,
                                     byte? widthPercent) {
            Id = id;
            Title = (title ?? string.Empty).Trim();
            Image = image;
            Areas = new List<RepresentationAreaForUser>();
            Size = size;
            WidthPercent = widthPercent;
        }

        public long Id { get; private set; }
        public string Title { get; private set; }
        public byte[] Image { get; private set; }
        public Size Size { get; private set; }
        public byte? WidthPercent { get; private set; }

        public List<RepresentationAreaForUser> Areas { get; private set; }

        /// <summary>
        /// Информация для сортировки
        /// </summary>
        [ScriptIgnore]
        public SortInfo SortInfo { get; private set; }

        public void AddArea(RepresentationAreaForUser area) {
            if (area != null) {
                Areas.Add(area);
            }
        }
    }
}