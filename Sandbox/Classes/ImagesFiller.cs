using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic;
using BusinessLogic.Data;
using BusinessLogic.Data.Representation;

namespace Sandbox.Classes {
    public class ImagesFiller {
        public static void FillRepresentation() {
            var adapter = new DbAdapter();
            adapter.ActionByContext(context => {
                IEnumerable<Representation> entities = context.Representation.AsEnumerable();
                foreach (Representation entity in entities) {
                    string fullFileName = @"C:\Projects\StudyLanguages\Источники визуального словаря\Картинки\"
                                          + entity.Title
                                          + ".jpg";
                    if (File.Exists(fullFileName)) {
                        byte[] image = File.ReadAllBytes(fullFileName);
                        //обновить возможно изменившийся рейтинг и изображение
                        entity.Image = image;
                        context.SaveChanges();
                    } else {
                        Console.WriteLine(
                            "Не найдена картинка для визуального словаря {0}! Нажмите Enter для продолжения...",
                            entity.Title);
                        Console.ReadLine();
                    }
                }
            });
        }

        public static void FillGroups() {
            var adapter = new DbAdapter();
            adapter.ActionByContext(context => {
                IEnumerable<Group> entities = context.Group.AsEnumerable();
                foreach (Group entity in entities) {
                    string fullFileName = @"C:\Projects\StudyLanguages\Источники для групп\Источники картинок\"
                                          + entity.Name
                                          + ".jpg";
                    byte[] image = null;
                    if (File.Exists(fullFileName)) {
                        image = File.ReadAllBytes(fullFileName);
                    } else {
                        Console.WriteLine("Не найдена картинка для группы {0}! Нажмите Enter для продолжения...",
                                          entity.Name);
                        Console.ReadLine();
                    }
                    entity.Image = image;
                    context.SaveChanges();
                }
            });
        }
    }
}