using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using BusinessLogic;
using BusinessLogic.Data;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Money;
using BusinessLogic.DataQuery.UserRepository;
using BusinessLogic.ExternalData;
using BusinessLogic.Helpers.Caches;
using BusinessLogic.Logger;
using BusinessLogic.PaymentSystems;
using BusinessLogic.Validators;

namespace StudyLanguages.Configs {
    /// <summary>
    /// Класс с веб настройками
    /// </summary>
    public class WebSettingsConfig : Singleton<WebSettingsConfig>, IWebSettings, IConfigurable {
        private static string _basePathToWriteData;
        private readonly TextTemplates _templates = new TextTemplates();
        private Action<WebSettingsConfig>[] _deferredActions;

        private RepositoryFactory _repositoryFactory;
        private HashSet<SectionId> _salesSections;
        private Dictionary<SectionId, ISalesSettings> _salesSettings;
        private HashSet<long> _fullRightUserIds;

        public WebSettingsConfig() {
            ForeignToRussianKeys = new Dictionary<char, char>();
        }

        public LanguageShortName DefaultLanguageFrom { get; private set; }

        public LanguageShortName DefaultLanguageTo { get; private set; }

        public string DomainWithProtocol {
            get { return "http://" + Domain + "/"; }
        }

        private static string BasePathToWriteData {
            get {
                if (_basePathToWriteData == null) {
                    _basePathToWriteData = HttpContext.Current.Server.MapPath("~/App_Data/");
                }
                return _basePathToWriteData;
            }
        }

        public bool CanPronounce { get; private set; }

        public string CookieWideDomain { get; private set; }

        public Dictionary<char, char> ForeignToRussianKeys { get; private set; }
        public Dictionary<char, char> RussianToForeignKeys { get; private set; }

        public string Counters { get; private set; }

        public string YandexMetrikaId { get; private set; }

        public List<AnotherDomainInfo> AnotherDomains { get; private set; }
        public string Logo { get; private set; }
        public UserLanguages DefaultUserLanguages { get; private set; }

        #region IConfigurable Members

        public void Configure() {
            string fullConfigName = Path.Combine(BasePathToWriteData, "WebSettings.xml");

            if (_repositoryFactory != null) {
                try {
                    _repositoryFactory.Reload();
                } catch (Exception e) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "WebSettingsConfig.Configure при попытке перегрузить репозитории возникло исключение {0}", e);
                }
            } else {
                _repositoryFactory = new RepositoryFactory(new RepositoryCache(BasePathToWriteData));
            }

            if (SalesPicturesCache != null) {
                SalesPicturesCache.ClearMemory();
            } else {
                string salesPicturesPath = Path.Combine(BasePathToWriteData, "Caches", "Sales");
                SalesPicturesCache = new DiskCache(salesPicturesPath);
            }

            if (CommonDiskCache != null) {
                CommonDiskCache.ClearMemory();
            } else {
                string commonDiskPath = Path.Combine(BasePathToWriteData, "Caches", "Common");
                CommonDiskCache = new DiskCache(commonDiskPath, false);
            }

            DefaultUserLanguages = null;

            var stream = new StreamReader(fullConfigName, Encoding.UTF8);
            XDocument document = XDocument.Load(stream);
            XElement root = document.Root;
            if (root == null) {
                //TODO: ругаться
                return;
            }

            Domain = XmlParseHelper.Get<string>(root, "Domain");
            Logo = XmlParseHelper.Get<string>(root, "Logo");
            CookieWideDomain = XmlParseHelper.Get<string>(root, "CookieWideDomain");
            CanPronounce = XmlParseHelper.Get<bool>(root, "CanPronounce");

            XElement defaultLanguages = root.Element("DefaultLanguages");
            DefaultLanguageFrom = GetLanguage(defaultLanguages, "From");
            DefaultLanguageTo = GetLanguage(defaultLanguages, "To");

            AvailableSections =
                new HashSet<SectionId>(XmlParseHelper.GetList<SectionId>(root, "AvailableSections", "SectionId"));
            //TODO: проверить корректность конфига, если что-то не так, то ругаться

            LoadKeyboardKeys(root);

            YandexMetrikaId = XmlParseHelper.Get<string>(root, "YandexMetrikaId");
            Counters = XmlParseHelper.Get<string>(root, "Counters");
            //подгружаем другие домены, а также удаляем текущий домен
            LoadAnotherDomains(root);

            LoadSalesSettings(root);

            PathToTopBanner = XmlParseHelper.Get<string>(root, "Banners", "TopPath");

            _templates.Load(root.Element("Patterns"));

            SetDeferredSettings(_deferredActions);

            MaxIDUserForTopMessage = XmlParseHelper.Get<long>(root, "MaxIDUserForTopMessage");

            WebPath = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/") : null;

            _fullRightUserIds = new HashSet<long>(XmlParseHelper.GetList<long>(root, "FullRightUserId"));
        }

        #endregion

        #region IWebSettings Members

        public DiskCache SalesPicturesCache { get; private set; }
        public DiskCache CommonDiskCache { get; private set; }

        public string Domain { get; private set; }

        public HashSet<SectionId> AvailableSections { get; private set; }

        /// <summary>
        /// Определяет разрешена ли секция
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>true - разрешена, false - запрещена</returns>
        public bool IsSectionAllowed(SectionId sectionId) {
            return AvailableSections.Contains(sectionId);
        }

        /// <summary>
        /// Определяет запрещена ли секция
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>true - запрещена, false - разрешена</returns>
        public bool IsSectionForbidden(SectionId sectionId) {
            return !IsSectionAllowed(sectionId);
        }

        public long GetLanguageFromId() {
            LoadLanguages();
            return GetLanguageToId(DefaultUserLanguages.From);
        }

        public long GetLanguageToId() {
            LoadLanguages();
            return GetLanguageToId(DefaultUserLanguages.To);
        }

        /// <summary>
        /// Доступы к платежной системе Robokassa
        /// </summary>
        public RobokassaSecurityParams RobokassaSecurityParams { get; private set; }

        /// <summary>
        /// Определяет разрешена ли покупка материалов для определенной секции
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>true - разрешена, false - запрещена</returns>
        public bool CanBuy(SectionId sectionId) {
            return _salesSections.Contains(sectionId);
        }

        /// <summary>
        /// Получает цену для материала
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <param name="id">идентификатор материала</param>
        /// <param name="name">название материала</param>
        /// <returns>если можно купить материал, то цена материала(может быть null), null - если материал не продается</returns>
        public decimal? GetPrice(SectionId sectionId, long id, string name) {
            bool cannotBuy = CannotBuy(sectionId);
            if (cannotBuy) {
                return null;
            }

            ISalesSettings salesSettings = GetSalesSettings(SectionId.VisualDictionary);
            return salesSettings != null ? salesSettings.GetPrice(id, name) : (decimal?) null;
        }

        /// <summary>
        /// Определяет запрещена ли покупка материалов для определенной секции
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>true - запрещена, false - разрешена</returns>
        public bool CannotBuy(SectionId sectionId) {
            return !CanBuy(sectionId);
        }

        /// <summary>
        /// Получает настройки для продажи определенной секции
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>настройки для продажи</returns>
        public ISalesSettings GetSalesSettings(SectionId sectionId) {
            ISalesSettings result;
            return _salesSettings.TryGetValue(sectionId, out result) ? result : null;
        }

        public RepositoryFactory GetRepositoryFactory() {
            return _repositoryFactory;
        }

        public string PathToTopBanner { get; private set; }

        /// <summary>
        /// Устанавливает отложенные настройки
        /// </summary>
        /// <param name="args">функции по отложенным вычислениям</param>
        public void SetDeferredSettings(params Action<WebSettingsConfig>[] args) {
            const int MAX_ATTEMPTS = 3;

            _deferredActions = args;
            if (EnumerableValidator.IsNullOrEmpty(_deferredActions)) {
                return;
            }

            Task.Run(() => {
                foreach (var action in _deferredActions) {
                    for (int i = 0; i < MAX_ATTEMPTS; i++) {
                        try {
                            action(this);
                            break;
                        } catch (ThreadAbortException e) {} catch (Exception e) {
                            if (i - 1 == MAX_ATTEMPTS) {
                                LoggerWrapper.RemoteMessage(LoggingSource.Mail, LoggingType.Error,
                                                            "WebSettingsConfig.SetDeferredSettings вылетело исключение {0}. Сделали {1} попыток!",
                                                            e, MAX_ATTEMPTS);
                            }
                        }
                    }
                }
            });
        }

        public string WebPath { get; private set; }
        public long MaxIDUserForTopMessage { get; private set; }

        /// <summary>
        /// Получает полные путь относительно папки App_Data
        /// </summary>
        /// <param name="paths">пути имена файлов, которые нужно дописывать к App_Data</param>
        /// <returns>полный путь</returns>
        public string GetDataFileName(params string[] paths) {
            var fullPaths = new string[paths.Length + 1];
            fullPaths[0] = BasePathToWriteData;
            paths.CopyTo(fullPaths, 1);
            return Path.Combine(fullPaths);
        }

        public bool IsAdmin(long userId) {
            return _fullRightUserIds.Contains(userId);
        }

        #endregion

        private void LoadSalesSettings(XElement root) {
            //Доступы к платежным системам
            RobokassaSecurityParams =
                new RobokassaSecurityParams(XmlParseHelper.Get<string>(root, "Payment", "Robokassa", "Login"),
                                            XmlParseHelper.Get<string>(root, "Payment", "Robokassa",
                                                                       "Password1"),
                                            XmlParseHelper.Get<string>(root, "Payment", "Robokassa",
                                                                       "Password2"));
            //секции на которых есть продажа материалов
            _salesSections =
                new HashSet<SectionId>(XmlParseHelper.GetList<SectionId>(root, "Payment", "SectionId"));
            _salesSettings = new Dictionary<SectionId, ISalesSettings>();
            foreach (SectionId salesSection in _salesSections) {
                AddSalesSettings(root, salesSection);
            }
        }

        private void AddSalesSettings(XElement root, SectionId sectionId) {
            string nodeName = sectionId.ToString();
            var defaultPrice = XmlParseHelper.Get<decimal>(root, "Payment",
                                                           "Settings",
                                                           nodeName,
                                                           "DefaultPrice");
            Dictionary<string, decimal> pricesByNames = XmlParseHelper.GetDictionary<string, decimal>(root, "Payment",
                                                                                                      "Settings",
                                                                                                      nodeName,
                                                                                                      "PriceByName");
            Dictionary<long, decimal> pricesByIds = XmlParseHelper.GetDictionary<long, decimal>(root, "Payment",
                                                                                                "Settings",
                                                                                                nodeName,
                                                                                                "PriceById");
            var discount = XmlParseHelper.Get<decimal>(root, "Payment",
                                                       "Settings",
                                                       nodeName,
                                                       "Discount");

            var summDiscountPrice = XmlParseHelper.Get<decimal>(root, "Payment",
                                                                "Settings",
                                                                nodeName,
                                                                "SummDiscountPrice");
            var salesSettings = new SalesSettings(defaultPrice, pricesByNames, pricesByIds)
            {Discount = discount, SummDiscountPrice = summDiscountPrice};
            if (salesSettings.IsInvalid) {
                return;
            }

            _salesSettings.Add(sectionId, salesSettings);
        }

        //NOTE: сделан для тестов
        public static void SetPath(string path) {
            _basePathToWriteData = path;
        }

        private void LoadLanguages() {
            if (DefaultUserLanguages != null) {
                return;
            }
            var languages = new LanguagesQuery(DefaultLanguageFrom,
                                               DefaultLanguageTo);
            DefaultUserLanguages = languages.GetLanguages(null);
        }

        private static long GetLanguageToId(Language language) {
            return language != null ? language.Id : IdValidator.INVALID_ID;
        }

        private void LoadAnotherDomains(XElement root) {
            AnotherDomains = new List<AnotherDomainInfo>();
            foreach (XElement element in root.Elements("AnotherDomain")) {
                var name = XmlParseHelper.ParseAttribute<string>(element, "name");
                var link = XmlParseHelper.ParseAttribute<string>(element, "link");
                var flag = XmlParseHelper.ParseAttribute<string>(element, "flag");
                var anotherDomain = new AnotherDomainInfo(name, link, flag);

                foreach (XElement pageElement in element.Elements("Page")) {
                    var pageSectionId = XmlParseHelper.ParseAttribute<SectionId>(pageElement, "sectionId");
                    var pageUrl = XmlParseHelper.ParseAttribute<string>(pageElement, "url");
                    var pageTitle = XmlParseHelper.ParseAttribute<string>(pageElement, "title");

                    anotherDomain.AddPage(pageSectionId, pageUrl, pageTitle);
                }

                if (anotherDomain.IsValid()
                    && !string.Equals(link, DomainWithProtocol, StringComparison.InvariantCultureIgnoreCase)) {
                    AnotherDomains.Add(anotherDomain);
                }
            }
        }

        private void LoadKeyboardKeys(XElement root) {
            var firstForeignChar = XmlParseHelper.Get<char>(root, "Keyboard", "FirstForeignChar");
            var lastForeignChar = XmlParseHelper.Get<char>(root, "Keyboard", "LastForeignChar");

            var alsoChars = new HashSet<char>(XmlParseHelper.GetList<char>(root, "Keyboard", "AlsoChar"));

            Dictionary<char, char> toRussian = XmlParseHelper.GetDictionary<char, char>(root, "Keyboard", "ToRussian");
            ForeignToRussianKeys = toRussian;
            RussianToForeignKeys = new Dictionary<char, char>();
            foreach (var chars in toRussian) {
                char source = chars.Key;
                char translation = chars.Value;

                if ((source >= firstForeignChar && source <= lastForeignChar && translation >= 'а' && translation <= 'я')
                    || alsoChars.Contains(source) || alsoChars.Contains(translation)) {
                    //нормальная иностранная буква и русская буква
                    RussianToForeignKeys.Add(translation, source);
                }
            }
        }

        private static LanguageShortName GetLanguage(XElement element, string name) {
            return XmlParseHelper.Get<LanguageShortName>(element, name);
        }

        /// <summary>
        /// Получить текст для раздела по шаблону
        /// </summary>
        /// <param name="sectionId">идентификатор раздела</param>
        /// <param name="templateId">идентификатор шаблона</param>
        /// <param name="args">дополнительный шаблон</param>
        /// <returns>текстовые шаблоны</returns>
        public string GetTemplateText(SectionId sectionId, TemplateId templateId, params object[] args) {
            return _templates.Get(sectionId, templateId, args);
        }

        /// <summary>
        /// Получить текст для раздела, страницы по шаблону
        /// </summary>
        /// <param name="sectionId">идентификатор раздела</param>
        /// <param name="pageId">идентификатор страницы</param>
        /// <param name="templateId">идентификатор шаблона</param>
        /// <param name="args">дополнительный шаблон</param>
        /// <returns>текстовые шаблоны</returns>
        public string GetTemplateText(SectionId sectionId, PageId pageId, TemplateId templateId, params object[] args) {
            return _templates.Get(sectionId, pageId, templateId, args);
        }
    }
}