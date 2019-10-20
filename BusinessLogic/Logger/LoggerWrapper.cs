using System;
using System.Collections.Generic;
using BusinessLogic.Mailer;
using NLog;

namespace BusinessLogic.Logger {
    public class LoggerWrapper {
        private static readonly LoggerQuery _loggerQuery = new LoggerQuery();

        private static readonly Dictionary<LoggerName, string> _loggerNames = new Dictionary<LoggerName, string> {
            {LoggerName.Default, "Default"},
            {LoggerName.Errors, "Errors"}
        };

        private readonly NLog.Logger _logger;

        private LoggerWrapper(NLog.Logger logger) {
            _logger = logger;
        }

        public static LoggerWrapper LogTo(LoggerName loggerName) {
            string logName;
            if (!_loggerNames.TryGetValue(loggerName, out logName)) {
                logName = _loggerNames[LoggerName.Default];
            }

            NLog.Logger logger = LogManager.GetLogger(logName);
            return new LoggerWrapper(logger);
        }

        public void DebugFormat(string message, params object[] args) {
            _logger.Debug(message, args);
        }

        public void InfoFormat(string message, params object[] args) {
            _logger.Info(message, args);
        }

        public void WarnFormat(string message, params object[] args) {
            _logger.Warn(message, args);
        }

        public void WarnException(string message, Exception exception, params object[] args) {
            _logger.WarnException(Format(message, args), exception);
        }

        public void ErrorFormat(string message, params object[] args) {
            _logger.Error(message, args);
        }

        public void ErrorException(string message, Exception exception, params object[] args) {
            _logger.ErrorException(Format(message, args), exception);
        }

        private static string Format(string message, params object[] args) {
            return string.Format(message, args);
        }

        public static void RemoteMessage(LoggingType loggingType, string message, params object[] args) {
            RemoteMessage(LoggingSource.Mail, loggingType, message, args);
        }

        public static void RemoteMessage(LoggingSource loggingSource,
                                         LoggingType loggingType,
                                         string message,
                                         params object[] args) {
            string formattedMessage = string.Format(message, args);
            bool isSuccess;
            if ((loggingSource & LoggingSource.Mail) == LoggingSource.Mail) {
                string subject = loggingType + " логирование с сайта";
                var mailerConfig = new MailerConfig {
                    IsHtmlBody = false,
                    DisplayName = subject,
                    SendToAdmins = true
                };
                
                var mailer = new Mailer.Mailer();
                string mailAddress = loggingType.HasFlag(LoggingType.Error) ? MailAddresses.EXCEPTIONS : MailAddresses.SUPPORT;
                isSuccess = mailer.SendMail(mailAddress, mailAddress, subject, formattedMessage, mailerConfig);

                if (!isSuccess) {
                    LogTo(LoggerName.Errors).ErrorFormat(
                        "LoggerWrapper.RemoteMessage не удалось отправить сообщение по почте: {0}",
                        formattedMessage);
                }
            }

            /*if ((loggingSource & LoggingSource.Db) == LoggingSource.Db) {
                isSuccess = _loggerQuery.Create(loggingType, formattedMessage);

                if (!isSuccess) {
                    LogTo(LoggerName.Errors).ErrorFormat(
                        "LoggerWrapper.RemoteMessage не удалось записать сообщение в БД: {0}",
                        formattedMessage);
                }
            }*/
        }
    }
}