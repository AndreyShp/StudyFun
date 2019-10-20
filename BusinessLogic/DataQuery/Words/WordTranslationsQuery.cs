using System.Linq;
using BusinessLogic.Data.Word;

namespace BusinessLogic.DataQuery.Words {
    public class WordTranslationsQuery : BaseQuery, IWordTranslationsQuery, IImageQuery {
        #region IWordTranslationsQuery Members

        /// <summary>
        /// Получает изображение для слова с переводом
        /// </summary>
        /// <param name="id">идентификатор слова с переводом, для которого нужно получить изображение</param>
        /// <returns>массив байт представляющий изображение</returns>
        public byte[] GetImageById(long id) {
            byte[] result = Adapter.ReadByContext(c => {
                IQueryable<WordTranslation> wordTranslationsQuery = (from wt in c.WordTranslation
                                                                     where wt.Id == id
                                                                     select wt);
                WordTranslation wordTranslation = wordTranslationsQuery.FirstOrDefault();
                return wordTranslation != null ? wordTranslation.Image : null;
            });
            return result;
        }

        #endregion
    }
}