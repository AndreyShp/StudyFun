using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace Sandbox.Classes.Translators {
    /// <summary>
    /// Интерфейс, который реализуют классы отвечающие за перевод текста с одного языка на другой
    /// </summary>
    internal interface ITranslator {
        /// <summary>
        /// Имя переводчика
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Переводит текст с одного языка на другой
        /// </summary>
        /// <param name="from">язык с которого переводится</param>
        /// <param name="to">язык на который переводится</param>
        /// <param name="text">текст, который нужно перевести</param>
        /// <returns>список переводов, если нет перевода, то пустой список, в случае ошибки null</returns>
        List<string> Translate(LanguageShortName from, LanguageShortName to, string text);
    }
}