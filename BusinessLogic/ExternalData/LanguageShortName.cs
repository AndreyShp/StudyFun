namespace BusinessLogic.ExternalData {
    /// <summary>
    /// Короткие названия для языков
    /// </summary>
    public enum LanguageShortName {
        //NOTE: не менять названия перечислений, т.к. нужно будет менять названия в БД и в конфиге

        /// <summary>
        /// Заглушка на отсутствие языка
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Английский
        /// </summary>
        En = 1,

        /// <summary>
        /// Немецкий
        /// </summary>
        De = 100,

        /// <summary>
        /// Французский
        /// </summary>
        Fr = 101,

        /// <summary>
        /// Итальянский
        /// </summary>
        It = 102,

        /// <summary>
        /// Испанский
        /// </summary>
        Es = 103,

        /// <summary>
        /// Польский
        /// </summary>
        Pl = 104,

        /// <summary>
        /// Португальский
        /// </summary>
        Pt = 105,

        /// <summary>
        /// Турецкий
        /// </summary>
        Tr = 106,

        /// <summary>
        /// Финнский
        /// </summary>
        Fi = 107,

        /// <summary>
        /// Чешский
        /// </summary>
        Cs = 108,

        /// <summary>
        /// Венгерский
        /// </summary>
        Hu = 109,

        /// <summary>
        /// Болгарский 	
        /// </summary>
        Bg = 110,

        /// <summary>
        /// Украинский
        /// </summary>
        Uk = 111,

        /// <summary>
        /// Голландский
        /// </summary>
        Nl = 112,

        /*/// <summary>
        /// Сербский
        /// </summary>
        Sr = 105,*/

        /// <summary>
        /// Русский
        /// </summary>
        Ru = 1000,

        /*/// <summary>
        /// Беларусский
        /// </summary>
        Be = 1002*/
    }
}