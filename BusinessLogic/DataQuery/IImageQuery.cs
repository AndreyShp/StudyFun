namespace BusinessLogic.DataQuery {
    public interface IImageQuery {
        /// <summary>
        /// Получает изображение по идентификатору
        /// </summary>
        /// <param name="id">идентификатор изображения</param>
        /// <returns>массив байтов, представляющих изображение</returns>
        byte[] GetImageById(long id);
    }
}