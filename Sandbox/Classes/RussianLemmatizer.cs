using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SolarixLemmatizatorEngineNET;

namespace Sandbox.Classes {
    public class RussianLemmatizer {
        private IntPtr _hEngine = IntPtr.Zero;

        private void LoadIfNeed() {
            if (_hEngine != IntPtr.Zero) {
                return;
            }
            _hEngine = LemmatizatorEngine.sol_LoadLemmatizatorW(
                Path.Combine(Environment.CurrentDirectory, "lemmatizer.db"),
                LemmatizatorEngine
                    .LEME_DEFAULT);
        }

        public List<string> GetWordLemmas(string word) {
            try {
                LoadIfNeed();

                var result = new HashSet<string>();
                var buffer = new StringBuilder();
                LemmatizatorEngine.sol_GetLemmaW(_hEngine, word, buffer, buffer.Capacity);
                AddNormalWordToList(buffer, result);

                IntPtr hList = LemmatizatorEngine.sol_GetLemmasW(_hEngine, word);
                return AddLemmasToResult(hList, buffer, result);
            } catch (Exception e) {
                return null;
            }
        }

        public List<string> GetSentenceLemmas(string sentence, char separator = ' ') {
            try {
                LoadIfNeed();

                var result = new HashSet<string>();
                var buffer = new StringBuilder();
                IntPtr hList = LemmatizatorEngine.sol_LemmatizePhraseW(_hEngine, sentence, LemmatizatorEngine.LEME_DEFAULT, separator);
                return AddLemmasToResult(hList, buffer, result);
            } catch (Exception e) {
                return null;
            }
        }

        private static List<string> AddLemmasToResult(IntPtr hList, StringBuilder buffer, HashSet<string> result) {
            int countLemmas = LemmatizatorEngine.sol_CountLemmas(hList);
            for (int i = 0; i < countLemmas; i++) {
                LemmatizatorEngine.sol_GetLemmaStringW(hList, i, buffer, buffer.Capacity);
                AddNormalWordToList(buffer, result);
            }
           // LemmatizatorEngine.sol_DeleteLemmas(hList);
            return result.ToList();
        }

        private static void AddNormalWordToList(StringBuilder buffer, HashSet<string> result) {
            if (buffer.Length > 0) {
                result.Add(buffer.ToString().ToLowerInvariant());
            }
        }
    }
}