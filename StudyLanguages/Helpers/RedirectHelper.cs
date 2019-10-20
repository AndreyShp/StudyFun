using System.Collections.Generic;
using System.Text.RegularExpressions;
using StudyLanguages.Configs;

namespace StudyLanguages.Helpers {
    public static class RedirectHelper {
        private static readonly List<RedirectRule> _redirectRules = new List<RedirectRule>();

        static RedirectHelper() {
            AddRedirectRule("VisualDictionaries[/]?$", "", null);

            string newUrl = UrlBuilder.GetVisualDictionaryUrl("VisDictName");
            AddRedirectRule("VisualDictionary/(?<VisDictName>[^/]+)[/]?$", newUrl, new[] {"VisDictName"});

            newUrl = UrlBuilder.GetKnowledgeWallUrl();
            AddRedirectRule("Мои знания", newUrl, new string[0]);
            AddRedirectRule("Мои знания/", newUrl, new string[0]);
            AddRedirectRule("%D0%9C%D0%BE%D0%B8%20%D0%B7%D0%BD%D0%B0%D0%BD%D0%B8%D1%8F", newUrl, new string[0]);
            AddRedirectRule("%D0%9C%D0%BE%D0%B8%20%D0%B7%D0%BD%D0%B0%D0%BD%D0%B8%D1%8F/", newUrl, new string[0]);

            newUrl = UrlBuilder.GetAllGroupWordsUrl();
            AddRedirectRule("GroupsByWord[/]?$", newUrl, null);

            newUrl = UrlBuilder.GetSpecialWordsTrainerUrl("GroupNameWord", "Word1", "Word2");
            AddRedirectRule("GroupWord/(?<GroupNameWord>[^/]+)/(?<Word1>[^/]+)/(?<Word2>[^/]+)[/]?$", newUrl,
                            new[] {"GroupNameWord", "Word1", "Word2"});
            AddRedirectRule(
                RouteConfig.PRETTY_TRAINER_GROUP_WORD_CONTROLLER
                + "/(?<GroupNameWord>[^/]+)/(?<Word1>[^/]+)/(?<Word2>[^/]+)$", newUrl,
                new[] {"GroupNameWord", "Word1", "Word2"});
            AddRedirectRule(
                UrlBuilder.EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_WORD_CONTROLLER)
                + "/(?<GroupNameWord>[^/]+)/(?<Word1>[^/]+)/(?<Word2>[^/]+)$", newUrl,
                new[] {"GroupNameWord", "Word1", "Word2"});

            newUrl = UrlBuilder.GetGroupWordsUrl("GroupNameWord");
            AddRedirectRule("GroupWord/(?<GroupNameWord>[^/]+)/All[/]?$", newUrl, new[] {"GroupNameWord"});

            newUrl = UrlBuilder.GetWordsTrainerUrl("GroupNameWord");
            AddRedirectRule("GroupWord/(?<GroupNameWord>[^/]+)[/]?$", newUrl, new[] {"GroupNameWord"});

            newUrl = UrlBuilder.GetAllGroupSentencesUrl();
            AddRedirectRule("GroupsBySentence[/]?$", newUrl, null);

            newUrl = UrlBuilder.GetSpecialSentencesTrainerUrl("GroupNameSentence", "Sentence1", "Sentence2");
            AddRedirectRule("GroupSentence/(?<GroupNameSentence>[^/]+)/(?<Sentence1>[^/]+)/(?<Sentence2>[^/]+)[/]?$",
                            newUrl,
                            new[] {"GroupNameSentence", "Sentence1", "Sentence2"});
            AddRedirectRule(
                RouteConfig.PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER
                + "/(?<GroupNameSentence>[^/]+)/(?<Sentence1>[^/]+)/(?<Sentence2>[^/]+)$", newUrl,
                new[] {"GroupNameSentence", "Sentence1", "Sentence2"});
            AddRedirectRule(
                UrlBuilder.EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER)
                + "/(?<GroupNameSentence>[^/]+)/(?<Sentence1>[^/]+)/(?<Sentence2>[^/]+)$", newUrl,
                new[] {"GroupNameSentence", "Sentence1", "Sentence2"});

            newUrl = UrlBuilder.GetGroupSentencesUrl("GroupNameSentence");
            AddRedirectRule("GroupSentence/(?<GroupNameSentence>[^/]+)/All[/]?$", newUrl, new[] {"GroupNameSentence"});

            newUrl = UrlBuilder.GetSentencesTrainerUrl("GroupNameSentence");
            AddRedirectRule("GroupSentence/(?<GroupNameSentence>[^/]+)[/]?$", newUrl, new[] {"GroupNameSentence"});

            newUrl = UrlBuilder.GetSentenceHomeTrainerUrl("Sentence1", "Sentence2");
            AddRedirectRule("Home/Translation/(?<Sentence1>[^/]+)/(?<Sentence2>[^/]+)[/]?$", newUrl,
                            new[] {"Sentence1", "Sentence2"});

            newUrl = UrlBuilder.GetSentenceHomeUrl();
            AddRedirectRule("Home[/]?$", newUrl, null);

            newUrl = UrlBuilder.GetAudioWordsTrainerUrl("Word1", "Word2");
            AddRedirectRule("AudioWords/Translation/(?<Word1>[^/]+)/(?<Word2>[^/]+)[/]?$", newUrl,
                            new[] {"Word1", "Word2"});

            newUrl = UrlBuilder.GetAudioWordsUrl();
            AddRedirectRule("AudioWords[/]?$", newUrl, null);

            newUrl = UrlBuilder.GetTranslationDefaulUrl(RouteConfig.WORDS_TRANSLATION_CONTROLLER);
            AddRedirectRule("Word[/]?$", newUrl, null);

            newUrl = UrlBuilder.GetTranslationDefaulUrl(RouteConfig.PHRASAL_VERBS_TRANLATION_CONTROLLER);
            AddRedirectRule("PhrasalVerbs[/]?$", newUrl, null);
        }

        private static void AddRedirectRule(string oldUrlMask, string newUrlMask, IEnumerable<string> groupNames) {
            _redirectRules.Add(new RedirectRule(oldUrlMask, newUrlMask, groupNames));
        }

        public static string GetNewUrl(string url) {
            foreach (RedirectRule redirectRule in _redirectRules) {
                if (redirectRule.IsMatch(url)) {
                    return redirectRule.NewUrl;
                }
            }
            return null;
        }

        #region Nested type: RedirectRule

        private class RedirectRule {
            private const string MASK_PLACE = "~maskPlace~";
            private readonly IEnumerable<string> _groupNames;
            private readonly Regex _mask;
            private readonly string _newUrl;

            public RedirectRule(string oldUrlMask, string newUrlMask, IEnumerable<string> groupNames) {
                string escapedUrl =
                    Regex.Escape(UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.Domain, MASK_PLACE));
                escapedUrl = escapedUrl.Replace(MASK_PLACE, oldUrlMask);
                _newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol, newUrlMask);

                _mask = new Regex(escapedUrl, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
                _groupNames = groupNames ?? new string[0];
            }

            public string NewUrl { get; private set; }

            public bool IsMatch(string currentUrl) {
                Match match = _mask.Match(currentUrl);
                if (!match.Success) {
                    return false;
                }

                NewUrl = _newUrl;
                foreach (string groupName in _groupNames) {
                    Group group = match.Groups[groupName];
                    if (group != null) {
                        NewUrl = NewUrl.Replace(groupName, group.Value);
                    }
                }
                return true;
            }
        }

        #endregion
    }
}