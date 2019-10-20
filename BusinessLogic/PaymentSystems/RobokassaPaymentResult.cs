namespace BusinessLogic.PaymentSystems {
    /// <summary>
    /// Данные об оплате платежки полученный от Robokassa
    /// </summary>
    public class RobokassaPaymentResult {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="paymentId">идентификатор платежа</param>
        /// <param name="price">сумма платежа</param>
        /// <param name="dirtyPrice">стоимость заплаченная пользователем в том виде в котором пришла к нам</param>
        public RobokassaPaymentResult(int paymentId, decimal price, /*string dirtyPaymentId,*/ string dirtyPrice) {
            PaymentId = paymentId;
            Price = price;
            //DirtyPaymentId = dirtyPaymentId;
            DirtyPrice = dirtyPrice;
        }

        /// <summary>
        /// Идентификатор платежки
        /// </summary>
        public int PaymentId { get; private set; }

        /// <summary>
        /// Стоимость заплаченная пользователем
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// Стоимость заплаченная пользователем в том виде в котором пришла к нам
        /// </summary>
        internal string DirtyPrice { get; private set; }

        /*/// <summary>
        /// Идентификатор платежа в том виде в котором пришел к нам
        /// </summary>
        internal string DirtyPaymentId { get; private set; }*/
    }
}