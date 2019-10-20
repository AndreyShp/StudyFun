using System;
using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery.UserRepository {
    public class UserBanInfo {
        public UserBanInfo() {
            BannedSections = new Dictionary<SectionId, BanType>();
        }

        /// <summary>
        /// ������������� ������������
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Ip-����� ������������
        /// </summary>
        public string UserIp { get; set; }

        /// <summary>
        /// ������� ������������
        /// </summary>
        public string BrowserName { get; set; }

        /// <summary>
        /// �������, ������� ��������
        /// </summary>
        public Dictionary<SectionId, BanType> BannedSections { get; set; }

        /// <summary>
        /// ���� ����
        /// </summary>
        public DateTime BanDate { get; set; }

        /// <summary>
        /// ����� ������
        /// </summary>
        /// <param name="sectionId">������, ������� ����� ��������</param>
        /// <param name="banType">��� ����</param>
        public void SetBanSection(SectionId sectionId, BanType banType) {
            BanDate = DateTime.Now;
            if (!BannedSections.ContainsKey(sectionId)) {
                BannedSections.Add(sectionId, banType);
            } else {
                BannedSections[sectionId] = banType;
            }
        }

        /// <summary>
        /// ���������� ������� �� ������
        /// </summary>
        /// <param name="sectionId">������</param>
        /// <returns>true - ������ �������, false - ������ ���������</returns>
        public bool IsBannedSection(SectionId sectionId) {
            BanType banType;
            if (!BannedSections.TryGetValue(sectionId, out banType)) {
                return false;
            }

            if (banType == BanType.Forever) {
                //������� ��������
                return true;
            }

            if (banType == BanType.Today && BanDate.Date == DateTime.Today) {
                //������� �� �������
                return true;
            }
            return false;
        }
    }
}