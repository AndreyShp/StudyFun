using System;

namespace BusinessLogic.Data {
    public class User {
        /// <summary>
        /// Максимальная длина электронного адреса (см. http://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address)
        /// </summary>
        public const int EMAIL_LENGTH = 254;

        public long Id { get; set; }
        public string UniqueHash { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime CreationDate { get; set; }
        public string LastIp { get; set; }
        public string CreationIp { get; set; }
        public string Email { get; set; }
    }
}