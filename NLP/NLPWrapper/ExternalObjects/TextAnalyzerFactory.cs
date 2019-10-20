namespace NLPWrapper.ExternalObjects {
    public class TextAnalyzerFactory {
#pragma warning disable 649
        private static volatile ITextAnalyzer _instance;
#pragma warning restore 649
        private static volatile object _sync = new object();

        public static ITextAnalyzer Create() {
            if (_instance == null) {
                lock (_sync) {
                    if (_instance == null) {
                        //TODO: изменить пути
                        var nativeTextAnalyzer = new NativeTextAnalyzer(@"C:\Projects\StudyLanguages\NLP\Models\",
                                                                        @"C:\Projects\StudyLanguages\NLP\Dictionaries\");

                        _instance = new TextAnalyzer(nativeTextAnalyzer);
                    }
                }
            }
            return _instance;
        }
    }
}