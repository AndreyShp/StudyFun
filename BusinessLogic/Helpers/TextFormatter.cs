namespace BusinessLogic.Helpers {
    public static class TextFormatter {
        public static string AppendCharIfNeed(string text, string end = ".") {
            return text.EndsWith(end) ? text : text + end;
        }

        //TODO: тесты
        public static string AddEndingByCount(long count, string[] words) {
            long lastDigit = count % 100;
            if (lastDigit >= 11 && lastDigit <= 14) {
                return words[0];
            }

            lastDigit = count % 10;
            if (count == 1) {
                return words[1];
            }

            if (lastDigit >= 2 && lastDigit <= 4) {
                return words[2];
            }

            return words[0];
        }
    }
}