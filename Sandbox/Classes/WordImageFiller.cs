using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Word;
using BusinessLogic.DataQuery;
using BusinessLogic.ExternalData;

namespace Sandbox.Classes {
    public class WordImageFiller {
        public static void Fill(string groupName, string folder) {
            var languages = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            Language english = languages.GetByShortName(LanguageShortName.En);
            Language russian = languages.GetByShortName(LanguageShortName.Ru);
            //первую строку пропускаем

            string imagePath = Path.Combine(folder, groupName);
            List<string> imageFiles = Directory.GetFiles(imagePath, "*.*")
                .Where(file => file.ToLower().EndsWith("jpg") || file.ToLower().EndsWith("jpeg"))
                .ToList();
            new DbAdapter().ActionByContext(c => {
                int i = 1;
                foreach (string imageFile in imageFiles) {
                    string englishWord = Path.GetFileNameWithoutExtension(imageFile);
                    ImageConverter.ResizeSmallImages(imageFile);

                    Image image = Image.FromFile(imageFile);
                    var memoryStream = new MemoryStream();
                    image.Save(memoryStream, ImageFormat.Jpeg);
                    byte[] imageBytes = memoryStream.ToArray();

                    IQueryable<WordTranslation> wordTranslations = (from wt in c.WordTranslation
                                                                    join w1 in c.Word on wt.WordId1 equals w1.Id
                                                                    join w2 in c.Word on wt.WordId2 equals w2.Id
                                                                    join gw in c.GroupWord on wt.Id equals
                                                                        gw.WordTranslationId
                                                                    join g in c.Group on gw.GroupId equals g.Id
                                                                    where
                                                                        g.Name == groupName &&
                                                                        ((w1.Text == englishWord
                                                                          && w1.LanguageId == english.Id)
                                                                         || (w2.Text == englishWord
                                                                             && w2.LanguageId == english.Id))
                                                                    select wt);
                    List<WordTranslation> wordsTranslation = wordTranslations.ToList();
                    foreach (WordTranslation wordTranslation in wordsTranslation) {
                        wordTranslation.Image = imageBytes;
                        c.SaveChanges();
                        Console.WriteLine("{0}. {1} сохранено", i, englishWord);
                    }
                    i++;
                }
                ;
            });
        }
    }
}