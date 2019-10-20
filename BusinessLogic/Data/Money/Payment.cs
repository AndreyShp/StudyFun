using System;
using BusinessLogic.Data.Enums.Money;

namespace BusinessLogic.Data.Money {
    /// <summary>
    /// ������ ������ �� �������
    /// </summary>
    public class Payment {
        /// <summary>
        /// ������������� �������
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// ����� ������
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// ������ ������
        /// </summary>
        public PaymentStatus Status { get; set; }
        /// <summary>
        /// �������� ������
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ������������� ������������(��� 0 ���� �� ���������������)
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// ���� � ����� �������� �������
        /// </summary>
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// ���� � ����� ������
        /// </summary>
        public DateTime PaymentDate { get; set; }
        /// <summary>
        /// ������� ������
        /// </summary>
        public PaymentSystem System { get; set; }
    }
}