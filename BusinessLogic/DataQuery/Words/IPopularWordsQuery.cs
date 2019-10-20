using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery.Words {
    public interface IPopularWordsQuery {
        /// <summary>
        /// ���������� ����� �� ���� ������������
        /// </summary>
        /// <param name="userLanguages">����</param>
        /// <param name="type">��� ���������� ����</param>
        /// <returns>������ ���� �� ����</returns>
        List<SourceWithTranslation> GetWordsByType(UserLanguages userLanguages, PopularWordType type);

        /// <summary>
        /// ������� ����� �� ���� ������������
        /// </summary>
        /// <param name="source">�����</param>
        /// <param name="translation">�������</param>
        /// <param name="type">��� ������������</param>
        /// <returns>��������� ����� ��� ������, ��� ������</returns>
        SourceWithTranslation GetOrCreate(PronunciationForUser source,
                                          PronunciationForUser translation,
                                          PopularWordType type);
    }
}