using System;

namespace BusinessLogic.Mailer {
    public class ExceptionsRegister {
        public static void Register(string domain, Exception ex) {
            if (ex == null) {
                return;
            }
            var mailer = new Mailer();
            mailer.SendMail(MailAddresses.EXCEPTIONS, MailAddresses.EXCEPTIONS, "Исключение на сайте " + domain, ex.ToString());
        }
    }
}