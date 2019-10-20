using BusinessLogic.PaymentSystems;
using StudyLanguages.Configs;

namespace StudyLanguages.Helpers {
    internal class ApiFactory {
        public static RobokassaApi GetRobokassaApi(IWebSettings webSettings) {
            return new RobokassaApi(webSettings.RobokassaSecurityParams);
        }
    }
}