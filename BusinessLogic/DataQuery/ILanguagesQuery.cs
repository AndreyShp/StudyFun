using System.Collections.Generic;
using BusinessLogic.Data;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery {
    public interface ILanguagesQuery {
        UserLanguages GetLanguages(List<long> preferLanguagesIds);
        UserLanguages GetLanguagesByShortNames(LanguageShortName source, LanguageShortName translation);

        Language GetById(long id);
        Language GetByShortName(LanguageShortName shortName);
        LanguageShortName GetShortNameById(long id);
    }
}