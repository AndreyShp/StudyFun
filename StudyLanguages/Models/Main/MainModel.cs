using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Models.Main {
    public class MainModel {
        private readonly Dictionary<SectionId, DescriptionSection> _sections =
            new Dictionary<SectionId, DescriptionSection>();

        public MainModel() {
            Items = new List<SectionId>();
        }

        public List<SectionId> Items { get; private set; }

        public void Add(SectionId sectionId, DescriptionSection section) {
            _sections.Add(sectionId, section);
            Items.Add(sectionId);
        }

        public DescriptionSection Get(SectionId sectionId) {
            DescriptionSection result;
            return _sections.TryGetValue(sectionId, out result)
                       ? result
                       : null;
        }
    }
}