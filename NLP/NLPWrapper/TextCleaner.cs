using System.Text.RegularExpressions;

namespace NLPWrapper {
    internal class TextCleaner {
        private const string REPLACEMENT = " ";
        private readonly Regex _htmlTagsCleaner = new Regex(@"<[^>]*>",
                                                            RegexOptions.Singleline | RegexOptions.Compiled
                                                            | RegexOptions.IgnoreCase);
        private readonly Regex _spaceCleaner = new Regex("\\s+",
                                                         RegexOptions.Singleline | RegexOptions.Compiled
                                                         | RegexOptions.IgnoreCase);

        public string Clear(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return text;
            }

            string result = text.Replace("\\r\\n", REPLACEMENT);
            result = result.Replace("\\n", REPLACEMENT);
            result = result.Replace("\\t", REPLACEMENT);
            result = _htmlTagsCleaner.Replace(result, REPLACEMENT);
            result = _spaceCleaner.Replace(result, REPLACEMENT);
            return result;
        }
    }
}