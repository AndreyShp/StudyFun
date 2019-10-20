namespace BusinessLogic.PaymentSystems {
    /// <summary>
    /// Пар-ры безопасности для Robokassa
    /// </summary>
    public class RobokassaSecurityParams {
        public RobokassaSecurityParams(string login, string password1, string password2) {
            Login = login;
            Password1 = password1;
            Password2 = password2;
        }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; private set; }
        /// <summary>
        /// Пароль для шифрования(публичный)
        /// </summary>
        public string Password1 { get; private set; }
        /// <summary>
        /// Пароль для шифрования(скрытый)
        /// </summary>
        public string Password2 { get; private set; }
    }
}