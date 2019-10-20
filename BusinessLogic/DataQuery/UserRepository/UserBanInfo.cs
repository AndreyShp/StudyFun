using System;
using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery.UserRepository {
    public class UserBanInfo {
        public UserBanInfo() {
            BannedSections = new Dictionary<string, BanType>();
        }

        /// <summary>
        /// »дентификатор пользовател€
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Ip-адрес пользовател€
        /// </summary>
        public string UserIp { get; set; }

        /// <summary>
        /// Ѕраузер пользовател€
        /// </summary>
        public string BrowserName { get; set; }

        /// <summary>
        /// –азделы, которые забанены
        /// </summary>
        public Dictionary<string, BanType> BannedSections { get; set; }

        /// <summary>
        /// ƒата бана
        /// </summary>
        public DateTime BanDate { get; set; }

        /// <summary>
        /// Ѕанит раздел
        /// </summary>
        /// <param name="sectionId">раздел, который нужно забанить</param>
        /// <param name="banType">тип бана</param>
        public void SetBanSection(SectionId sectionId, BanType banType) {
            BanDate = DateTime.Now;
            string key = sectionId.ToString();
            if (!BannedSections.ContainsKey(key)) {
                BannedSections.Add(key, banType);
            } else {
                BannedSections[key] = banType;
            }
        }

        /// <summary>
        /// ќпредел€ет забанен ли раздел
        /// </summary>
        /// <param name="sectionId">раздел</param>
        /// <returns>true - раздел забанен, false - раздел незабанен</returns>
        public bool IsBannedSection(SectionId sectionId) {
            BanType banType;
            string key = sectionId.ToString();
            if (!BannedSections.TryGetValue(key, out banType)) {
                return false;
            }

            if (banType == BanType.Forever) {
                //забанен навсегда
                return true;
            }

            if (banType == BanType.Today && BanDate.Date == DateTime.Today) {
                //забанен на сегодн€
                return true;
            }
            return false;
        }
    }
}