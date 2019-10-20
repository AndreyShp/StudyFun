using System;
using System.Collections.Generic;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.ExternalData.Knowledge;

namespace BusinessLogic.DataQuery.Knowledge {
    public interface IUserKnowledgeQuery {
        /// <summary>
        /// �������� ������ ������������
        /// </summary>
        /// <param name="sourceLanguageId">������������� �����, � �������� ���������</param>
        /// <param name="translationLanguageId">������������� �����, �� ������� ���������</param>
        /// <param name="status">������ ������, ������� ����� ��������</param>
        /// <param name="prevId">������������ ������������� ���������� ������</param>
        /// <param name="count">���-�� ������� ��� ���������</param>
        /// <returns></returns>
        List<UserKnowledgeItem> GetData(long sourceLanguageId,
                                        long translationLanguageId,
                                        KnowledgeStatus status,
                                        long prevId,
                                        int count);

        /// <summary>
        /// ��������� "����� ������ ������" ������������
        /// </summary>
        /// <param name="knowledgeItems">����� ������ ������</param>
        /// <param name="maxCountItemsPerDay">������������ ���-�� ������� � ���� �� ����������</param>
        /// <returns>������ �������� ���������� �������, ��� ������� ������ ���� ������</returns>
        List<KnowledgeAddStatus> Add(List<UserKnowledgeItem> knowledgeItems, int maxCountItemsPerDay);

        /*/// <summary>
        /// �������� ������� ��� ������ �� �� ���������������
        /// </summary>
        /// <param name="userId">������������� ������������</param>
        /// <param name="dataType">��� ������</param>
        /// <param name="dataIds">�������������� ������, ��� ������� ����� �������� ������</param>
        /// <returns></returns>
        Dictionary<long, Status> GetStatusesByDataIds(long userId, DataType dataType, List<long> dataIds);*/

        /// <summary>
        /// ���������� �������� �� ������ ������������� ��� ����������
        /// </summary>
        /// <param name="knowledgeItem">������ �� ��������</param>
        /// <returns>true - ������ �����������, false - ������ ���������</returns>
        bool IsInvalid(UserKnowledgeItem knowledgeItem);

        /// <summary>
        /// ������� ������
        /// </summary>
        /// <param name="id">������������� ������, ������� ����� �������</param>
        /// <returns>true - ������ �������, ����� false</returns>
        bool Remove(long id);

        /// <summary>
        /// ��������������� ������
        /// </summary>
        /// <param name="id">������������� ������, ������� ����� ������������</param>
        /// <returns>true - ������ �������������, ����� false</returns>
        bool Restore(long id);

        /// <summary>
        /// �������� ���������� ������������
        /// </summary>
        /// <returns>���������� ������������</returns>
        UserKnowledgeStatistic GetStatistic();

        /// <summary>
        /// ��������� ������ � ���������������� ������ �� ���� ����������� �����������
        /// </summary>
        /// <param name="sourceLanguageId">������������� ����� � �������� ���������</param>
        /// <param name="translationLanguageId">������������� ����� �� ������� ���������</param>
        /// <param name="userKnowledges">������</param>
        /// <returns>������ ������������</returns>
        List<UserKnowledgeItem> ConvertEntitiesToItems(long sourceLanguageId,
                                                       long translationLanguageId,
                                                       IEnumerable<UserKnowledge> userKnowledges);

        /// <summary>
        /// ���������� ��������������, ������� ���� � ������������ � �������
        /// </summary>
        /// <param name="ids">��������������, ��� ������� ����� ���������� ������������� ������</param>
        /// <param name="dataType">��� ������, ��� ������� �������� ��������������</param>
        /// <returns>�������������� ������, ������� ���� � ������������ � �������</returns>
        List<long> GetExistenceIds(List<long> ids, KnowledgeDataType dataType);

        /// <summary>
        /// ������� ������ ������������, ������� ���� ������� ������������� - ����� ������� ����
        /// </summary>
        /// <returns>true - ������ �������, false - ������ �� ������� �������</returns>
        bool RemoveDeleted();
    }
}