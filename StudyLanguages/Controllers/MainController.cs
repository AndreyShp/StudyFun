using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Comparisons;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Comparisons;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.ExternalData.Videos;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Models.Main;
using StudyLanguages.Models.Sales;

namespace StudyLanguages.Controllers {
    public class MainController : BaseController {
        [UserLanguages]
        public ActionResult Index(UserLanguages userLanguages) {
            const int COUNT = 5;

            if (UserLanguages.IsInvalid(userLanguages)) {
                return RedirectToAction("Unknown", "Errors");
            }

            var model = new MainModel();
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();

            //слова по темам
            AddSectionIfNeed(SectionId.GroupByWords, model, () => {
                IGroupsQuery groupsQuery = new GroupsQuery(languageId);
                List<GroupForUser> groups = groupsQuery.GetVisibleGroups(GroupType.ByWord, COUNT);
                return new DescriptionSection {
                    Title = new DescriptionTitle {
                        Title = "Слова по темам",
                        Url = Url.Action("Index", RouteConfig.GROUPS_BY_WORDS_CONTROLLER, null, Request.Url.Scheme),
                        Description = "Перейти ко всем темам"
                    },
                    Description =
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.GroupByWordsDescription),
                    MostPopularTitle = "5 наиболее популярных тем:",
                    Items = ConvertToItems(groups, e => e.Name),
                    TitleItemGetter =
                        item =>
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.GroupByWords, PageId.Index,
                                                                   TemplateId.ItemTipOnMainPage, item.Title),
                    UrlItemGetter =
                        item =>
                        @Url.Action("Index", RouteConfig.GROUP_WORD_CONTROLLER, new {group = item.Title + "/"},
                                    Request.Url.Scheme),
                    UrlImageItemGetter =
                        item =>
                        Url.Action("GetImageByName", RouteConfig.GROUP_WORD_CONTROLLER, new {group = item.Title},
                                   Request.Url.Scheme),
                    //Btn = BtnModel.CreateBuyAllMaterials(Url, "btn btn-danger btn-xs buy-btn-main")
                };
            });

            //почувствуй разницу
            AddSectionIfNeed(SectionId.FillDifference, model, () => {
                IComparisonsQuery comparisonsQuery = new ComparisonsQuery(languageId);
                List<ComparisonForUser> comparisons = comparisonsQuery.GetVisibleWithoutRules(COUNT);
                return new DescriptionSection {
                    Title = new DescriptionTitle {
                        Title = "Почувствуй разницу",
                        Url =
                            Url.Action("Index", RouteConfig.GROUPS_BY_COMPARISONS_CONTROLLER, null,
                                       Request.Url.Scheme),
                        Description = "Перейти ко всем правилам употребления"
                    },
                    Description =
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.FillDifferenceDescription),
                    MostPopularTitle = "5 наиболее популярных тем сравнения:",
                    Items = ConvertToItems(comparisons, e => e.Title),
                    TitleItemGetter =
                        item =>
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.FillDifference, PageId.Index,
                                                                   TemplateId.ItemTipOnMainPage, item.Title),
                    UrlItemGetter =
                        item =>
                        Url.Action("Index", RouteConfig.COMPARISON_CONTROLLER, new {group = item.Title + "/"},
                                   Request.Url.Scheme)
                };
            });

            //минилекс слов
            AddSectionIfNeed(SectionId.PopularWord, model, () => {
                return new DescriptionSection {
                    Title = new DescriptionTitle {
                        Title = "Минилекс слов",
                        Url =
                            Url.Action("Index", RouteConfig.POPULAR_WORDS_CONTROLLER, null, Request.Url.Scheme),
                        Description = "Перейти к минилексу слов Гуннемарка"
                    },
                    Description =
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.PopularWordDescription)
                };
            });

            //визуальные словари
            AddSectionIfNeed(SectionId.VisualDictionary, model, () => {
                IRepresentationsQuery representationsQuery = new RepresentationsQuery(languageId);
                List<RepresentationForUser> visibleDictionaries = representationsQuery.GetVisibleWithoutAreas(COUNT);
                return new DescriptionSection {
                    Title = new DescriptionTitle {
                        Title = "Визуальные словари",
                        Url =
                            Url.Action("Index", RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME, null,
                                       Request.Url.Scheme),
                        Description = "Перейти ко всем визуальным словарям"
                    },
                    Description =
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.Main,
                                                                   TemplateId.VisualDictionaryDescription),
                    MostPopularTitle = "5 наиболее популярных визуальных словарей:",
                    Items = ConvertToItems(visibleDictionaries, e => e.Title),
                    TitleItemGetter =
                        item =>
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.VisualDictionary, PageId.Index,
                                                                   TemplateId.ItemTipOnMainPage, item.Title),
                    UrlItemGetter =
                        item =>
                        Url.Action("Index", RouteConfig.VISUAL_DICTIONARY_CONTROLLER, new {group = item.Title + "/"},
                                   Request.Url.Scheme),
                    UrlImageItemGetter =
                        item =>
                        Url.Action("GetImageByName", RouteConfig.VISUAL_DICTIONARY_CONTROLLER, new {group = item.Title},
                                   Request.Url.Scheme),
                    //Btn = BtnModel.CreateBuyVisualDictionaries(Url, "btn btn-danger btn-xs buy-btn-main")
                };
            });

            AddSectionIfNeed(SectionId.GroupByPhrases, model, () => {
                //фразы по темам
                IGroupsQuery groupsQuery = new GroupsQuery(languageId);
                List<GroupForUser> groups = groupsQuery.GetVisibleGroups(GroupType.BySentence, COUNT);
                return new DescriptionSection {
                    Title = new DescriptionTitle {
                        Title = "Фразы по темам",
                        Url = Url.Action("Index", RouteConfig.GROUPS_BY_SENTENCES_CONTROLLER, null, Request.Url.Scheme),
                        Description = "Перейти ко всем темам"
                    },
                    Description =
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.GroupByPhrasesDescription),
                    MostPopularTitle = "5 наиболее популярных тем:",
                    Items = ConvertToItems(groups, e => e.Name),
                    TitleItemGetter =
                        item =>
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.GroupByPhrases, PageId.Index,
                                                                   TemplateId.ItemTipOnMainPage, item.Title),
                    UrlItemGetter =
                        item =>
                        @Url.Action("Index", RouteConfig.GROUP_SENTENCE_CONTROLLER, new {group = item.Title + "/"},
                                    Request.Url.Scheme),
                    UrlImageItemGetter =
                        item =>
                        Url.Action("GetImageByName", RouteConfig.GROUP_SENTENCE_CONTROLLER, new {group = item.Title},
                                   Request.Url.Scheme),
                };
            });

            //стена знания
            AddSectionIfNeed(SectionId.MyKnowledge, model, () => new DescriptionSection {
                Title = new DescriptionTitle {
                    Title = "Стена знаний",
                    Url = Url.Action("Index", RouteConfig.USER_KNOWLEDGE_CONTROLLER, null, Request.Url.Scheme),
                    Description = "Перейти к стене с Вашими знаниями"
                },
                Description =
                    WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.MyKnowledgeDescription),
                Items = null,
            });

            //генератор знаний
            AddSectionIfNeed(SectionId.KnowledgeGenerator, model, () => new DescriptionSection {
                Title = new DescriptionTitle {
                    Title = "Генератор знаний",
                    Url = Url.Action("Index", RouteConfig.KNOWLEDGE_GENERATOR_CONTROLLER, null, Request.Url.Scheme),
                    Description = "Перейти к генератору знаний"
                },
                Description =
                    WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.KnowledgeGeneratorDescription),
                Items = null,
            });

            //видео
            AddSectionIfNeed(SectionId.Video, model, () => {
                IVideosQuery videosQuery = new VideosQuery(languageId);
                List<VideoForUser> videos = videosQuery.GetVisible(VideoType.Clip, COUNT);
                return new DescriptionSection {
                    Title = new DescriptionTitle {
                        Title = "Видеоролики",
                        Url = Url.Action("Index", RouteConfig.VIDEO_CONTROLLER, new { type = VideoType.Clip }, Request.Url.Scheme),
                        Description = "Перейти ко всем видеороликам"
                    },
                    Description =
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.VideoDescription),
                    MostPopularTitle = "5 наиболее популярных видеороликов:",
                    Items = ConvertToItems(videos, e => e.Title),
                    TitleItemGetter =
                        item =>
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.Video, PageId.Index,
                                                                   TemplateId.ItemTipOnMainPage, item.Title),
                    UrlItemGetter =
                        item =>
                        @Url.Action("Detail", RouteConfig.VIDEO_CONTROLLER, new {group = item.Title + "/"},
                                    Request.Url.Scheme),
                    UrlImageItemGetter =
                        item =>
                        Url.Action("GetImageByName", RouteConfig.VIDEO_CONTROLLER, new {group = item.Title},
                                   Request.Url.Scheme),
                };
            });

            //задания
            AddSectionIfNeed(SectionId.UserTasks, model, () => {
                return new DescriptionSection {
                    Title = new DescriptionTitle {
                        Title = "Задания",
                        Url =
                            Url.Action("Index", RouteConfig.USER_TASKS_CONTROLLER, null, Request.Url.Scheme),
                        Description = "Перейти к заданиям пользователей"
                    },
                    Description =
                        WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.UserTasksDescription)
                };
            });

            //предложения
            AddSectionIfNeed(SectionId.Sentences, model, () => new DescriptionSection {
                Title = new DescriptionTitle {
                    Title = "Предложения",
                    Url = Url.Action("Index", RouteConfig.HOME_CONTROLLER, null, Request.Url.Scheme),
                    Description = "Перейти к предложениям"
                },
                Description =
                    WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.SentencesDescription),
                Items = null,
            });

            //аудирование
            AddSectionIfNeed(SectionId.Audio, model, () => new DescriptionSection {
                Title = new DescriptionTitle {
                    Title = "Аудирование",
                    Url = Url.Action("Index", RouteConfig.AUDIO_WORDS_CONTROLLER, null, Request.Url.Scheme),
                    Description = "Перейти к аудированию"
                },
                Description = WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.AudioDescription),
                Items = null,
            });

            //перевод слов
            AddSectionIfNeed(SectionId.WordTranslation, model, () => new DescriptionSection {
                Title = new DescriptionTitle {
                    Title = "Перевод слов",
                    Url = Url.Action("Index", RouteConfig.WORDS_TRANSLATION_CONTROLLER, null, Request.Url.Scheme),
                    Description = "Перейти к переводу слов"
                },
                Description =
                    WebSettingsConfig.Instance.GetTemplateText(SectionId.Main, TemplateId.WordTranslationDescription),
                Items = null,
            });

            //перевод фразовых глаголов
            AddSectionIfNeed(SectionId.PhraseVerbTranslation, model, () => new DescriptionSection {
                Title = new DescriptionTitle {
                    Title = "Перевод фразовых глаголов",
                    Url = Url.Action("Index", RouteConfig.PHRASAL_VERBS_TRANLATION_CONTROLLER, null, Request.Url.Scheme),
                    Description = "Перейти к переводу фразовых глаголов"
                },
                Description =
                    WebSettingsConfig.Instance.GetTemplateText(SectionId.Main,
                                                               TemplateId.PhrasalVerbsTranslationDescription),
                Items = null,
            });

            return View(model);
        }

        private static void AddSectionIfNeed(SectionId sectionId,
                                             MainModel model,
                                             Func<DescriptionSection> sectionGetter) {
            if (WebSettingsConfig.Instance.IsSectionAllowed(sectionId)) {
                model.Add(sectionId, sectionGetter());
            }
        }

        private static List<DescriptionSectionItem> ConvertToItems<T>(IEnumerable<T> items, Func<T, string> titleGetter) {
            return items.Select(e => new DescriptionSectionItem {Title = titleGetter(e)}).ToList();
        }
    }
}