namespace BusinessLogic.ExternalData.Knowledge {
    public class UserKnowledgeStatistic {
        internal UserKnowledgeStatistic() {}

        /// <summary>
        /// Статистика за сегодня
        /// </summary>
        public CountStatistic Today { get; internal set; }

        /// <summary>
        /// Суммарная статистика
        /// </summary>
        public CountStatistic Total { get; internal set; }

        #region Nested type: CountStatistic

        /// <summary>
        /// Статистика по кол-ву
        /// </summary>
        public class CountStatistic {
            public int CountWords { get; set; }
            public int CountPhrases { get; set; }
            public int CountSentences { get; set; }
            public long Total {
                get { return CountWords + CountPhrases + CountSentences; }
            }
        }

        #endregion
    }
}