using System;
using System.Text;
using BusinessLogic.Helpers;

namespace BusinessLogic {
    /// <summary>
    /// ���������������� �������������
    /// </summary>
    public class UserUniqueId {
        public const int USER_HASH_LEN = 96;
        
        /// <summary>
        /// ��������� ������������ ����������������� ��������������
        /// </summary>
        /// <param name="userUniqueId">������������� ������������</param>
        /// <returns>true - ������������� ����������, false - ������������� ������������</returns>
        public bool IsValid(string userUniqueId) {
            return IdGenerator.IsValid(userUniqueId, USER_HASH_LEN);
        }

        /// <summary>
        /// ���������� ���������� ������������� ������������
        /// </summary>
        /// <returns>���������� ������������� ������������</returns>
        public string New() {
            return IdGenerator.GenerateByLength(USER_HASH_LEN);
        }
    }
}