using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using BusinessLogic.Logger;

namespace BusinessLogic.Mailer {
    public class Mailer {
        private readonly MailerAuthInfo _authInfo;

        public Mailer(MailerAuthInfo authInfo = null) {
            _authInfo = authInfo;
        }

        public bool SendMailToCopyAdmins(string from,
                                         string to,
                                         string subject,
                                         string body) {
            MailerConfig mailerConfig = CreateDefaultMailerConfig();
            mailerConfig.SendToAdmins = true;
            return SendMail(from, to, subject, body, mailerConfig);
        }

        public bool SendMail(string from, string to, string subject, string body, MailerConfig mailerConfig = null) {
            MailMessage message = null;

            if (mailerConfig == null) {
                mailerConfig = CreateDefaultMailerConfig();
            }

            try {
                var client = new SmtpClient(_authInfo.Smtp, _authInfo.Port);
                client.EnableSsl = _authInfo.EnableSsl;
                client.Credentials = new NetworkCredential(_authInfo.UserName ?? from,
                                                           _authInfo.Password);
                message = new MailMessage();
                message.From = new MailAddress(from, mailerConfig.DisplayName, Encoding.UTF8);
                string[] addressesTo = to.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string address in addressesTo) {
                    message.To.Add(new MailAddress(address));
                }
                if (mailerConfig.SendToAdmins && !addressesTo.Contains(MailAddresses.ADMIN)) {
                    message.Bcc.Add(new MailAddress(MailAddresses.ADMIN));
                }

                message.Body = body;
                message.BodyEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;
                message.Subject = subject;
                message.IsBodyHtml = mailerConfig.IsHtmlBody;

                client.Send(message);
                return true;
            } catch (Exception ex) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "При попытке отправить письмо {0}:{1} пользователям {2} вылетело исключение {3}", subject, body, to,
                    ex);
                if (message != null) {
                    message.Dispose();
                }
                return false;
            }
        }

        private static MailerConfig CreateDefaultMailerConfig() {
            return new MailerConfig {
                DisplayName = "Сайт studyfun.ru",
                IsHtmlBody = false,
                SendToAdmins = false
            };
        }
    }
}