using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using BusinessLogic.Logger;

namespace BusinessLogic.Helpers.Caches {
    /// <summary>
    /// Дисковый кэш файлов
    /// </summary>
    public class DiskCache {
        private readonly ConcurrentDictionary<string, byte[]> _cache = new ConcurrentDictionary<string, byte[]>();
        private readonly string _path;
        private readonly bool _saveToMemory;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">путь к файлам кэша</param>
        /// <param name="saveToMemory">сохранять ли данные в памяти</param>
        public DiskCache(string path, bool saveToMemory = true) {
            _path = path;
            _saveToMemory = saveToMemory;
        }

        /// <summary>
        /// Получает строку из кэша
        /// </summary>
        /// <param name="key">ключ для получения данных</param>
        /// <param name="encoding">кодировка строки</param>
        /// <returns>если успешно, то массив байтов, иначе null</returns>
        public string Get(string key, Encoding encoding) {
            byte[] bytes = Get(key);
            return bytes != null ? encoding.GetString(bytes) : null;
        }

        /// <summary>
        /// Получает данные из кэша
        /// </summary>
        /// <param name="key">ключ для получения данных</param>
        /// <returns>если успешно, то массив байтов, иначе null</returns>
        public byte[] Get(string key) {
            byte[] result;
            if (_saveToMemory && _cache.TryGetValue(key, out result) && result != null) {
                return result;
            }

            result = SafetyFunc(key, fullPath => File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : null, null);
            if (result != null) {
                AddToCache(key, result);
            }
            return result;
        }

        /// <summary>
        /// Сохраняет данные на диск
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="data">данные</param>
        /// <param name="encoding">кодировка в которой нужно сохранять данные</param>
        /// <returns>true - данные успешно сохранены на диск, false - данные не удалось сохранить</returns>
        public bool Save(string key, string data, Encoding encoding) {
            return Save(key, data != null ? encoding.GetBytes(data) : null);
        }

        /// <summary>
        /// Сохраняет данные на диск
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="data">данные</param>
        /// <returns>true - данные успешно сохранены на диск, false - данные не удалось сохранить</returns>
        public bool Save(string key, byte[] data) {
            if (data == null) {
                return false;
            }

            AddToCache(key, data);
            return SafetyFunc(key, fullPath => {
                CreateDirectoryIfNeed();
                File.WriteAllBytes(fullPath, data);
                return true;
            }, false);
        }

        /// <summary>
        /// Очищает кэш
        /// </summary>
        public void ClearMemory() {
            _cache.Clear();
        }

        private void AddToCache(string key, byte[] innerResult) {
            if (_saveToMemory) {
                _cache.AddOrUpdate(key, innerResult, (k, oldValue) => innerResult);
            }
        }

        private void CreateDirectoryIfNeed() {
            if (!Directory.Exists(_path)) {
                Directory.CreateDirectory(_path);
            }
        }

        private T SafetyFunc<T>(string key, Func<string, T> func, T defaultValue) {
            T result = defaultValue;
            try {
                string fullPath = Path.Combine(_path, key);
                return func(fullPath);
            } catch (Exception e) {
                LoggerWrapper.RemoteMessage(LoggingType.Error, "DiskCache.SafetyFunc Path={0}, key={1}. Exception: {2}",
                                            _path, key, e);
            }
            return result;
        }
    }
}