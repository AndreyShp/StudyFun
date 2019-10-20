using System.Web;
using BusinessLogic.DataQuery.UserRepository.Tasks;
using BusinessLogic.ExternalData;
using BusinessLogic.Logger;

namespace StudyLanguages.Helpers {
    public class BanHelper {
        private readonly string _browserName;
        private readonly string _userIp;

        public BanHelper(HttpRequestBase request) {
            _userIp = RemoteClientHelper.GetClientIpAddress(request);
            _browserName = RemoteClientHelper.GetBrowserName(request);
        }

        public bool IsBanned(SectionId sectionId, long userId, BanRepository banRepository) {
            bool isBanned = banRepository.IsUserBanned(sectionId, userId, _userIp, _browserName);
            return isBanned;
        }

        public void RegisterEvent(SectionId sectionId, string @event, long userId, BanRepository banRepository) {
            bool result = banRepository.RegisterEvent(sectionId, @event, userId, _userIp, _browserName);
            if (!result) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "BanHelper.RegisterEvent НЕ УДАЛОСЬ зарегистрировать событие {0} для секции {1} для пользователя {2}, {3}, {4}!",
                    sectionId, @event, userId, _userIp, _browserName);
            }
        }
    }
}