using NUnit.Framework;
using StudyLanguages;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;

namespace UnitTests.Web.Helpers {
    [TestFixture]
    public class RedirectHelperTest {
        public RedirectHelperTest() {
            WebSettingsConfig.SetPath("../../");
        }

        private static void AssertGetNewUrl(string oldUrl, string expectedUrl) {
            string newUrl = RedirectHelper.GetNewUrl(oldUrl);
            Assert.That(newUrl, Is.EqualTo(expectedUrl));
        }

        private static void AssertNullGetNewUrl(string oldUrl) {
            string newUrl = RedirectHelper.GetNewUrl(oldUrl);
            Assert.That(newUrl, Is.Null);
        }

        [Test]
        public void GetNewUrl_AudioWords() {
            AssertNullGetNewUrl("http://studyfun.ru/AudioWords234swrwer");
            AssertNullGetNewUrl("http://studyfun.ru/AudioWords124124124/");

            string newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                           UrlBuilder.GetAudioWordsUrl());
            AssertGetNewUrl("http://studyfun.ru/AudioWords", newUrl);
            AssertGetNewUrl("http://studyfun.ru/AudioWords/", newUrl);
            AssertGetNewUrl("studyfun.ru/AudioWords", newUrl);
            AssertGetNewUrl("studyfun.ru/AudioWords/", newUrl);

            AssertNullGetNewUrl("http://studyfun.ru/AudioWords/Translation");
            AssertNullGetNewUrl("http://studyfun.ru/AudioWords/Translation/");

            newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                    UrlBuilder.GetAudioWordsTrainerUrl("123", "456"));
            AssertGetNewUrl("http://studyfun.ru/AudioWords/Translation/123/456", newUrl);
            AssertGetNewUrl("http://studyfun.ru/AudioWords/Translation/123/456/", newUrl);
            AssertGetNewUrl(
                "studyfun.ru/AudioWords/Translation/123/456",
                newUrl);
            AssertGetNewUrl(
                "studyfun.ru/AudioWords/Translation/123/456/",
                newUrl);
        }

        [Test]
        public void GetNewUrl_GroupSentences() {
            AssertNullGetNewUrl("http://studyfun.ru/GroupsBySentence234swrwer");
            AssertNullGetNewUrl("http://studyfun.ru/GroupsBySentence124124124/");

            string newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                           UrlBuilder.GetAllGroupSentencesUrl());
            AssertGetNewUrl("http://studyfun.ru/GroupsBySentence", newUrl);
            AssertGetNewUrl("http://studyfun.ru/GroupsBySentence/", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupsBySentence", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupsBySentence/", newUrl);

            AssertNullGetNewUrl("http://studyfun.ru/GroupSentence/");
            AssertNullGetNewUrl("http://studyfun.ru/GroupSentence");
            AssertNullGetNewUrl("http://studyfun.ru/GroupSentence/Image/2");

            newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                    UrlBuilder.GetGroupSentencesUrl("Привет"));
            AssertGetNewUrl("http://studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/All", newUrl);
            AssertGetNewUrl("http://studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/All", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/All", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/All", newUrl);

            newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                    UrlBuilder.GetSentencesTrainerUrl("Привет"));
            AssertGetNewUrl("http://studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/", newUrl);
            AssertGetNewUrl("http://studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82", newUrl);

            newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                    UrlBuilder.GetSpecialSentencesTrainerUrl("Привет", "123", "456"));
            AssertGetNewUrl("http://studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/123/456", newUrl);
            AssertGetNewUrl("http://studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/123/456/", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/123/456", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupSentence/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/123/456/", newUrl);

            AssertGetNewUrl(
                "http://studyfun.ru/" + RouteConfig.PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER
                + "/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/123/456", newUrl);
            AssertGetNewUrl(
                "http://studyfun.ru/" + UrlBuilder.EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER)
                + "/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/123/456", newUrl);

            AssertNullGetNewUrl("http://studyfun.ru/" + RouteConfig.PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER
                                + "/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/123/456/");
            AssertNullGetNewUrl(
                "http://studyfun.ru/" + UrlBuilder.EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_SENTENCE_CONTROLLER)
                + "/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/123/456/");
        }

        [Test]
        public void GetNewUrl_GroupWords() {
            AssertNullGetNewUrl("http://studyfun.ru/GroupsByWord234swrwer");
            AssertNullGetNewUrl("http://studyfun.ru/GroupsByWord124124124/");

            string newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                           UrlBuilder.GetAllGroupWordsUrl());
            AssertGetNewUrl("http://studyfun.ru/GroupsByWord", newUrl);
            AssertGetNewUrl("http://studyfun.ru/GroupsByWord/", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupsByWord", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupsByWord/", newUrl);

            AssertNullGetNewUrl("http://studyfun.ru/GroupWord/");
            AssertNullGetNewUrl("http://studyfun.ru/GroupWord");
            AssertNullGetNewUrl("http://studyfun.ru/GroupWord/Image/2");

            newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                    UrlBuilder.GetGroupWordsUrl("Привет"));
            AssertGetNewUrl("http://studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/All", newUrl);
            AssertGetNewUrl("http://studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/All", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/All", newUrl);
            AssertGetNewUrl("studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/All", newUrl);

            newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                    UrlBuilder.GetWordsTrainerUrl("Привет"));
            AssertGetNewUrl("http://studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/", newUrl);
            AssertGetNewUrl("http://studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82", newUrl);

            newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                    UrlBuilder.GetSpecialWordsTrainerUrl("Привет", "hobby", "хобби"));
            AssertGetNewUrl(
                "http://studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/hobby/%d1%85%d0%be%d0%b1%d0%b1%d0%b8",
                newUrl);
            AssertGetNewUrl(
                "http://studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/hobby/%d1%85%d0%be%d0%b1%d0%b1%d0%b8/",
                newUrl);

            AssertGetNewUrl(
                "http://studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/hobby/%d1%85%d0%be%d0%b1%d0%b1%d0%b8/",
                newUrl);
            AssertGetNewUrl(
                "studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/hobby/%d1%85%d0%be%d0%b1%d0%b1%d0%b8",
                newUrl);
            AssertGetNewUrl(
                "studyfun.ru/GroupWord/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/hobby/%d1%85%d0%be%d0%b1%d0%b1%d0%b8/",
                newUrl);
            AssertGetNewUrl(
                "http://studyfun.ru/" + RouteConfig.PRETTY_TRAINER_GROUP_WORD_CONTROLLER
                + "/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/hobby/%d1%85%d0%be%d0%b1%d0%b1%d0%b8",
                newUrl);
            AssertGetNewUrl(
                "http://studyfun.ru/" + UrlBuilder.EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_WORD_CONTROLLER)
                + "/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/hobby/%d1%85%d0%be%d0%b1%d0%b1%d0%b8",
                newUrl);

            //с косыми на конце
            AssertNullGetNewUrl("http://studyfun.ru/" + RouteConfig.PRETTY_TRAINER_GROUP_WORD_CONTROLLER
                                + "/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/hobby/%d1%85%d0%be%d0%b1%d0%b1%d0%b8/");
            AssertNullGetNewUrl("http://studyfun.ru/"
                                + UrlBuilder.EncodePartUrl(RouteConfig.PRETTY_TRAINER_GROUP_WORD_CONTROLLER)
                                + "/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/hobby/%d1%85%d0%be%d0%b1%d0%b1%d0%b8/");
        }

        [Test]
        public void GetNewUrl_Home() {
            AssertNullGetNewUrl("http://studyfun.ru/Home234swrwer");
            AssertNullGetNewUrl("http://studyfun.ru/Home124124124/");

            string newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                           UrlBuilder.GetSentenceHomeUrl());
            AssertGetNewUrl("http://studyfun.ru/Home", newUrl);
            AssertGetNewUrl("http://studyfun.ru/Home/", newUrl);
            AssertGetNewUrl("studyfun.ru/Home", newUrl);
            AssertGetNewUrl("studyfun.ru/Home/", newUrl);

            AssertNullGetNewUrl("http://studyfun.ru/Home/Translation");
            AssertNullGetNewUrl("http://studyfun.ru/Home/Translation/");

            newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                    UrlBuilder.GetSentenceHomeTrainerUrl("123", "456"));
            AssertGetNewUrl("http://studyfun.ru/Home/Translation/123/456", newUrl);
            AssertGetNewUrl("http://studyfun.ru/Home/Translation/123/456/", newUrl);
            AssertGetNewUrl(
                "studyfun.ru/Home/Translation/123/456",
                newUrl);
            AssertGetNewUrl(
                "studyfun.ru/Home/Translation/123/456/",
                newUrl);
        }

        [Test]
        public void GetNewUrl_MyKnowledge() {
            string newUrl = UrlBuilder.GetKnowledgeWallUrl();
            AssertGetNewUrl("http://studyfun.ru/Мои знания", "http://studyfun.ru/" + newUrl);
            AssertGetNewUrl("http://studyfun.ru/Мои знания/", "http://studyfun.ru/" + newUrl);
            AssertGetNewUrl("http://studyfun.ru/%D0%9C%D0%BE%D0%B8%20%D0%B7%D0%BD%D0%B0%D0%BD%D0%B8%D1%8F",
                            "http://studyfun.ru/" + newUrl);
            AssertGetNewUrl("http://studyfun.ru/%D0%9C%D0%BE%D0%B8%20%D0%B7%D0%BD%D0%B0%D0%BD%D0%B8%D1%8F/",
                            "http://studyfun.ru/" + newUrl);
        }

        [Test]
        public void GetNewUrl_PhrasalVerbsTranslation() {
            AssertNullGetNewUrl("http://studyfun.ru/PhrasalVerbs234swrwer");
            AssertNullGetNewUrl("http://studyfun.ru/PhrasalVerbs124124124/");

            string newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                           UrlBuilder.GetTranslationDefaulUrl(
                                                               RouteConfig.PHRASAL_VERBS_TRANLATION_CONTROLLER));
            AssertGetNewUrl("http://studyfun.ru/PhrasalVerbs", newUrl);
            AssertGetNewUrl("http://studyfun.ru/PhrasalVerbs/", newUrl);
            AssertGetNewUrl("studyfun.ru/PhrasalVerbs", newUrl);
            AssertGetNewUrl("studyfun.ru/PhrasalVerbs/", newUrl);
        }

        [Test]
        public void GetNewUrl_VisualDictionary() {
            AssertNullGetNewUrl("http://studyfun.ru/VisualDictionaries124124124");
            AssertNullGetNewUrl("http://studyfun.ru/VisualDictionaries124124124/");

            AssertGetNewUrl("http://studyfun.ru/VisualDictionaries", "http://studyfun.ru/");
            AssertGetNewUrl("http://studyfun.ru/VisualDictionaries/", "http://studyfun.ru/");
            AssertGetNewUrl("studyfun.ru/VisualDictionaries", WebSettingsConfig.Instance.DomainWithProtocol);
            AssertGetNewUrl("studyfun.ru/VisualDictionaries/", WebSettingsConfig.Instance.DomainWithProtocol);

            AssertNullGetNewUrl("http://studyfun.ru/VisualDictionary/");
            AssertNullGetNewUrl("http://studyfun.ru/VisualDictionary");

            string newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                           UrlBuilder.GetVisualDictionaryUrl("Hello"));
            AssertGetNewUrl("http://studyfun.ru/VisualDictionary/Hello/", newUrl);
            AssertGetNewUrl("http://studyfun.ru/VisualDictionary/Hello", newUrl);

            newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                    UrlBuilder.GetVisualDictionaryUrl("Привет"));
            AssertGetNewUrl("http://studyfun.ru/VisualDictionary/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/", newUrl);
            AssertGetNewUrl("http://studyfun.ru/VisualDictionary/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82", newUrl);
            AssertGetNewUrl("studyfun.ru/VisualDictionary/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82/", newUrl);
            AssertGetNewUrl("studyfun.ru/VisualDictionary/%d0%9f%d1%80%d0%b8%d0%b2%d0%b5%d1%82", newUrl);
        }

        [Test]
        public void GetNewUrl_WordsTranslation() {
            AssertNullGetNewUrl("http://studyfun.ru/Word234swrwer");
            AssertNullGetNewUrl("http://studyfun.ru/Word124124124/");

            string newUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol,
                                                           UrlBuilder.GetTranslationDefaulUrl(
                                                               RouteConfig.WORDS_TRANSLATION_CONTROLLER));
            AssertGetNewUrl("http://studyfun.ru/Word", newUrl);
            AssertGetNewUrl("http://studyfun.ru/Word/", newUrl);
            AssertGetNewUrl("studyfun.ru/Word", newUrl);
            AssertGetNewUrl("studyfun.ru/Word/", newUrl);
        }
    }
}