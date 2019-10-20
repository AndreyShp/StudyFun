namespace BusinessLogic.Mailer {
    public class MailerAuthInfo {
        /// <summary>
        /// SMTP-сервер
        /// </summary>
        public string Smtp { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Порт
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Используется ли SSL протокол
        /// </summary>
        public bool EnableSsl { get; set; }
    }
}