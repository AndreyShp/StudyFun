using System;
using System.Collections.Generic;
using BusinessLogic.ExternalData;
using NUnit.Framework;
using StudyLanguages.Helpers.Trainer;
using StudyLanguages.Models.Groups;

namespace UnitTests.Web.Helpers.Trainer {
    [TestFixture]
    public class GapsTrainerHelperTest {
        private readonly GapsTrainerHelper _helper = new GapsTrainerHelper();

        private static SourceWithTranslation GetSource(string source, string translation = "translation bla-bla") {
            var result = new SourceWithTranslation();
            result.Set(84746, new PronunciationForUser(837, source, true, 31),
                       new PronunciationForUser(33, translation, false, 47));
            return result;
        }

        private void AssertConvertToItems(List<SourceWithTranslation> sourcesWithTranslation,
                                          List<List<Tuple<int, int>>> ranges) {
            List<GapsTrainerItem> items = _helper.ConvertToItems(sourcesWithTranslation);
            Assert.That(items.Count, Is.EqualTo(sourcesWithTranslation.Count));

            for (int i = 0; i < sourcesWithTranslation.Count; i++) {
                GapsTrainerItem item = items[i];
                SourceWithTranslation sourceWithTranslation = sourcesWithTranslation[i];

                Assert.That(item, Is.Not.Null);
                Assert.That(item.Id, Is.EqualTo(sourceWithTranslation.Id));
                Assert.That(item.Original, Is.EqualTo(sourceWithTranslation.Source));
                Assert.That(item.Translation, Is.EqualTo(sourceWithTranslation.Translation));
                string textWithGaps = item.TextForUser;
                Assert.That(textWithGaps, Is.Not.Null);

                string sourceText = string.Join(" ",
                                                sourceWithTranslation.Source.Text.Split(new[] {' '},
                                                                                        StringSplitOptions.
                                                                                            RemoveEmptyEntries));
                Assert.That(textWithGaps.Length, Is.EqualTo(sourceText.Length));
                string[] words = textWithGaps.Split(' ');
                List<Tuple<int, int>> wordRanges = ranges[i];
                for (int wordIndex = 0; wordIndex < words.Length; wordIndex++) {
                    string word = words[wordIndex];
                    Tuple<int, int> range = wordRanges[wordIndex];
                    AssertWord(word, range.Item1, range.Item2);
                }
            }
        }

        private static void AssertWord(string word, int min, int max) {
            Assert.That(!string.IsNullOrEmpty(word), Is.True);
            int countGaps = 0;
            foreach (char c in word) {
                if (c == GapsTrainerHelper.GAP_CHAR) {
                    countGaps++;
                }
            }

            Assert.That(countGaps >= min, Is.True, "слово " + word + ", ожидалось не менее " + min + ", а получили " + countGaps);
            Assert.That(countGaps <= max, Is.True, "слово " + word + ", ожидалось не более " + max + ", а получили " + countGaps);
        }

        [Test]
        public void ConvertToItems() {
            AssertConvertToItems(new List<SourceWithTranslation>(), new List<List<Tuple<int, int>>>());

            AssertConvertToItems(new List<SourceWithTranslation> {
                GetSource("привет"),
                GetSource("привет мозг!"),
                GetSource("hello!"),
                GetSource("hello world from Andrey!", "skjsjsj"),
            }, new List<List<Tuple<int, int>>> {
                //привет
                new List<Tuple<int, int>> {new Tuple<int, int>(2, 4)}, 
                //привет мозг!
                new List<Tuple<int, int>> {new Tuple<int, int>(2, 4), new Tuple<int, int>(1, 3),}, 
                //hello!
                new List<Tuple<int, int>> {new Tuple<int, int>(1, 4)}, 
                //hello world from Andrey!
                new List<Tuple<int, int>> {
                    new Tuple<int, int>(1, 4),
                    new Tuple<int, int>(1, 4),
                    new Tuple<int, int>(1, 3),
                    new Tuple<int, int>(2, 4),
                },
            });

            AssertConvertToItems(new List<SourceWithTranslation> {
                GetSource("смешанный текст, на нескольких    языках -- mixed text,  on the several languages",
                          "лывиаифлыри врлфыив")
            },
                                 new List<List<Tuple<int, int>>> {
                                     new List<Tuple<int, int>> {
                                         new Tuple<int, int>(2, 7), //смешанный
                                         new Tuple<int, int>(1, 4), //текст
                                         new Tuple<int, int>(1, 1), //на
                                         new Tuple<int, int>(3, 7), //нескольких
                                         new Tuple<int, int>(2, 4), //языках
                                         new Tuple<int, int>(0, 0), //--
                                         new Tuple<int, int>(1, 4), //mixed
                                         new Tuple<int, int>(1, 3), //text
                                         new Tuple<int, int>(1, 1), //on
                                         new Tuple<int, int>(1, 2), //the
                                         new Tuple<int, int>(2, 5), //several
                                         new Tuple<int, int>(2, 7), //languages
                                     }
                                 });
        }
    }
}