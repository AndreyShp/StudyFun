using System;

namespace BusinessLogic.Extensions {
    public static class DateTimeExtension {
        public static DateTime GetDbDateTime(this DateTime dateTime) {
            return new DateTime(2001, 01, 01);
        }
    }
}