namespace BusinessLogic {
    /// <summary>
    /// Класс реализует паттерн одиночка
    /// </summary>
    /// <typeparam name="T">тип класса, который существует в единственном экземпляре</typeparam>
    public class Singleton<T> where T : class, new() {
        private static readonly object _syncRoot = new object();
        private static volatile T _instance;

        protected Singleton() {}

        public static T Instance {
            get {
                if (_instance == null) {
                    lock (_syncRoot) {
                        if (_instance == null) {
                            _instance = new T();
                            var configurable = _instance as IConfigurable;
                            if (configurable != null) {
                                //класс настраивыемый - настроить
                                configurable.Configure();
                            }
                        }
                    }
                }

                return _instance;
            }
        }
    }
}