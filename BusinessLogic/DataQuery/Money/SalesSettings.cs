using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.DataQuery.Money {
    /// <summary>
    /// Определяет цену за товары
    /// </summary>
    public class SalesSettings : ISalesSettings {
        private readonly decimal _defaultPrice;
        private readonly Dictionary<long, decimal> _pricesByIds;
        private readonly Dictionary<string, decimal> _pricesByNames;
        private readonly decimal _specialPrice;

        public SalesSettings(decimal defaultPrice, Dictionary<long, decimal> pricesByIds)
            : this(defaultPrice, null, pricesByIds) {}

        public SalesSettings(decimal defaultPrice, Dictionary<string, decimal> pricesByNames)
            : this(defaultPrice, pricesByNames, null) {}

        public SalesSettings(decimal defaultPrice,
                             Dictionary<string, decimal> pricesByNames,
                             Dictionary<long, decimal> pricesByIds) {
            _defaultPrice = defaultPrice;
            _pricesByNames = pricesByNames != null
                                 ? pricesByNames.ToDictionary(e => GetKeyByName(e.Key), e => e.Value)
                                 : new Dictionary<string, decimal>(0);
            _pricesByIds = pricesByIds ?? new Dictionary<long, decimal>(0);
            _specialPrice = _pricesByIds.Sum(e => e.Value) + _pricesByNames.Sum(e => e.Value);
        }

        #region ISalesSettings Members

        /// <summary>
        /// Получить цену
        /// </summary>
        /// <param name="id">идентификатор товара для продажи</param>
        /// <param name="name">название товара для продажи</param>
        /// <returns>цена за товар</returns>
        public decimal GetPrice(long id, string name) {
            decimal result;
            if (_pricesByIds.TryGetValue(id, out result) || _pricesByNames.TryGetValue(GetKeyByName(name), out result)) {
                return result;
            }

            return _defaultPrice;
        }

        /// <summary>
        /// Скидка
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Суммарная цена со скидкой
        /// </summary>
        public decimal SummDiscountPrice { get; set; }

        /// <summary>
        /// Проверяет корректные ли настройки
        /// </summary>
        public bool IsInvalid {
            get { return _specialPrice <= 0 && _defaultPrice <= 0 && SummDiscountPrice <= 0; }
        }

        #endregion

        private static string GetKeyByName(string name) {
            return name.Trim().ToLowerInvariant();
        }
    }
}