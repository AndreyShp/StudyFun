using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.Data.Auxiliary;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.Logger;
using BusinessLogic.Validators;
using ExternalCrossReference = BusinessLogic.ExternalData.Auxiliaries.CrossReference;

namespace BusinessLogic.DataQuery.Auxiliaries {
    public class CrossReferencesQuery : BaseQuery, ICrossReferencesQuery {
        private readonly Dictionary<CrossReferenceType, IdNameCache> _cachesByCrossReferenceTypes =
            new Dictionary<CrossReferenceType, IdNameCache>();
        private readonly IGroupsQuery _groupsQuery;
        private readonly IRepresentationsQuery _representationsQuery;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="languageId">идентификатор языка, для которого нужно получать данные</param>
        public CrossReferencesQuery(long languageId) {
            _groupsQuery = new GroupsQuery(languageId);
            _representationsQuery = new RepresentationsQuery(languageId);

            AddIdNameCacheByCrossType(CrossReferenceType.GroupWord);
            AddIdNameCacheByCrossType(CrossReferenceType.GroupSentence);
            AddIdNameCacheByCrossType(CrossReferenceType.VisualDictionary);
        }

        #region ICrossReferencesQuery Members

        public long Add(string source,
                        CrossReferenceType sourceType,
                        string destination,
                        CrossReferenceType destinationType) {
            long sourceId = GetIdByName(source, sourceType);
            long destinationId = GetIdByName(destination, destinationType);
            if (IdValidator.IsInvalid(sourceId) || IdValidator.IsInvalid(destinationId)) {
                return IdValidator.INVALID_ID;
            }
            if (sourceId == destinationId
                && string.Equals(source, destination, StringComparison.InvariantCultureIgnoreCase)) {
                //саму на себя ссылку нельзя добавлять
                return IdValidator.INVALID_ID;
            }

            long result = IdValidator.INVALID_ID;
            var parsedSourceType = (int) sourceType;
            var parsedDestinationType = (int) destinationType;
            Adapter.ActionByContext(c => {
                CrossReference crossReference =
                    c.CrossReference.FirstOrDefault(
                        e =>
                        e.SourceId == sourceId && e.SourceType == parsedSourceType && e.DestinationId == destinationId
                        && e.DestinationType == parsedDestinationType);
                if (crossReference != null) {
                    result = crossReference.Id;
                    return;
                }
                crossReference = new CrossReference {
                    SourceId = sourceId,
                    SourceType = parsedSourceType,
                    DestinationId = destinationId,
                    DestinationType = parsedDestinationType
                };
                c.CrossReference.Add(crossReference);
                c.SaveChanges();

                if (IdValidator.IsInvalid(crossReference.Id)) {
                    return;
                }
                result = crossReference.Id;
            });
            return result;
        }

        public List<ExternalCrossReference> GetReferences(long sourceId, CrossReferenceType sourceType) {
            var parsedSourceType = (int) sourceType;
            List<CrossReference> crossReferences =
                Adapter.ReadByContext(
                    c =>
                    c.CrossReference.Where(e => e.SourceId == sourceId && e.SourceType == parsedSourceType).ToList());
            var result = new List<ExternalCrossReference>();
            foreach (CrossReference crossReference in crossReferences) {
                var crossReferenceType = (CrossReferenceType) crossReference.DestinationType;
                long id = crossReference.DestinationId;
                string name = GetNameById(id, crossReferenceType);
                if (!string.IsNullOrEmpty(name)) {
                    result.Add(new ExternalCrossReference(id, name, crossReferenceType));
                } else {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "CrossReferencesQuery.GetReferences не удалось найти перекрестную ссылку с идентификатором {0} и типом {1} для источника с идентификатором {2}, типом {3}!",
                        id, crossReferenceType, sourceId, sourceType);
                }
            }
            return result;
        }

        public bool RemoveAllExceptIds(HashSet<long> ids) {
            return Adapter.ReadByContext(c => {
                DbSet<CrossReference> crossReferences = c.CrossReference;
                foreach (CrossReference crossReference in crossReferences) {
                    long id = crossReference.Id;
                    if (!ids.Contains(id)) {
                        c.CrossReference.Remove(crossReference);
                    }
                }
                c.SaveChanges();
                return true;
            });
        }

        #endregion

        private void AddIdNameCacheByCrossType(CrossReferenceType type) {
            Func<IEnumerable<Tuple<long, string>>> dataGetter;
            switch (type) {
                case CrossReferenceType.VisualDictionary:
                    dataGetter = () => {
                        List<RepresentationForUser> areas = _representationsQuery.GetVisibleWithoutAreas();
                        return areas.Select(e => new Tuple<long, string>(e.Id, e.Title));
                    };
                    break;
                case CrossReferenceType.GroupWord:
                    dataGetter = () => GetGroupsByType(GroupType.ByWord);
                    break;
                case CrossReferenceType.GroupSentence:
                    dataGetter = () => GetGroupsByType(GroupType.BySentence);
                    break;
                default:
                    throw new ArgumentException("Неизвестный тип " + type);
            }

            var idNameCache = new IdNameCache(dataGetter);
            _cachesByCrossReferenceTypes.Add(type, idNameCache);
        }

        private IEnumerable<Tuple<long, string>> GetGroupsByType(GroupType groupType) {
            List<GroupForUser> areas = _groupsQuery.GetVisibleGroups(groupType);
            return areas.Select(e => new Tuple<long, string>(e.Id, e.Name));
        }

        private long GetIdByName(string name, CrossReferenceType type) {
            IdNameCache idNameCache = GetCacheByCrossType(type);
            return idNameCache != null ? idNameCache.GetId(name) : IdValidator.INVALID_ID;
        }

        private string GetNameById(long id, CrossReferenceType type) {
            IdNameCache idNameCache = GetCacheByCrossType(type);
            return idNameCache != null ? idNameCache.GetName(id) : null;
        }

        private IdNameCache GetCacheByCrossType(CrossReferenceType type) {
            IdNameCache idNameCache;
            if (_cachesByCrossReferenceTypes.TryGetValue(type, out idNameCache)) {
                return idNameCache;
            }
            LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                "CrossReferencesQuery.GetCacheByCrossType не удалось найти кэш для типа {0}!", type);
            return null;
        }
    }
}