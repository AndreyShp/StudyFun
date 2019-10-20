using BusinessLogic.Validators;

namespace StudyLanguages.Models.Sales {
    internal class FileModel {
        public FileModel(string name, byte[] content) {
            Name = name;
            Content = content;
        }

        public string Name { get; private set; }
        public byte[] Content { get; private set; }

        public bool IsValid {
            get { return !string.IsNullOrEmpty(Name) || EnumerableValidator.IsNotNullAndNotEmpty(Content); }
        }

        public static bool IsInvalid(FileModel fileModel) {
            return fileModel == null || !fileModel.IsValid;
        }
    }
}