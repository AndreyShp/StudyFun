using System;
using System.Collections.Generic;
using System.Globalization;
using BusinessLogic.DataQuery.NoSql;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.UserRepository.Tasks {
    public class BanRepository {
        private const string USER_BAN_INFO_TABLE = "BanInfo";
        private const string USER_EVENT_INFO_TABLE = "EventInfo";
        private readonly IRepositoryCache _repositoryCache;

        public BanRepository(IRepositoryCache repositoryCache) {
            _repositoryCache = repositoryCache;
        }

        public bool IsUserBanned(SectionId sectionId, long userId, string userIp, string browserName) {
            KeyValueRepository commonRepository = _repositoryCache.GetCommonRepository();

            UserBanInfo existingBanInfo;
            if (IdValidator.IsValid(userId)) {
                string userKey = GetUserKey(userId);
                existingBanInfo = GetUserBanInfo(commonRepository, userKey);
                if (existingBanInfo != null && existingBanInfo.IsBannedSection(sectionId)) {
                    return true;
                }
            }

            string anonymousKey = GetAnonymousUserKey(userIp, browserName);
            existingBanInfo = GetUserBanInfo(commonRepository, anonymousKey);
            if (existingBanInfo != null && existingBanInfo.IsBannedSection(sectionId)) {
                return true;
            }

            return false;
        }

        public bool AddBan(SectionId sectionId, BanType banType, long userId, string userIp, string browserName) {
            KeyValueRepository commonRepository = _repositoryCache.GetCommonRepository();

            var newUserBanInfo = new UserBanInfo {UserId = userId, UserIp = userIp, BrowserName = browserName};
            string userKey = GetUserKey(userId);
            bool result = SetUserBanInfo(commonRepository, userKey,
                                         userBanInfo => userBanInfo.SetBanSection(sectionId, banType), newUserBanInfo);
            if (!result) {
                return false;
            }

            string anonymousKey = GetAnonymousUserKey(userIp, browserName);
            result = SetUserBanInfo(commonRepository, anonymousKey,
                                    userBanInfo => userBanInfo.SetBanSection(sectionId, banType), newUserBanInfo);
            return result;
        }

        public bool RegisterEvent(SectionId sectionId, string @event, long userId, string userIp, string browserName) {
            KeyValueRepository commonRepository = _repositoryCache.GetCommonRepository();

            string fullEvent = string.Format("{0}_{1}", sectionId, @event);
            if (IdValidator.IsValid(userId)) {
                string userKey = GetUserKey(userId);
                if (!SetEventInfo(commonRepository, userKey, fullEvent)) {
                    return false;
                }
            }

            string anonymousKey = GetAnonymousUserKey(userIp, browserName);
            var result = SetEventInfo(commonRepository, anonymousKey, fullEvent);
            return result;
        }

        private static void Register(string @event, Dictionary<string, Dictionary<string, long>> userEvents) {
            string today = DateTime.Today.ToString("dd.MM.yy");
            Dictionary<string, long> eventsByDate;
            if (!userEvents.TryGetValue(today, out eventsByDate)) {
                eventsByDate = new Dictionary<string, long>();
                userEvents.Add(today, eventsByDate);
            }

            if (!eventsByDate.ContainsKey(@event)) {
                eventsByDate.Add(@event, 1);
            } else {
                eventsByDate[@event]++;
            }
        }

        private static string GetUserKey(long userId) {
            return "UserId_" + userId.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetAnonymousUserKey(string userIp, string browserName) {
            return userIp + "_" + browserName;
        }

        private static UserBanInfo GetUserBanInfo(KeyValueRepository commonRepository, string key) {
            UserBanInfo result = commonRepository.Select<string, UserBanInfo>(USER_BAN_INFO_TABLE, key, null);
            return result;
        }

        private static bool SetUserBanInfo(KeyValueRepository commonRepository,
                                           string key,
                                           Action<UserBanInfo> editor,
                                           UserBanInfo userBanInfo) {
            return commonRepository.SyncSet(USER_BAN_INFO_TABLE, key, editor, () => userBanInfo);
        }

        private static bool SetEventInfo(KeyValueRepository commonRepository,
                                         string key,
                                         string @event) {
            return commonRepository.SyncSet(USER_EVENT_INFO_TABLE, key, events => Register(@event, events),
                                            () => {
                                                var events = new Dictionary<string, Dictionary<string, long>>();
                                                Register(@event, events);
                                                return events;
                                            });
        }
    }
}