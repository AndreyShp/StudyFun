using System;

namespace BusinessLogic.Data.Knowledge {
    public class UserKnowledge {
        public const int HASH_LENGTH = 32;

        public long Id { get; set; }
        public long UserId { get; set; }
        public long LanguageId { get; set; }
        /// <summary>
        /// ������ ������������
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// ������ �� �������� ������(��, ������ ������������ ���� ������)
        /// </summary>
        public long? DataId { get; set; }
        public int DataType { get; set; }
        public string Tip { get; set; }
        public int Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DeletedDate { get; set; }
        /// <summary>
        /// ��������� ������
        /// </summary>
        public string SystemData { get; set; }
        /// <summary>
        /// ���������� ��� ������
        /// </summary>
        public string Hash { get; set; }
    }
}