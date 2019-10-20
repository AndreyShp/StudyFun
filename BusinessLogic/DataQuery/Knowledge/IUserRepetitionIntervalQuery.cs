using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData.Knowledge;

namespace BusinessLogic.DataQuery.Knowledge {
    public interface IUserRepetitionIntervalQuery {
        /// <summary>
        /// ���������� count ������, ������� ����� ���������
        /// </summary>
        /// <param name="sourceLanguageId">������������� ����� � �������� ���������</param>
        /// <param name="translationLanguageId">������������� ����� �� ������� ���������</param>
        /// <param name="count">���-�� �������</param>
        /// <returns>���� ����, �� ������ ������� ����� ���������, ����� ������ �����</returns>
        List<UserRepetitionIntervalItem> GetRepetitionIntervalItems(long sourceLanguageId,
                                                                    long translationLanguageId,
                                                                    int count);

        /// <summary>
        /// ���������� ������ ������
        /// </summary>
        /// <param name="intervalItem">������</param>
        /// <param name="mark">������</param>
        /// <returns>true - ������ ������� ���������, false - ������ �� ������� ���������</returns>
        bool SetMark(UserRepetitionIntervalItem intervalItem, KnowledgeMark mark);

        /// <summary>
        /// ������� ������ ������������, ��� ������� ��� ��� ������ �� �������� �� �������� ������������
        /// </summary>
        /// <returns>true - ������ ������� �������, false - �� ������� ������� ������</returns>
        bool RemoveWithoutData();
    }
}