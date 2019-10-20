using System;
using System.Collections.Generic;

namespace Sandbox.Classes {
    public static class LemmatizerHelper {
        public static void Run() {
            //Мы ели суп, а вдоль аллеи стояли раскидистые ели
            var russianLemmatizer = new RussianLemmatizer();
            do {
                Console.WriteLine("Введите слово для лемматизации:");
                string word = Console.ReadLine();
                if (string.IsNullOrEmpty(word)) {
                    break;
                }
                List<string> lemmas = word.Contains(" ") 
                    ? russianLemmatizer.GetSentenceLemmas(word)
                    : russianLemmatizer.GetWordLemmas(word);
                Console.WriteLine(string.Join(", ", lemmas));
            } while (true);
            Console.WriteLine("Лемматизация слов завершена!");
            Console.ReadLine();
            return;
        }
    }
}