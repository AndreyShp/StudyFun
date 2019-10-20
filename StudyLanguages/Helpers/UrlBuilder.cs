using System;
using System.Globalization;
using System.Web;

namespace StudyLanguages.Helpers {
    public class UrlBuilder {
        //private static readonly Regex _regex = new Regex("(%[0-9a-f][0-9a-f])", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

        private static string GetUrl(HttpRequestBase request, string relativeUrl) {
            //TODO: разобраться как работать с Url.HttpRouteUrl
            string domain = request.Url.Scheme + "://" + request.Url.Host + request.ApplicationPath;
            return ConcatDomainWithUrl(domain, relativeUrl);
        }

        public static string ConcatDomainWithUrl(string domain, string relativeUrl) {
            if (!domain.EndsWith("/")) {
                domain += "/";
            }
            domain += relativeUrl;
            return domain;
        }

        public static string EncodePartUrl(string part) {
            string result = HttpUtility.UrlPathEncode(part);
            //result = _regex.Replace(result, c => c.Value.ToUpper());
            return result;
        }

        public static string GetImageUrlById(HttpRequestBase request, string controllerName, string id) {
            return GetUrl(request, controllerName + "/Image/" + id);
        }

        #region Предложение

        public static string GetSentenceHomeTrainerUrl(HttpRequestBase request, long sentenceId1, long sentenceId2) {
            return GetUrl(request,
                          GetSentenceHomeTrainerUrl(sentenceId1.ToString(CultureInfo.InvariantCulture),
                                                    sentenceId2.ToString(CultureInfo.InvariantCulture)));
        }

        public static string GetSentenceHomeUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_HOME_CONTROLLER);
        }

        public static string GetSentenceHomeTrainerUrl(string sentenceId1, string sentenceId2) {
            return EncodePartUrl(RouteConfig.PRETTY_HOME_CONTROLLER) + "/" + sentenceId1 + "/" + sentenceId2;
        }

        public static string GetHomeActionUrl(string action) {
            return RouteConfig.HOME_CONTROLLER + "/" + action;
        }

        #endregion

        #region Перевод слов/фразовых глаголов

        public static string GetTranslationDefaulUrl(string controllerName) {
            string prettyUrl = GetPrettyTranslationUrl(controllerName);
            return EncodePartUrl(prettyUrl);
        }

        public static string GetTranslationActionUrl(string controllerName, string action) {
            return controllerName + "/" + action;
        }

        public static string GetTranslationPatternUrl(string controllerName) {
            string prettyUrl = GetPrettyTranslationUrl(controllerName);
            return EncodePartUrl(prettyUrl) + "/{0}-{1}/{2}/";
        }

        private static string GetPrettyTranslationUrl(string controllerName) {
            return controllerName == RouteConfig.WORDS_TRANSLATION_CONTROLLER
                       ? RouteConfig.PRETTY_WORDS_TRANSLATION_CONTROLLER
                       : RouteConfig.PRETTY_PHRASAL_VERBS_TRANLATION_CONTROLLER;
        }

        #endregion

        #region Аудирование

        public static string GetAudioWordsTrainerUrl(HttpRequestBase request, long wordId1, long wordId2) {
            return GetUrl(request,
                          GetAudioWordsTrainerUrl(wordId1.ToString(CultureInfo.InvariantCulture),
                                                  wordId2.ToString(CultureInfo.InvariantCulture)));
        }

        public static string GetAudioWordsUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_AUDIO_WORDS_CONTROLLER);
        }

        public static string GetAudioWordsTrainerUrl(string wordId1, string wordId2) {
            return EncodePartUrl(RouteConfig.PRETTY_AUDIO_WORDS_CONTROLLER) + "/" + wordId1 + "/" + wordId2;
        }

        public static string GetAudioWordsActionUrl(string action) {
            return RouteConfig.AUDIO_WORDS_CONTROLLER + "/" + action;
        }

        public static string GetAudioWordsImageUrl(string id) {
            return GetAudioWordsActionUrl("Image") + "/" + id;
        }

        public static string GetAudioWordsImageUrl(HttpRequestBase request, long id) {
            string url = GetAudioWordsImageUrl(id.ToString(CultureInfo.InvariantCulture));
            return GetUrl(request, url);
        }

        #endregion

        #region Слова по группам

        public static string GetAllGroupWordsUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_GROUP_BY_WORDS_CONTROLLER);
        }

        public static string GetGroupWordsUrl(HttpRequestBase request, string groupName) {
            return GetUrl(request, GetGroupWordsUrl(groupName));
        }

        public static string GetGroupWordsUrl(string groupName) {
            return EncodePartUrl(RouteConfig.PRETTY_GROUP_WORD_CONTROLLER) + "/" + EncodePartUrl(groupName) + "/";
        }

        public static string GetGroupWordsTrainerUrl(string groupName) {
            return EncodePartUrl(RouteConfig.PRETTY_USER_TRAINER_GROUP_WORDS_CONTROLLER) + "/"
                   + EncodePartUrl(groupName) + "/";
        }

        public static string GetWordsTrainerUrl(string groupName) {
            return
                EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_WORD_CONTROLLER) + "/"
                + EncodePartUrl(groupName) + "/";
        }

        public static string GetSpecialWordsTrainerUrl(string groupName,
                                                       string word1,
                                                       string word2) {
            return EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_WORD_CONTROLLER) + "/"
                   + EncodePartUrl(groupName) + "/"
                   + EncodePartUrl(word1) + "/" + EncodePartUrl(word2) + "/";
        }

        #endregion

        #region Фразы по группам

        public static string GetAllGroupSentencesUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_GROUP_BY_SENTENCES_CONTROLLER);
        }

        public static string GetGroupSentencesUrl(HttpRequestBase request, string groupName) {
            return GetUrl(request, GetGroupSentencesUrl(groupName));
        }

        public static string GetGroupSentencesUrl(string groupName) {
            return
                EncodePartUrl(RouteConfig.PRETTY_GROUP_SENTENCE_CONTROLLER) + "/" + EncodePartUrl(groupName)
                + "/";
        }

        public static string GetGroupPhrasesTrainerUrl(string groupName) {
            return EncodePartUrl(RouteConfig.PRETTY_USER_TRAINER_GROUP_PHRASES_CONTROLLER) + "/"
                   + EncodePartUrl(groupName) + "/";
        }

        public static string GetSentencesTrainerUrl(string groupName) {
            return
                EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER) + "/"
                + EncodePartUrl(groupName) + "/";
        }

        public static string GetSpecialSentencesTrainerUrl(string groupName,
                                                           string sentenceId1,
                                                           string sentenceId2) {
            return
                EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER) + "/"
                + EncodePartUrl(groupName)
                + "/" + sentenceId1 + "/" + sentenceId2 + "/";
        }

        #endregion

        #region Визуальные словари

        public static string GetVisualDictionariesUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_VISUAL_DICTIONARIES_CONTROLLER_NAME);
        }

        public static string GetVisualDictionaryUrl(string title) {
            return EncodePartUrl(RouteConfig.PRETTY_VISUAL_DICTIONARY_CONTROLLER) + "/" + EncodePartUrl(title) + "/";
        }

        public static string GetVisualDictionaryTrainerUrl(string title) {
            return EncodePartUrl(RouteConfig.PRETTY_USER_TRAINER_VISUAL_WORDS_CONTROLLER) + "/" + EncodePartUrl(title)
                   + "/";
        }

        public static string GetVisualDictionaryUrl(HttpRequestBase request, string title) {
            return GetUrl(request, GetVisualDictionaryUrl(title));
        }

        #endregion

        #region Почувствуй разницу

        public static string GetComparisonsUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_GROUPS_BY_COMPARISONS_CONTROLLER);
        }

        public static string GetComparisonUrl(string title) {
            return EncodePartUrl(RouteConfig.PRETTY_COMPARISON_CONTROLLER) + "/" + EncodePartUrl(title) + "/";
        }

        #endregion

        #region Сериалы

        public static string GetTVSeriesUrl(string title) {
            return EncodePartUrl(RouteConfig.PRETTY_TV_SERIE_CONTROLLER) + "/" + EncodePartUrl(title);
        }

        public static string GetTVSeriesDetailUrl(string title, string season, string episode) {
            return EncodePartUrl(RouteConfig.PRETTY_TV_SERIE_CONTROLLER) + "/" + EncodePartUrl(title) + "/" + EncodePartUrl(season) + "/" + EncodePartUrl(episode);
        }

        #endregion

        #region Видео

        public static string GetVideosUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_VIDEOS_CONTROLLER);
        }

        public static string GetVideoDetailUrl(string title) {
            return EncodePartUrl(RouteConfig.PRETTY_VIDEO_CONTROLLER) + "/" + EncodePartUrl(title) + "/";
        }

        #endregion

        #region Знания

        public static string GetKnowledgeWallUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_USER_KNOWLEDGE_CONTROLLER);
        }

        public static string GetKnowledgeGeneratorUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_KNOWLEDGE_GENERATOR_CONTROLLER);
        }

        #endregion

        #region Популярные слова/фразы

        public static string GetPopularWordsUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_POPULAR_WORDS_CONTROLLER);
        }

        #endregion

        #region Задания пользователя

        public static string GetUserTasksUrl() {
            return EncodePartUrl(RouteConfig.PRETTY_USER_TASKS_CONTROLLER);
        }

        public static string GetUserTaskUrl(long authorId, string taskKey) {
            return EncodePartUrl(RouteConfig.PRETTY_DETAIL_USER_TASKS_CONTROLLER) + "/"
                   + EncodePartUrl(authorId.ToString(CultureInfo.InvariantCulture)) + "/"
                   + EncodePartUrl(taskKey.ToString(CultureInfo.InvariantCulture)) + "/";
        }

        #endregion

        #region Разное

        #endregion

        #region Nested type: Patterns

        public static class Patterns {
            /*public const string HOME_TRANSLATION = RouteConfig.HOME_CONTROLLER + "/Translation/{0}/{1}/";*/
            public const string AUDIO_WORDS_TRANSLATION = RouteConfig.AUDIO_WORDS_CONTROLLER + "/Translation/{0}/{1}/";
        }

        #endregion
    }
}