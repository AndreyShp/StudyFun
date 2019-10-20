using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Comparison;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Ratings;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Comparisons;
using BusinessLogic.Helpers;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Comparisons {
    public class ComparisonsQuery : BaseQuery, IComparisonsQuery, IRatingQuery {
        private readonly long _languageId;
        private readonly ISentencesQuery _sentencesQuery = new SentencesQuery();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="languageId">идентификатор языка, на котором нужно получать группы правил сравнения</param>
        public ComparisonsQuery(long languageId) {
            _languageId = languageId;
        }

        #region IComparisonsQuery Members

        /// <summary>
        /// Возвращает список видимых групп сравнений
        /// </summary>
        /// <param name="count">кол-во записей, если значение 0 или отрицательное, то все записи</param>
        /// <returns>список видимых групп сравнений</returns>
        public List<ComparisonForUser> GetVisibleWithoutRules(int count = 0) {
            List<ComparisonForUser> result = Adapter.ReadByContext(c => {
                IQueryable<GroupComparison> comparisonsQuery = (from r in c.GroupComparison
                                                                where r.IsVisible && r.LanguageId == _languageId
                                                                orderby r.Rating descending , r.Title
                                                                select r);
                if (count > 0) {
                    comparisonsQuery = comparisonsQuery.Take(count);
                }
                List<ComparisonForUser> innerResult =
                    comparisonsQuery.AsEnumerable().Select(e => new ComparisonForUser(e)).ToList();
                return innerResult;
            });
            return result ?? new List<ComparisonForUser>(0);
        }

        /// <summary>
        /// Получает группу сравнения по названию
        /// </summary>
        /// <param name="userLanguages">языковые настройки пользователя</param>
        /// <param name="title">название представления</param>
        /// <returns>представление или null если не найдено</returns>
        public ComparisonForUser GetWithFullInfo(UserLanguages userLanguages, string title) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;

            ComparisonForUser result = Adapter.ReadByContext(c => {
                var comparisonsQuery = (from gc in c.GroupComparison
                                        join ci in c.ComparisonItem on gc.Id equals ci.GroupComparisonId
                                        join cr in c.ComparisonRule on ci.Id equals cr.ComparisonItemId
                                        join cre in c.ComparisonRuleExample on cr.Id equals cre.ComparisonRuleId
                                        join st in c.SentenceTranslation on cre.SentenceTranslationId equals st.Id
                                        join s1 in c.Sentence on st.SentenceId1 equals s1.Id
                                        join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                        where gc.Title == title && gc.LanguageId == _languageId &&
                                              ((s1.LanguageId == sourceLanguageId
                                                && s2.LanguageId == translationLanguageId)
                                               ||
                                               (s1.LanguageId == translationLanguageId
                                                && s2.LanguageId == sourceLanguageId))
                                        orderby ci.Order , cr.Order , cre.Order
                                        select new {gc, ci, cr, cre, st, s1, s2});
                var comparisonsInfos = comparisonsQuery.AsEnumerable();
                var firstComparison = comparisonsInfos.FirstOrDefault();
                if (firstComparison == null) {
                    return null;
                }
                var innerResult = new ComparisonForUser(firstComparison.gc);
                long prevComparisonItemId = IdValidator.INVALID_ID;
                long prevComparisonRuleId = IdValidator.INVALID_ID;
                ComparisonItemForUser comparisonItemForUser = null;
                ComparisonRuleForUser comparisonRuleForUser = null;
                foreach (var comparisonInfo in comparisonsInfos) {
                    ComparisonItem comparisonItem = comparisonInfo.ci;
                    if (prevComparisonItemId != comparisonItem.Id) {
                        prevComparisonItemId = comparisonItem.Id;

                        if (comparisonItemForUser != null) {
                            innerResult.AddItem(comparisonItemForUser);
                        }

                        comparisonItemForUser = new ComparisonItemForUser(comparisonItem);
                    }

                    ComparisonRule comparisonRule = comparisonInfo.cr;
                    if (comparisonRule.Id != prevComparisonRuleId) {
                        prevComparisonRuleId = comparisonRule.Id;

                        comparisonRuleForUser = new ComparisonRuleForUser(comparisonRule);
                        comparisonItemForUser.AddRule(comparisonRuleForUser);
                    }

                    SourceWithTranslation sourceWithTranslation =
                        ConverterEntities.ConvertToSourceWithTranslation(comparisonInfo.st.Id,
                                                                         comparisonInfo.st.Image,
                                                                         comparisonInfo.s1.LanguageId,
                                                                         comparisonInfo.s1,
                                                                         comparisonInfo.s2);
                    sourceWithTranslation.IsCurrent = false;
                    var example = new ComparisonRuleExampleForUser(comparisonInfo.cre, sourceWithTranslation);
                    comparisonRuleForUser.AddExample(example);
                }

                if (comparisonItemForUser != null) {
                    innerResult.AddItem(comparisonItemForUser);
                }

                return innerResult;
            });
            return result;
        }

        public ComparisonForUser GetOrCreate(ComparisonForUser comparisonForUser) {
            if (!comparisonForUser.IsValid()) {
                return null;
            }

            bool isSuccess = true;
            ComparisonForUser result = null;
            Adapter.ActionByContext(c => {
                GroupComparison groupComparison = GetOrCreateGroupComparison(comparisonForUser, c);
                if (IdValidator.IsInvalid(groupComparison.Id)) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "ComparisonsQuery.GetOrCreate не удалось создать! Название: {0}, описание: {1}",
                        comparisonForUser.Title,
                        comparisonForUser.Description);
                    isSuccess = false;
                    return;
                }
                result = new ComparisonForUser(groupComparison);

                int orderItem = 1;
                foreach (ComparisonItemForUser comparisonItemForUser in comparisonForUser.Items) {
                    ComparisonItem comparisonItem = GetOrCreateComparisonItem(comparisonItemForUser, groupComparison.Id,
                                                                              orderItem++, c);
                    if (IdValidator.IsInvalid(comparisonItem.Id)) {
                        LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                            "ComparisonsQuery.GetOrCreate не удалось создать пункт для сравнения! " +
                            "Id сравнения: {0}, название {1}, перевод названия {2}, описание {3}",
                            groupComparison.Id, comparisonItemForUser.Title, comparisonItemForUser.TitleTranslated,
                            comparisonItemForUser.Description);
                        isSuccess = false;
                        continue;
                    }

                    var newComparisonItemForUser = new ComparisonItemForUser(comparisonItem);
                    result.AddItem(newComparisonItemForUser);

                    int orderRule = 1;
                    foreach (ComparisonRuleForUser comparisonRuleForUser in comparisonItemForUser.Rules) {
                        ComparisonRule comparisonRule = GetOrCreateComparisonRule(comparisonRuleForUser,
                                                                                  comparisonItem.Id, orderRule++, c);
                        long ruleId = comparisonRule.Id;
                        if (IdValidator.IsInvalid(ruleId)) {
                            LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                                "ComparisonsQuery.GetOrCreate не удалось создать правило для сравнения! " +
                                "Id пункта сравнения: {0}, описание {1}",
                                comparisonItem.Id, comparisonRule.Description);
                            isSuccess = false;
                            continue;
                        }

                        var newComparisonRuleForUser = new ComparisonRuleForUser(comparisonRule);
                        newComparisonItemForUser.AddRule(newComparisonRuleForUser);

                        isSuccess = EnumerableValidator.IsNotEmpty(comparisonRuleForUser.Examples)
                                    && CreateExamples(comparisonRuleForUser.Examples, newComparisonRuleForUser, c);
                    }
                }

                if (isSuccess) {
                    //удалить пункты, правила, примеры, которые не были переданы в этот раз
                    DeleteOldInfos(c, result);
                }
            });
            return isSuccess && result != null && result.IsValid() ? result : null;
        }

        #endregion

        #region IRatingQuery Members

        public bool IncRating(long entityId) {
            bool result = Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку обновления и джоинов
                const string SQL_COMMAND = "update GroupComparison set Rating=coalesce(Rating, 0)+1 where Id={0}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             entityId
                                                         });
            });
            return result;
        }

        #endregion

        private static void DeleteOldInfos(StudyLanguageContext c, ComparisonForUser comparisonForUser) {
            if (comparisonForUser == null) {
                return;
            }

            var itemsIds = new List<long>();
            foreach (ComparisonItemForUser item in comparisonForUser.Items) {
                long itemId = item.Id;

                var rulesIds = new List<long>();
                foreach (ComparisonRuleForUser rule in item.Rules) {
                    long ruleId = rule.Id;

                    DeleteRuleExamples(c, ruleId, rule.Examples.Select(e => e.Id).ToList());
                    rulesIds.Add(ruleId);
                }

                DeleteRules(c, itemId, rulesIds);
                itemsIds.Add(itemId);
            }

            DeleteItems(c, comparisonForUser.Id, itemsIds);
        }

        private static void DeleteItems(StudyLanguageContext c, long comparisonId, List<long> itemsIds) {
            IQueryable<ComparisonItem> itemsToDelete =
                c.ComparisonItem.Where(e => e.GroupComparisonId == comparisonId && !itemsIds.Contains(e.Id));
            foreach (ComparisonItem itemToDelete in itemsToDelete) {
                c.ComparisonItem.Remove(itemToDelete);
            }
            c.SaveChanges();
        }

        private static void DeleteRules(StudyLanguageContext c, long itemId, List<long> rulesIds) {
            IQueryable<ComparisonRule> rulesToDelete =
                c.ComparisonRule.Where(e => e.ComparisonItemId == itemId && !rulesIds.Contains(e.Id));
            foreach (ComparisonRule ruleToDelete in rulesToDelete) {
                c.ComparisonRule.Remove(ruleToDelete);
            }
            c.SaveChanges();
        }

        private static void DeleteRuleExamples(StudyLanguageContext c, long ruleId, List<long> examplesIds) {
            IQueryable<ComparisonRuleExample> examplesToDelete =
                c.ComparisonRuleExample.Where(
                    e => e.ComparisonRuleId == ruleId && !examplesIds.Contains(e.Id));
            foreach (ComparisonRuleExample exampleToDelete in examplesToDelete) {
                c.ComparisonRuleExample.Remove(exampleToDelete);
            }
            c.SaveChanges();
        }

        private bool CreateExamples(IEnumerable<ComparisonRuleExampleForUser> ruleExamples,
                                    ComparisonRuleForUser newComparisonRuleForUser,
                                    StudyLanguageContext c) {
            bool result = true;
            int orderRuleExample = 1;
            foreach (ComparisonRuleExampleForUser ruleExample in ruleExamples) {
                SourceWithTranslation example = ruleExample.Example;
                SourceWithTranslation sentenceWithTranslation =
                    _sentencesQuery.GetOrCreate(SentenceType.ComparisonExample,
                                                example.Source,
                                                example.Translation,
                                                null);
                long sentenceTranslationId = sentenceWithTranslation.Id;
                if (IdValidator.IsInvalid(sentenceTranslationId)) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "ComparisonsQuery.GetOrCreate не удалось создать предложения примера! " +
                        "Предложение: {0}, перевод предложения {1}",
                        sentenceWithTranslation.Source.Text, sentenceWithTranslation.Translation.Text);
                    result = false;
                    continue;
                }

                long ruleId = newComparisonRuleForUser.Id;
                ComparisonRuleExample comparisonRuleExample = GetOrCreateComparisonRuleExample(ruleId,
                                                                                               sentenceTranslationId,
                                                                                               ruleExample.Description,
                                                                                               orderRuleExample++, c);

                if (IdValidator.IsInvalid(comparisonRuleExample.Id)) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "ComparisonsQuery.GetOrCreate не удалось создать пример для правила сравнения! " +
                        "Id примера: {0}, sentenceWithTranslationId {1}",
                        ruleId, sentenceTranslationId);
                    result = false;
                    continue;
                }

                var exampleForUser = new ComparisonRuleExampleForUser(comparisonRuleExample, sentenceWithTranslation);
                newComparisonRuleForUser.AddExample(exampleForUser);
            }
            return result;
        }

        private GroupComparison GetOrCreateGroupComparison(ComparisonForUser comparisonForUser,
                                                                  StudyLanguageContext c) {
            GroupComparison groupComparison =
                c.GroupComparison.FirstOrDefault(e => e.Title == comparisonForUser.Title && e.LanguageId == _languageId);
            if (groupComparison == null) {
                groupComparison = new GroupComparison {
                    Title = comparisonForUser.Title,
                    LanguageId = _languageId
                };
                c.GroupComparison.Add(groupComparison);
            }

            groupComparison.AdditionalInfo = comparisonForUser.GetAdditionalInfo();
            groupComparison.Description = comparisonForUser.Description;
            groupComparison.LastModified = DateTime.Now;
            c.SaveChanges();

            return groupComparison;
        }

        private static ComparisonItem GetOrCreateComparisonItem(ComparisonItemForUser comparisonItemForUser,
                                                                long groupComparisonId,
                                                                int order,
                                                                StudyLanguageContext c) {
            ComparisonItem comparisonItem =
                c.ComparisonItem.FirstOrDefault(
                    e => e.GroupComparisonId == groupComparisonId && e.Title == comparisonItemForUser.Title);
            if (comparisonItem == null) {
                comparisonItem = new ComparisonItem {
                    GroupComparisonId = groupComparisonId,
                    Title = comparisonItemForUser.Title
                };
                c.ComparisonItem.Add(comparisonItem);
            }

            comparisonItem.TitleTranslation = comparisonItemForUser.TitleTranslated;
            comparisonItem.Description = comparisonItemForUser.Description;
            comparisonItem.Order = order;
            c.SaveChanges();

            return comparisonItem;
        }

        private static ComparisonRule GetOrCreateComparisonRule(ComparisonRuleForUser comparisonRuleForUser,
                                                                long comparisonItemId,
                                                                int order,
                                                                StudyLanguageContext c) {
            ComparisonRule comparisonRule =
                c.ComparisonRule.FirstOrDefault(
                    e => e.ComparisonItemId == comparisonItemId && e.Description == comparisonRuleForUser.Description);
            if (comparisonRule == null) {
                comparisonRule = new ComparisonRule {
                    ComparisonItemId = comparisonItemId,
                    Description = comparisonRuleForUser.Description
                };
                c.ComparisonRule.Add(comparisonRule);
            }

            comparisonRule.Order = order;
            c.SaveChanges();

            return comparisonRule;
        }

        private static ComparisonRuleExample GetOrCreateComparisonRuleExample(long comparisonRuleId,
                                                                              long sentenceTranslationId,
                                                                              string description,
                                                                              int order,
                                                                              StudyLanguageContext c) {
            ComparisonRuleExample comparisonRuleExample =
                c.ComparisonRuleExample.FirstOrDefault(
                    e => e.ComparisonRuleId == comparisonRuleId && e.SentenceTranslationId == sentenceTranslationId);
            if (comparisonRuleExample == null) {
                comparisonRuleExample = new ComparisonRuleExample {
                    ComparisonRuleId = comparisonRuleId,
                    SentenceTranslationId = sentenceTranslationId
                };
                c.ComparisonRuleExample.Add(comparisonRuleExample);
            }

            comparisonRuleExample.Description = description;
            comparisonRuleExample.Order = order;
            c.SaveChanges();

            return comparisonRuleExample;
        }
    }
}