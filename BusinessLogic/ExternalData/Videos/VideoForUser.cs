using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using BusinessLogic.Validators;

namespace BusinessLogic.ExternalData.Videos {
    /// <summary>
    /// Данные о видео для пользователя
    /// </summary>
    public class VideoForUser {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="video">данные о видео</param>
        internal VideoForUser(Data.Video.Video video)
            : this(video.Title, video.HtmlCode) {
            Id = video.Id;
            HasImage = EnumerableValidator.IsNotNullAndNotEmpty(video.Image);
            SortInfo = new SortInfo(video);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="title">название видео</param>
        /// <param name="htmlCode">код вставки видео</param>
        public VideoForUser(string title, string htmlCode) {
            Title = title;
            HtmlCode = htmlCode;
            Sentences = new List<Tuple<string, string>>();
        }

        /// <summary>
        /// Идентификатор видео
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Название видео
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код для вставки видео
        /// </summary>
        public string HtmlCode { get; set; }

        /// <summary>
        /// Есть ли изображение у видео
        /// </summary>
        public bool HasImage { get; set; }

        /// <summary>
        /// Предложения текста
        /// </summary>
        public List<Tuple<string, string>> Sentences { get; private set; }

        /// <summary>
        /// Есть ли текст
        /// </summary>
        public bool HasSentences {
            get { return EnumerableValidator.IsNotEmpty(Sentences); }
        }

        /// <summary>
        /// Есть ли перевод текста
        /// </summary>
        public bool HasAnyTranslation {
            get { return Sentences.Any(HasTranslation); }
        }

        /// <summary>
        /// Информация для сортировки
        /// </summary>
        [ScriptIgnore]
        public SortInfo SortInfo { get; private set; }

        public bool HasTranslation(Tuple<string, string> sentences) {
            return !string.IsNullOrWhiteSpace(sentences.Item2);
        }
    }
}