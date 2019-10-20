namespace StudyLanguages.Configs {
    /// <summary>
    /// Идентификатор текстового шаблона
    /// </summary>
    public enum TemplateId {
        /// <summary>
        /// Тег title
        /// </summary>
        Title,

        /// <summary>
        /// Мета-тег Keywords
        /// </summary>
        Keywords,

        /// <summary>
        /// Мета-тег Description
        /// </summary>
        Description,

        /// <summary>
        /// Заголовок на странице
        /// </summary>
        Header,

        /// <summary>
        /// Подсказка для картинки
        /// </summary>
        ImageAlt,

        /// <summary>
        /// Подсказка при наведении на раздел
        /// </summary>
        ThumbnailTip,

        /// <summary>
        /// Подсказка в поле поиска
        /// </summary>
        SearchTip,

        /// <summary>
        /// Короткая подсказка(для раздела "Почувствуй разницу" и для задания пользователя)
        /// </summary>
        ShortDescription,

        /// <summary>
        /// Подсказка при наведении на произношение(для визуального словаря, ручного тренажера слов/фраз)
        /// </summary>
        SpeakTip,

        /// <summary>
        /// Текст на кнопке добавления нового раздела
        /// </summary>
        AddNewBtn,

        #region Для главной страницы

        /// <summary>
        /// Описание визуального словаря(для главной страницы)
        /// </summary>
        VisualDictionaryDescription,

        /// <summary>
        /// Описание моих знаний(для главной страницы)
        /// </summary>
        MyKnowledgeDescription,

        /// <summary>
        /// Описание генератора знаний(для главной страницы)
        /// </summary>
        KnowledgeGeneratorDescription,

        /// <summary>
        /// Описание почувствуй разницу(для главной страницы)
        /// </summary>
        FillDifferenceDescription,

        /// <summary>
        /// Описание слов по темам(для главной страницы)
        /// </summary>
        GroupByWordsDescription,

        /// <summary>
        /// Описание фраз по темам(для главной страницы)
        /// </summary>
        GroupByPhrasesDescription,

        /// <summary>
        /// Описание видео(для главной страницы)
        /// </summary>
        VideoDescription,

        /// <summary>
        /// Описание предложений(для главной страницы)
        /// </summary>
        SentencesDescription,

        /// <summary>
        /// Описание аудио(для главной страницы)
        /// </summary>
        AudioDescription,

        /// <summary>
        /// Описание перевода слов(для главной страницы)
        /// </summary>
        WordTranslationDescription,

        /// <summary>
        /// Описание перевода фраз(для главной страницы)
        /// </summary>
        PhrasalVerbsTranslationDescription,

        /// <summary>
        /// Подсказка на главную страницу для наведения на наиболее часто употребляемый элемент
        /// </summary>
        ItemTipOnMainPage,

        /// <summary>
        /// Описание минилекса(для главной страницы)
        /// </summary>
        PopularWordDescription,

        /// <summary>
        /// Описание заданий
        /// </summary>
        UserTasksDescription

        #endregion
    }
}