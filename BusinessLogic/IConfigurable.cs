namespace BusinessLogic {
    /// <summary>
    /// Интерфейс для классов, которые могут быть настраивыми
    /// </summary>
    public interface IConfigurable {
        /// <summary>
        /// Вызывается для настройки класса
        /// </summary>
        void Configure();
    }
}