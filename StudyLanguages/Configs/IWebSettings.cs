using System;
using System.Collections.Generic;
using BusinessLogic.DataQuery.Money;
using BusinessLogic.DataQuery.UserRepository;
using BusinessLogic.ExternalData;
using BusinessLogic.Helpers.Caches;
using BusinessLogic.PaymentSystems;

namespace StudyLanguages.Configs {
    /// <summary>
    /// Интерфейс для веб-настройки
    /// </summary>
    internal interface IWebSettings {
        /// <summary>
        /// Текущий домен
        /// </summary>
        string Domain { get; }

        /// <summary>
        /// Все доступные секции для текущего домена
        /// </summary>
        HashSet<SectionId> AvailableSections { get; }
        /// <summary>
        /// Кэш для картинок, которые продаем
        /// </summary>
        DiskCache SalesPicturesCache { get; }
        /// <summary>
        /// Общий кэш для файлов на диске
        /// </summary>
        DiskCache CommonDiskCache { get; }

        /// <summary>
        /// Доступы к платежной системе Robokassa
        /// </summary>
        RobokassaSecurityParams RobokassaSecurityParams { get; }
        /// <summary>
        /// Путь к баннеру если он есть
        /// </summary>
        string PathToTopBanner { get; }

        /// <summary>
        /// Определяет разрешена ли секция
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>true - разрешена, false - запрещена</returns>
        bool IsSectionAllowed(SectionId sectionId);

        /// <summary>
        /// Определяет запрещена ли секция
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>true - запрещена, false - разрешена</returns>
        bool IsSectionForbidden(SectionId sectionId);

        long GetLanguageFromId();

        long GetLanguageToId();

        /// <summary>
        /// Определяет разрешена ли покупка материалов для определенной секции
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>true - разрешена, false - запрещена</returns>
        bool CanBuy(SectionId sectionId);

        /// <summary>
        /// Получает цену для материала
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <param name="id">идентификатор материала</param>
        /// <param name="name">название материала</param>
        /// <returns>если можно купить материал, то цена материала(может быть null), null - если материал не продается</returns>
        decimal? GetPrice(SectionId sectionId, long id, string name);

        /// <summary>
        /// Определяет запрещена ли покупка материалов для определенной секции
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>true - запрещена, false - разрешена</returns>
        bool CannotBuy(SectionId sectionId);

        /// <summary>
        /// Получает настройки для продажи определенной секции
        /// </summary>
        /// <param name="sectionId">секция</param>
        /// <returns>настройки для продажи</returns>
        ISalesSettings GetSalesSettings(SectionId sectionId);

        /// <summary>
        /// Получить фабрику репозиториев
        /// </summary>
        /// <returns>фабрика репозиториев</returns>
        RepositoryFactory GetRepositoryFactory();

        /// <summary>
        /// Устанавливает отложенные настройки
        /// </summary>
        /// <param name="args">функции по отложенным вычислениям</param>
        void SetDeferredSettings(params Action<WebSettingsConfig>[] args);

        /// <summary>
        /// Путь к вебу
        /// </summary>
        string WebPath { get; }

        /// <summary>
        /// Максимальный идентификатор пользователя для показа сообщения
        /// </summary>
        long MaxIDUserForTopMessage { get; }

        /// <summary>
        /// Получает полные путь относительно папки App_Data
        /// </summary>
        /// <param name="paths">пути имена файлов, которые нужно дописывать к App_Data</param>
        /// <returns>полный путь</returns>
        string GetDataFileName(params string[] paths);

        /// <summary>
        /// Определяет является ли пользователь администратором
        /// </summary>
        bool IsAdmin(long userId);
    }
}