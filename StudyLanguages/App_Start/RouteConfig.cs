using System.Web.Mvc;
using System.Web.Routing;
using BusinessLogic.Data.Enums;
using StudyLanguages.App_Start;

namespace StudyLanguages {
    public class RouteConfig {
        public const string MAIN_CONTROLLER_NAME = "Main";
        public const string VISUAL_DICTIONARIES_CONTROLLER_NAME = "VisualDictionaries";
        public const string VISUAL_DICTIONARY_CONTROLLER = "VisualDictionary";
        public const string PRETTY_VISUAL_DICTIONARIES_CONTROLLER_NAME = "Визуальные словари";
        public const string PRETTY_VISUAL_DICTIONARY_CONTROLLER = "Визуальный словарь по теме";
        public const string PRETTY_VISUAL_DICTIONARY_DOWNLOAD_CONTROLLER = "Скачать визуальный словарь по теме";
        public const string USER_TRAINER_VISUAL_WORDS_CONTROLLER = "TrainerVisualDictionaryWords";
        public const string PRETTY_USER_TRAINER_VISUAL_WORDS_CONTROLLER = "Карточки слов визуального словаря по теме";
        public const string PRETTY_GAPS_TRAINER_VISUAL_WORDS_CONTROLLER = "Пробелы в визуальном словаре по теме";

        private const string PRETTY_CHECK_PAYMENT = "Проверка оплаты покупок";

        public const string PRETTY_ADD_NEW_PATH = "Добавить тему";
        public const string PRETTY_GROUP_BY_WORDS_CONTROLLER = "Слова по темам";
        public const string GROUPS_BY_WORDS_CONTROLLER = "GroupsByWord";
        public const string GROUP_WORD_CONTROLLER = "GroupWord";
        public const string PRETTY_GROUP_WORD_CONTROLLER = "Слова по теме";
        public const string PRETTY_GROUP_WORD_DOWNLOAD_CONTROLLER = "Скачать слова по теме";
        public const string PRETTY_TRAINER_GROUP_WORD_CONTROLLER = "Тренажер слов по теме";
        public const string PRETTY_GAPS_TRAINER_GROUP_WORD_CONTROLLER = "Пробелы в словах на тему";
        public const string USER_TRAINER_GROUP_WORDS_CONTROLLER = "TrainerGroupWords";
        public const string PRETTY_USER_TRAINER_GROUP_WORDS_CONTROLLER = "Карточки слов по теме";

        public const string PRETTY_GROUP_BY_SENTENCES_CONTROLLER = "Фразы по темам";
        public const string GROUPS_BY_SENTENCES_CONTROLLER = "GroupsBySentence";
        public const string USER_TRAINER_GROUP_PHRASES_CONTROLLER = "TrainerGroupPhrases";
        public const string PRETTY_USER_TRAINER_GROUP_PHRASES_CONTROLLER = "Карточки фраз по теме";

        public const string GROUP_SENTENCE_CONTROLLER = "GroupSentence";
        public const string PRETTY_GROUP_SENTENCE_CONTROLLER = "Фразы по теме";
        public const string PRETTY_GROUP_SENTENCE_DOWNLOAD_CONTROLLER = "Скачать фразы по теме";
        public const string PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER = "Тренажер фраз по теме";
        public const string PRETTY_GAPS_TRAINER_GROUP_SENTENCE_CONTROLLER = "Пробелы в фразах на тему";

        public const string PRETTY_AUDIO_WORDS_CONTROLLER = "Аудирование";
        public const string AUDIO_WORDS_CONTROLLER = "AudioWords";

        public const string GROUP_PARAM_NAME = "group";

        public const string PRETTY_HOME_CONTROLLER = "Предложение";
        public const string HOME_CONTROLLER = "Home";

        public const string PRETTY_WORDS_TRANSLATION_CONTROLLER = "Перевод слов";
        public const string WORDS_TRANSLATION_CONTROLLER = "Word";

        public const string PRETTY_PHRASAL_VERBS_TRANLATION_CONTROLLER = "Перевод фразовых глаголов";
        public const string PHRASAL_VERBS_TRANLATION_CONTROLLER = "PhrasalVerbs";

        public const string GROUPS_BY_COMPARISONS_CONTROLLER = "Comparisons";
        public const string PRETTY_GROUPS_BY_COMPARISONS_CONTROLLER = "Правила употребления по группам";
        public const string COMPARISON_CONTROLLER = "Comparison";
        public const string PRETTY_COMPARISON_CONTROLLER = "Правила употребления";
        public const string PRETTY_COMPARISON_DOWNLOAD_CONTROLLER = "Скачать правила употребления";

        #region Знания

        public const string USER_KNOWLEDGE_CONTROLLER = "Knowledge";
        public const string PRETTY_USER_KNOWLEDGE_CONTROLLER = "Стена знаний";

        public const string USER_TRAINER_ALL_CONTROLLER = "TrainerAllKnowledge";
        public const string PRETTY_USER_TRAINER_CONTROLLER = "Карточки моих знаний";

        public const string USER_TRAINER_WORDS_CONTROLLER = "TrainerWordsKnowledge";
        public const string PRETTY_USER_TRAINER_WORDS_CONTROLLER = "Карточки моих слов";

        public const string USER_TRAINER_PHRASES_CONTROLLER = "TrainerPhrasesKnowledge";
        public const string PRETTY_USER_TRAINER_PHRASES_CONTROLLER = "Карточки моих фраз";

        public const string USER_TRAINER_SENTENCES_CONTROLLER = "TrainerSentencesKnowledge";
        public const string PRETTY_USER_TRAINER_SENTENCES_CONTROLLER = "Карточки моих предложений";

        public const string KNOWLEDGE_GENERATOR_CONTROLLER = "KnowledgeGenerator";
        public const string PRETTY_KNOWLEDGE_GENERATOR_CONTROLLER = "Генератор знаний";
        public const string PRETTY_KNOWLEDGE_GENERATOR_DOWNLOAD_CONTROLLER = "Скачать сгенерированные знания";

        #endregion

        #region Видео

        public const string VIDEO_CONTROLLER = "Video";
        public const string PRETTY_VIDEOS_CONTROLLER = "Видеоролики";
        public const string PRETTY_VIDEO_CONTROLLER = "Видеоролик";
        public const string PRETTY_VIDEO_TEXT_DOWNLOAD_CONTROLLER = "Скачать текст из видео";

        public const string TV_SERIES_CONTROLLER = "TVSeries";
        public const string PRETTY_TV_SERIES_CONTROLLER = "Сериалы";
        public const string PRETTY_TV_SERIE_CONTROLLER = "Сериал";

        #endregion

        #region Минилекс

        public const string POPULAR_WORDS_CONTROLLER = "PopularWords";
        public const string PRETTY_POPULAR_WORDS_CONTROLLER = "Минилекс слов Гуннемарка";
        public const string PRETTY_POPULAR_WORDS_DOWNLOAD_CONTROLLER = "Скачать минилекс слов";

        #endregion

        #region Пользователь

        public const string PROFILE_CONTROLLER = "Profile";
        public const string PRETTY_PROFILE_CONTROLLER = "Ваш профиль";

        public const string USER_TASKS_CONTROLLER = "UserTasks";
        public const string PRETTY_USER_TASKS_CONTROLLER = "Задания";
        public const string PRETTY_ADD_NEW_USER_TASKS_CONTROLLER = "Новое задание";
        public const string PRETTY_DETAIL_USER_TASKS_CONTROLLER = "Задание";

        public const string AUTHOR_ID_PARAM_NAME = "authorId";
        public const string KEY_PARAM_NAME = "key";

        #endregion

        #region Другое

        public const string COMMENT_CONTROLLER = "Comment";
        public const string PRETTY_COMMENT_CONTROLLER = "Отзывы";

        public const string HELP_CONTROLLER = "Help";
        public const string PRETTY_HELP_CONTROLLER = "Как помочь проекту";
        public const string PRETTY_VACATIONS_CONTROLLER = "Давайте сотрудничать";

        public const string SERVICE_CONTROLLER = "Service";

        #endregion

        #region Платежи

        public const string PAYMENT_CONTROLLER = "Payment";

        #endregion

        public static void RegisterRoutes(RouteCollection routes) {
            //идентификаторы должны быть больше 0 
            const string ID_CONSTRAINT = @"^[1-9]\d*$";

            //ограничение на короткое имя языка 
            const string SHORT_LANGUAGE_NAME_CONSTRAINT = @"^\w{2,5}$";

            //ограничение на не пустую строку 
            const string STRING_CONSTRAINT = @"^.+?$";

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Контроллер, который возвращает данные не для пользователей

            routes.MapRoute(
                name: "ServiceController",
                url: "sitemap.xml",
                defaults:
                    new {
                        controller = SERVICE_CONTROLLER,
                        action = "Sitemap",
                    }
                );

            #endregion

            #region Контроллер, отвечающий за произношение чего-то

            routes.MapRoute(
                name: "SpeakerController",
                url: "speak",
                defaults: new {controller = "Speaker", action = "Speak"} /*,
                constraints: new { id = ID_CONSTRAINT, type = ID_CONSTRAINT }*/
                );

            #endregion

            #region Карта слов

            routes.MapRoute(
                name: "GroupByWords",
                url: PRETTY_GROUP_BY_WORDS_CONTROLLER,
                defaults: new {controller = GROUPS_BY_WORDS_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "GroupWordNewVisitor",
                url: PRETTY_GROUP_WORD_CONTROLLER + "/NewVisitor/{id}",
                defaults: new {controller = GROUP_WORD_CONTROLLER, action = "NewVisitor"},
                constraints: new {id = new RegexConstraint(ID_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "GroupWordAddNew",
                url: PRETTY_GROUP_WORD_CONTROLLER + "/" + PRETTY_ADD_NEW_PATH,
                defaults:
                    new {
                        controller = GROUP_WORD_CONTROLLER,
                        action = "AddNew"
                    }
                );

            /*routes.MapRoute(
                name: "GroupWordExamples",
                url: PRETTY_GROUP_WORD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/Examples",
                defaults:
                    new {
                        controller = GROUP_WORD_CONTROLLER,
                        action = "Examples"
                    },
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );*/

            routes.MapRoute(
                name: "GroupWordDownload",
                url: PRETTY_GROUP_WORD_DOWNLOAD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{type}",
                defaults:
                    new {
                        controller = GROUP_WORD_CONTROLLER,
                        action = "Download"
                    },
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "GroupWordTrainer",
                url: PRETTY_TRAINER_GROUP_WORD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults:
                    new {
                        controller = GROUP_WORD_CONTROLLER,
                        action = "Trainer",
                    },
                constraints: new {
                    group = new RegexConstraint(STRING_CONSTRAINT)
                }
                );

            routes.MapRoute(
                name: "GroupWordGapsTrainer",
                url: PRETTY_GAPS_TRAINER_GROUP_WORD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults:
                    new {
                        controller = GROUP_WORD_CONTROLLER,
                        action = "GapsTrainer"
                    },
                constraints: new {
                    group = new RegexConstraint(STRING_CONSTRAINT)
                }
                );

            routes.MapRoute(
                name: "GroupWordSpecial",
                url: PRETTY_TRAINER_GROUP_WORD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{elem1}/{elem2}/",
                defaults:
                    new {
                        controller = GROUP_WORD_CONTROLLER,
                        action = "ShowSpecialItem"
                    },
                constraints: new {
                    group = new RegexConstraint(STRING_CONSTRAINT)
                }
                );

            routes.MapRoute(
                name: "GroupWordImage",
                url: PRETTY_GROUP_WORD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/Image",
                defaults: new {controller = GROUP_WORD_CONTROLLER, action = "GetImageByName"},
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "GroupWordImageById",
                url: PRETTY_GROUP_WORD_CONTROLLER + "/Image/{id}",
                defaults: new {controller = GROUP_WORD_CONTROLLER, action = "Image"},
                constraints: new {id = new RegexConstraint(ID_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "UserTrainerGroupWords",
                url: PRETTY_USER_TRAINER_GROUP_WORDS_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{action}",
                defaults:
                    new {
                        controller = USER_TRAINER_GROUP_WORDS_CONTROLLER,
                        action = "Index"
                    },
                constraints: new {
                    group = new RegexConstraint(STRING_CONSTRAINT)
                }
                );

            routes.MapRoute(
                name: "GroupWordDefault",
                url: PRETTY_GROUP_WORD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults:
                    new {
                        controller = GROUP_WORD_CONTROLLER,
                        action = "Index",
                    },
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );

            #endregion

            #region Карта фраз

            routes.MapRoute(
                name: "GroupBySentences",
                url: PRETTY_GROUP_BY_SENTENCES_CONTROLLER,
                defaults: new {controller = GROUPS_BY_SENTENCES_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "GroupSentenceNewVisitor",
                url: PRETTY_GROUP_SENTENCE_CONTROLLER + "/NewVisitor/{id}",
                defaults: new {controller = GROUP_SENTENCE_CONTROLLER, action = "NewVisitor"},
                constraints: new {id = new RegexConstraint(ID_CONSTRAINT)}
                );

            /*routes.MapRoute(
                name: "GroupSentenceExamples",
                url: PRETTY_GROUP_SENTENCE_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/Examples",
                defaults:
                    new {
                        controller = GROUP_SENTENCE_CONTROLLER,
                        action = "Examples"
                    },
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );*/

            routes.MapRoute(
                name: "GroupSentenceAddNew",
                url: PRETTY_GROUP_SENTENCE_CONTROLLER + "/" + PRETTY_ADD_NEW_PATH,
                defaults:
                    new {
                        controller = GROUP_SENTENCE_CONTROLLER,
                        action = "AddNew"
                    }
                );

            routes.MapRoute(
                name: "GroupSentenceDownload",
                url: PRETTY_GROUP_SENTENCE_DOWNLOAD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{type}",
                defaults:
                    new {
                        controller = GROUP_SENTENCE_CONTROLLER,
                        action = "Download"
                    },
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "GroupSentenceTrainer",
                url: PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults:
                    new {
                        controller = GROUP_SENTENCE_CONTROLLER,
                        action = "Trainer",
                    },
                constraints: new {
                    group = new RegexConstraint(STRING_CONSTRAINT)
                }
                );

            routes.MapRoute(
                name: "GroupSentenceGapsTrainer",
                url: PRETTY_GAPS_TRAINER_GROUP_SENTENCE_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults:
                    new {
                        controller = GROUP_SENTENCE_CONTROLLER,
                        action = "GapsTrainer"
                    },
                constraints: new {
                    group = new RegexConstraint(STRING_CONSTRAINT)
                }
                );

            routes.MapRoute(
                name: "GroupSentenceSpecial",
                url: PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{elem1}/{elem2}/",
                defaults:
                    new {
                        controller = GROUP_SENTENCE_CONTROLLER,
                        action = "ShowSpecialItem"
                    },
                constraints:
                    new {
                        group = new RegexConstraint(STRING_CONSTRAINT)
                    }
                );

            routes.MapRoute(
                name: "GroupSentenceImage",
                url: PRETTY_GROUP_SENTENCE_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/Image",
                defaults: new {controller = GROUP_SENTENCE_CONTROLLER, action = "GetImageByName"},
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "GroupSentenceImageById",
                url: PRETTY_GROUP_SENTENCE_CONTROLLER + "/Image/{id}",
                defaults: new {controller = GROUP_SENTENCE_CONTROLLER, action = "Image"},
                constraints: new {id = new RegexConstraint(ID_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "UserTrainerGroupPhrases",
                url: PRETTY_USER_TRAINER_GROUP_PHRASES_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{action}",
                defaults:
                    new {
                        controller = USER_TRAINER_GROUP_PHRASES_CONTROLLER,
                        action = "Index"
                    },
                constraints: new {
                    group = new RegexConstraint(STRING_CONSTRAINT)
                }
                );

            routes.MapRoute(
                name: "GroupSentenceDefault",
                url: PRETTY_GROUP_SENTENCE_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults:
                    new {
                        controller = GROUP_SENTENCE_CONTROLLER,
                        action = "Index"
                    },
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );

            #endregion

            #region Почувствуй разницу

            routes.MapRoute(
                name: "ComparisonNewVisitor",
                url: PRETTY_COMPARISON_CONTROLLER + "/NewVisitor/{id}",
                defaults: new {controller = COMPARISON_CONTROLLER, action = "NewVisitor"},
                constraints: new {id = new RegexConstraint(ID_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "ComparisonDownload",
                url: PRETTY_COMPARISON_DOWNLOAD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{type}",
                defaults:
                    new {
                        controller = COMPARISON_CONTROLLER,
                        action = "Download"
                    } /* ,
                constraints: new {group = STRING_CONSTRAINT}*/
                );

            routes.MapRoute(
                name: "ComparisonDefault",
                url: PRETTY_COMPARISON_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults:
                    new {
                        controller = COMPARISON_CONTROLLER,
                        action = "Index"
                    },
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "GroupsByComparison",
                url: PRETTY_GROUPS_BY_COMPARISONS_CONTROLLER,
                defaults:
                    new {
                        controller = GROUPS_BY_COMPARISONS_CONTROLLER,
                        action = "Index"
                    }
                );

            #endregion

            #region Визуальный словарь
            
            routes.MapRoute(
                name: "VisualDictionaryNewVisitor",
                url: PRETTY_VISUAL_DICTIONARY_CONTROLLER + "/NewVisitor/{id}",
                defaults: new {controller = VISUAL_DICTIONARY_CONTROLLER, action = "NewVisitor"},
                constraints: new {id = new RegexConstraint(ID_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "VisualDictionaryPreview",
                url: PRETTY_VISUAL_DICTIONARY_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/Preview",
                defaults: new { controller = VISUAL_DICTIONARY_CONTROLLER, action = "Preview" },
                 constraints: new { group = new RegexConstraint(STRING_CONSTRAINT) }
                );

            routes.MapRoute(
                name: "VisualDictionariesImage",
                url: PRETTY_VISUAL_DICTIONARY_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/Image",
                defaults: new {controller = VISUAL_DICTIONARY_CONTROLLER, action = "GetImageByName"},
                constraints: new {group = new RegexConstraint(STRING_CONSTRAINT)}
                );

            routes.MapRoute(
                name: "VisualDictionaryDownload",
                url: PRETTY_VISUAL_DICTIONARY_DOWNLOAD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{type}",
                defaults:
                    new {
                        controller = VISUAL_DICTIONARY_CONTROLLER,
                        action = "Download"
                    } /* ,
                constraints: new {group = STRING_CONSTRAINT}*/
                );

            routes.MapRoute(
                name: "UserTrainerVisualWords",
                url: PRETTY_USER_TRAINER_VISUAL_WORDS_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{action}",
                defaults:
                    new {
                        controller = USER_TRAINER_VISUAL_WORDS_CONTROLLER,
                        action = "Index"
                    },
                constraints: new {
                    group = new RegexConstraint(STRING_CONSTRAINT)
                }
                );

            routes.MapRoute(
                name: "VisualDictionary",
                url: PRETTY_VISUAL_DICTIONARY_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults:
                    new {
                        controller = VISUAL_DICTIONARY_CONTROLLER,
                        action = "Index"
                    } /*,
                constraints: new {group = STRING_CONSTRAINT}*/
                );

            routes.MapRoute(
                name: "VisualDictionaryAddNew",
                url: PRETTY_VISUAL_DICTIONARY_CONTROLLER + "/Добавить словарь",
                defaults:
                    new {
                        controller = VISUAL_DICTIONARY_CONTROLLER,
                        action = "AddNew"
                    }
                );

            routes.MapRoute(
                name: "UserGapsTrainerVisualWords",
                url: PRETTY_GAPS_TRAINER_VISUAL_WORDS_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults:
                    new {
                        controller = VISUAL_DICTIONARY_CONTROLLER,
                        action = "GapsTrainer"
                    },
                constraints: new {
                    group = new RegexConstraint(STRING_CONSTRAINT)
                }
                );

            routes.MapRoute(
                name: "VisualDictionaries",
                url: PRETTY_VISUAL_DICTIONARIES_CONTROLLER_NAME,
                defaults:
                    new {
                        controller = VISUAL_DICTIONARIES_CONTROLLER_NAME,
                        action = "Index"
                    }
                );

            #endregion

            #region Переводчик

            routes.MapRoute(
                name: "WordTranslationSpecial",
                url: PRETTY_WORDS_TRANSLATION_CONTROLLER + "/{sourceLanguage}-{destinationLanguage}/{word}/",
                defaults: new {controller = WORDS_TRANSLATION_CONTROLLER, action = "Translation"},
                constraints:
                    new {
                        sourceLanguage = SHORT_LANGUAGE_NAME_CONSTRAINT,
                        destinationLanguage = SHORT_LANGUAGE_NAME_CONSTRAINT
                    }
                );

            routes.MapRoute(
                name: "WordTranslationDefault",
                url: PRETTY_WORDS_TRANSLATION_CONTROLLER,
                defaults: new {controller = WORDS_TRANSLATION_CONTROLLER, action = "Index"});

            routes.MapRoute(
                name: "PhrasalVerbsTranslationSpecial",
                url: PRETTY_PHRASAL_VERBS_TRANLATION_CONTROLLER + "/{sourceLanguage}-{destinationLanguage}/{word}/",
                defaults: new {controller = PHRASAL_VERBS_TRANLATION_CONTROLLER, action = "Translation"},
                constraints:
                    new {
                        sourceLanguage = SHORT_LANGUAGE_NAME_CONSTRAINT,
                        destinationLanguage = SHORT_LANGUAGE_NAME_CONSTRAINT
                    }
                );

            routes.MapRoute(
                name: "PhrasalVerbsTranslationDefault",
                url: PRETTY_PHRASAL_VERBS_TRANLATION_CONTROLLER,
                defaults: new {controller = PHRASAL_VERBS_TRANLATION_CONTROLLER, action = "Index"}
                );

            #endregion

            #region Предложения

            routes.MapRoute(
                name: "HomeIndex",
                url: PRETTY_HOME_CONTROLLER,
                defaults: new {controller = HOME_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "HomeTranslation",
                url: PRETTY_HOME_CONTROLLER + "/{sourceId}/{translationId}",
                defaults: new {controller = HOME_CONTROLLER, action = "Translation"},
                constraints:
                    new {
                        sourceId = new RegexConstraint(ID_CONSTRAINT),
                        translationId = new RegexConstraint(ID_CONSTRAINT)
                    }
                );

            #endregion

            #region Аудирование

            routes.MapRoute(
                name: "AudioWordsIndex",
                url: PRETTY_AUDIO_WORDS_CONTROLLER,
                defaults: new {controller = AUDIO_WORDS_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "AudioWordsTranslation",
                url: PRETTY_AUDIO_WORDS_CONTROLLER + "/{sourceId}/{translationId}",
                defaults: new {controller = AUDIO_WORDS_CONTROLLER, action = "Translation"},
                constraints:
                    new {
                        sourceId = new RegexConstraint(ID_CONSTRAINT),
                        translationId = new RegexConstraint(ID_CONSTRAINT)
                    }
                );

            #endregion

            #region Стена знаний

            routes.MapRoute(
                name: "UserKnowledgeIndex",
                url: PRETTY_USER_KNOWLEDGE_CONTROLLER,
                defaults: new {controller = USER_KNOWLEDGE_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "UserKnowledgeTotalTrainer",
                url: PRETTY_USER_TRAINER_CONTROLLER,
                defaults: new {controller = USER_TRAINER_ALL_CONTROLLER, action = "Index"}
                );
            routes.MapRoute(
                name: "UserKnowledgeWordsTrainer",
                url: PRETTY_USER_TRAINER_WORDS_CONTROLLER,
                defaults: new {controller = USER_TRAINER_WORDS_CONTROLLER, action = "Index"}
                );
            routes.MapRoute(
                name: "UserKnowledgePhrasesTrainer",
                url: PRETTY_USER_TRAINER_PHRASES_CONTROLLER,
                defaults: new {controller = USER_TRAINER_PHRASES_CONTROLLER, action = "Index"}
                );
            routes.MapRoute(
                name: "UserKnowledgeSentencesTrainer",
                url: PRETTY_USER_TRAINER_SENTENCES_CONTROLLER,
                defaults: new {controller = USER_TRAINER_SENTENCES_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "UserKnowledgeGeneratorIndex",
                url: PRETTY_KNOWLEDGE_GENERATOR_CONTROLLER,
                defaults: new {controller = KNOWLEDGE_GENERATOR_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "UserKnowledgeGeneratorDownload",
                url: PRETTY_KNOWLEDGE_GENERATOR_DOWNLOAD_CONTROLLER + "/{type}",
                defaults: new {controller = KNOWLEDGE_GENERATOR_CONTROLLER, action = "Download"}
                );

            #endregion

            #region Видео

            routes.MapRoute(
                name: "VideoDetail",
                url: PRETTY_VIDEO_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/",
                defaults: new {controller = VIDEO_CONTROLLER, action = "Detail"}
                );

            routes.MapRoute(
                name: "VideoDownloadText",
                url: PRETTY_VIDEO_TEXT_DOWNLOAD_CONTROLLER + "/{" + GROUP_PARAM_NAME + "}/{type}",
                defaults: new {controller = VIDEO_CONTROLLER, action = "Download"}
                );

            routes.MapRoute(
                name: "VideoAddNew",
                url: PRETTY_VIDEO_CONTROLLER + "/Добавить видеоролик",
                defaults:
                    new {
                        controller = VIDEO_CONTROLLER,
                        action = "AddNew"
                    }
                );

            routes.MapRoute(
                name: "Video",
                url: PRETTY_VIDEOS_CONTROLLER,
                defaults: new {controller = VIDEO_CONTROLLER, action = "Index", type = VideoType.Clip }
                );

            /*routes.MapRoute(
                name: "OurVideo",
                url: OUR_VIDEO_CONTROLLER,
                defaults: new { controller = OUR_VIDEO_CONTROLLER, action = "Index" }
                );*/

            #endregion

            #region Сериалы

            routes.MapRoute(
                name: "TVSeriesWatch",
                url: PRETTY_TV_SERIE_CONTROLLER + "/{baseUrlPart}/{season}/{episode}",
                defaults: new { controller = TV_SERIES_CONTROLLER, action = "Watch" },
                constraints: new { season = new RegexConstraint(ID_CONSTRAINT), episode = new RegexConstraint(ID_CONSTRAINT) }
                );

            routes.MapRoute(
                name: "TVSeriesDetail",
                url: PRETTY_TV_SERIE_CONTROLLER + "/{baseUrlPart}",
                defaults: new { controller = TV_SERIES_CONTROLLER, action = "Detail" }
                );

            routes.MapRoute(
                name: "TVSeriesIndex",
                url: PRETTY_TV_SERIES_CONTROLLER,
                defaults: new { controller = TV_SERIES_CONTROLLER, action = "Index" }
                );

            #endregion

            #region Минилекс

            routes.MapRoute(
                name: "PopularWords",
                url: PRETTY_POPULAR_WORDS_CONTROLLER,
                defaults: new {controller = POPULAR_WORDS_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "PopularWordsDownload",
                url: PRETTY_POPULAR_WORDS_DOWNLOAD_CONTROLLER + "/{type}",
                defaults:
                    new {
                        controller = POPULAR_WORDS_CONTROLLER,
                        action = "Download"
                    }
                );

            #endregion

            #region Пользователь

            routes.MapRoute(
                name: "Profile",
                url: PRETTY_PROFILE_CONTROLLER,
                defaults: new {controller = PROFILE_CONTROLLER, action = "Index"}
                );

            //задания пользователя
            routes.MapRoute(
                name: "UserTasks",
                url: PRETTY_USER_TASKS_CONTROLLER,
                defaults: new {controller = USER_TASKS_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "UserTasksAddNew",
                url: PRETTY_ADD_NEW_USER_TASKS_CONTROLLER,
                defaults: new {controller = USER_TASKS_CONTROLLER, action = "NewTaskIndex"}
                );

            routes.MapRoute(
                name: "UserTasksDetail",
                url: PRETTY_DETAIL_USER_TASKS_CONTROLLER + "/{" + AUTHOR_ID_PARAM_NAME + "}/{" + KEY_PARAM_NAME + "}/",
                defaults: new {controller = USER_TASKS_CONTROLLER, action = "DetailIndex"}
                );

            #endregion

            #region Другое

            routes.MapRoute(
                name: "CommentIndex",
                url: PRETTY_COMMENT_CONTROLLER,
                defaults: new {controller = COMMENT_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "HelpIndex",
                url: PRETTY_HELP_CONTROLLER,
                defaults: new {controller = HELP_CONTROLLER, action = "Index"}
                );

            routes.MapRoute(
                name: "Vacations",
                url: PRETTY_VACATIONS_CONTROLLER,
                defaults: new { controller = HELP_CONTROLLER, action = "Vacations" }
                );

            #endregion

            #region Оплата

            routes.MapRoute(
                name: "SuccessPayment",
                url: "SuccessPayment",
                defaults: new { controller = PAYMENT_CONTROLLER, action = "Success" }
                );

            routes.MapRoute(
                name: "FailPayment",
                url: "FailPayment",
                defaults: new { controller = PAYMENT_CONTROLLER, action = "Fail" }
                );

            routes.MapRoute(
                name: "ResultPayment",
                url: "ResultPayment",
                defaults: new { controller = PAYMENT_CONTROLLER, action = "Result" }
                );

            routes.MapRoute(
                name: "CheckPayment",
                url: PRETTY_CHECK_PAYMENT,
                defaults: new { controller = PAYMENT_CONTROLLER, action = "Check" }
                );

            #endregion

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults:
                    new {controller = MAIN_CONTROLLER_NAME, action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}