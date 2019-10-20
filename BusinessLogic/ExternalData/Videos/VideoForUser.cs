using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using BusinessLogic.Validators;

namespace BusinessLogic.ExternalData.Videos {
    /// <summary>
    /// ������ � ����� ��� ������������
    /// </summary>
    public class VideoForUser {
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="video">������ � �����</param>
        internal VideoForUser(Data.Video.Video video)
            : this(video.Title, video.HtmlCode) {
            Id = video.Id;
            HasImage = EnumerableValidator.IsNotNullAndNotEmpty(video.Image);
            SortInfo = new SortInfo(video);
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="title">�������� �����</param>
        /// <param name="htmlCode">��� ������� �����</param>
        public VideoForUser(string title, string htmlCode) {
            Title = title;
            HtmlCode = htmlCode;
            Sentences = new List<Tuple<string, string>>();
        }

        /// <summary>
        /// ������������� �����
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// �������� �����
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// ��� ��� ������� �����
        /// </summary>
        public string HtmlCode { get; set; }

        /// <summary>
        /// ���� �� ����������� � �����
        /// </summary>
        public bool HasImage { get; set; }

        /// <summary>
        /// ����������� ������
        /// </summary>
        public List<Tuple<string, string>> Sentences { get; private set; }

        /// <summary>
        /// ���� �� �����
        /// </summary>
        public bool HasSentences {
            get { return EnumerableValidator.IsNotEmpty(Sentences); }
        }

        /// <summary>
        /// ���� �� ������� ������
        /// </summary>
        public bool HasAnyTranslation {
            get { return Sentences.Any(HasTranslation); }
        }

        /// <summary>
        /// ���������� ��� ����������
        /// </summary>
        [ScriptIgnore]
        public SortInfo SortInfo { get; private set; }

        public bool HasTranslation(Tuple<string, string> sentences) {
            return !string.IsNullOrWhiteSpace(sentences.Item2);
        }
    }
}