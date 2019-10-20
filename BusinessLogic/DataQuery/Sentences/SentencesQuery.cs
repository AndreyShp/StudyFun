using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Sentence;
using BusinessLogic.ExternalData;
using BusinessLogic.Helpers;
using BusinessLogic.Helpers.Speakers;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Sentences {
    public class SentencesQuery : BaseRandomQuery, ISentencesQuery, IImageQuery, ICleaner {
        private readonly SpeakerFactory _speakerFactory = new SpeakerFactory();
        
        #region IImageQuery Members

        /// <summary>
        /// Получает изображение для предложения с переводом
        /// </summary>
        /// <param name="id">идентификатор предложения с переводом, для которого нужно получить изображение</param>
        /// <returns>массив байт представляющий изображение</returns>
        public byte[] GetImageById(long id) {
            byte[] result = Adapter.ReadByContext(c => {
                IQueryable<SentenceTranslation> sentenceTranslationsQuery = (from st in c.SentenceTranslation
                                                                             where st.Id == id
                                                                             select st);
                SentenceTranslation sentenceTranslation = sentenceTranslationsQuery.FirstOrDefault();
                return sentenceTranslation != null ? sentenceTranslation.Image : null;
            });
            return result;
        }

        #endregion

        #region ISentencesQuery Members

        public IPronunciation GetById(long id) {
            Sentence result = Adapter.ReadByContext(c => {
                IQueryable<Sentence> sentencesQuery = (from s in c.Sentence
                                                       where s.Id == id
                                                       select s);
                return sentencesQuery.FirstOrDefault();
            });
            return result;
        }

        public void FillSpeak(long languageId) {
            ISpeaker speaker = _speakerFactory.Create(languageId);
            Adapter.ActionByContext(c => {
                IQueryable<Sentence> sentencesQuery = (from s in c.Sentence
                                                       where s.LanguageId == languageId && s.Pronunciation == null
                                                       select s);
                List<Sentence> sentences = sentencesQuery.ToList();
                foreach (Sentence sentence in sentences) {
                    sentence.Pronunciation = speaker.ConvertTextToAudio(sentence.Text);
                    c.SaveChanges();
                }
            });
        }

        public SourceWithTranslation GetOrCreate(SentenceType type,
                                                 PronunciationForUser source,
                                                 PronunciationForUser translation,
                                                 byte[] image,
                                                 int? rating = null) {
            //TODO: проверять корректность параметров
            SourceWithTranslation result = null;
            Adapter.ActionByContext(context => {
                SentenceTranslation sentenceTranslation = GetSentenceTranslation(context, source, translation);
                if (sentenceTranslation != null) {
                    //обновить возможно изменившиеся поля
                    SetSentenceTranslation(sentenceTranslation, type, image, rating);
                    context.SaveChanges();
                    if (IdValidator.IsValid(sentenceTranslation.Id)) {
                        result = new SourceWithTranslation();
                        result.Set(sentenceTranslation.Id, source, translation);
                    }
                } else {
                    result = CreateSentencencesWithTranslation(type, source, translation, image, rating);
                }
            });
            return result;
        }

        public long GetOrCreate(long languageId, string text) {
            long result = IdValidator.INVALID_ID;
            Adapter.ActionByContext(context => {
                Sentence sentence = GetOrAddSentenceToContext(languageId, text, context);
                context.SaveChanges();
                result = sentence.Id;
            });
            return result;
        }

        public List<SourceWithTranslation> GetByCount(UserLanguages userLanguages, SentenceType type, int count) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            var convertedType = (int) type;
            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                var joinedSequence = (from s1 in c.Sentence
                                      join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                      join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                      where (st.Type & convertedType) == convertedType &&
                                            ((s1.LanguageId == sourceLanguageId
                                              && s2.LanguageId == translationLanguageId)
                                             ||
                                             (s1.LanguageId == translationLanguageId
                                              && s2.LanguageId == sourceLanguageId))
                                      select new {st.Id, st.Image, s1, s2}).Take(count);
                return
                    joinedSequence.AsEnumerable().Select(
                        e => ConverterEntities.ConvertToSourceWithTranslation(e.Id, e.Image, sourceLanguageId, e.s1, e.s2)).
                        ToList();
            });
            return result;
        }

        public Dictionary<long, SourceWithTranslation> GetByTranslationsIds(List<long> sentencesTrandlationsIds,
                                                                long sourceLanguageId,
                                                                long translationLanguageId) {
            return Adapter.ReadByContext(c => {
                var joinedSequence = (from s1 in c.Sentence
                                      join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                      join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                      where (sentencesTrandlationsIds.Contains(st.Id) &&
                                             ((s1.LanguageId == sourceLanguageId
                                               && s2.LanguageId == translationLanguageId)
                                              ||
                                              (s1.LanguageId == translationLanguageId
                                               && s2.LanguageId == sourceLanguageId)))
                                      select new {st.Id, st.Image, s1, s2});
                return
                    joinedSequence.AsEnumerable().ToDictionary(e => e.Id,
                        e => ConverterEntities.ConvertToSourceWithTranslation(e.Id, e.Image, sourceLanguageId, e.s1, e.s2));
            });
        }

        #endregion

        /// <summary>
        /// Получает count случайных предложений с переводом для текущего пользователя
        /// </summary>
        /// <param name="userId">уникальный идентификатор пользователя</param>
        /// <param name="sourceLanguageId">язык, с которого нужно переводить</param>
        /// <param name="translationLanguageId">язык, на который нужно переводить</param>
        /// <param name="count">кол-во предложений, которое необходимо получить</param>
        /// <returns>>список случайных предложений</returns>
        protected override List<SourceWithTranslation> GetSourceWithTranslations(long userId,
                                                                                 long sourceLanguageId,
                                                                                 long translationLanguageId,
                                                                                 int count) {
            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                var joinedSequence = (from s1 in c.Sentence
                                      join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                      join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                      join ss in c.ShuffleSentence on st.Id equals ss.SentenceTranslationId
                                      where (ss.UserId == userId && ss.IsShown == false &&
                                             ((s1.LanguageId == sourceLanguageId
                                               && s2.LanguageId == translationLanguageId)
                                              ||
                                              (s1.LanguageId == translationLanguageId
                                               && s2.LanguageId == sourceLanguageId)))
                                      orderby ss.Id ascending
                                      select new {st.Id, st.Image, ss, s1, s2}).Take(count);
                return
                    joinedSequence.AsEnumerable().OrderBy(e => e.ss.Id).Select(
                        e => ConverterEntities.ConvertToSourceWithTranslation(e.Id, e.Image, sourceLanguageId, e.s1, e.s2)).
                        ToList();
            });
            return result;
        }

        protected override Tuple<long, byte[], PronunciationEntity, PronunciationEntity> GetBySourceAndTranslation(
            long sourceId, long translationId) {
            Tuple<long, byte[], PronunciationEntity, PronunciationEntity> result =
                Adapter.ReadByContext(
                    c => {
                        var sourceWithTranslation = (from s1 in c.Sentence
                                                     join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                                     join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                                     where ((st.SentenceId1 == sourceId
                                                             && st.SentenceId2 == translationId)
                                                            ||
                                                            (st.SentenceId2 == sourceId
                                                             && st.SentenceId1 == translationId))
                                                     select new {st.Id, st.Image, s1, s2}).FirstOrDefault();
                        return sourceWithTranslation != null
                                   ? new Tuple<long, byte[], PronunciationEntity, PronunciationEntity>(
                                         sourceWithTranslation.Id,
                                         sourceWithTranslation.Image,
                                         sourceWithTranslation.s1,
                                         sourceWithTranslation.s2)
                                   : new Tuple<long, byte[], PronunciationEntity, PronunciationEntity>(IdValidator.INVALID_ID, null,
                                                                                               null, null);
                    });
            return result;
        }

        protected override long GetMaxId(long userId, long sourceLanguageId, long translationLanguageId) {
            return Adapter.ReadByContext(c => (from s1 in c.Sentence
                                               join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                               join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                               join ss in c.ShuffleSentence on st.Id equals ss.SentenceTranslationId
                                               where (ss.UserId == userId &&
                                                      ((s1.LanguageId == sourceLanguageId
                                                        && s2.LanguageId == translationLanguageId)
                                                       ||
                                                       (s1.LanguageId == translationLanguageId
                                                        && s2.LanguageId == sourceLanguageId)))
                                               select st.Id).Max(), IdValidator.INVALID_ID);
        }

        protected override List<SourceWithTranslation> GetPrevById(long userId,
                                                                   long id,
                                                                   long sourceLanguageId,
                                                                   long translationLanguageId,
                                                                   int count) {
            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                long shuffleId = GetShuffleIdById(userId, id, c);
                if (IdValidator.IsInvalid(shuffleId)) {
                    return new List<SourceWithTranslation>(0);
                }

                var joinedSequence = (from s1 in c.Sentence
                                      join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                      join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                      join ss in c.ShuffleSentence on st.Id equals ss.SentenceTranslationId
                                      where (ss.UserId == userId && ss.Id < shuffleId &&
                                             ((s1.LanguageId == sourceLanguageId
                                               && s2.LanguageId == translationLanguageId)
                                              ||
                                              (s1.LanguageId == translationLanguageId
                                               && s2.LanguageId == sourceLanguageId)))
                                      orderby ss.Id descending
                                      select new {st.Id, st.Image, ss, s1, s2}).Take(count);
                return
                    joinedSequence.AsEnumerable().OrderBy(e => e.ss.Id).Select(
                        e => ConverterEntities.ConvertToSourceWithTranslation(e.Id, e.Image, sourceLanguageId, e.s1, e.s2)).
                        ToList();
            });
            return result;
        }

        private static long GetShuffleIdById(long userId, long sentenceTranslationId, StudyLanguageContext c) {
            ShuffleSentence shuffle =
                c.ShuffleSentence.FirstOrDefault(
                    e => e.UserId == userId && e.SentenceTranslationId == sentenceTranslationId);
            return shuffle != null ? shuffle.Id : IdValidator.INVALID_ID;
        }

        protected override List<SourceWithTranslation> GetNextById(long userId,
                                                                   long id,
                                                                   long sourceLanguageId,
                                                                   long translationLanguageId,
                                                                   int count) {
            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                long shuffleId = GetShuffleIdById(userId, id, c);
                if (IdValidator.IsInvalid(shuffleId)) {
                    return new List<SourceWithTranslation>(0);
                }

                var joinedSequence = (from s1 in c.Sentence
                                      join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                      join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                      join ss in c.ShuffleSentence on st.Id equals ss.SentenceTranslationId
                                      where (ss.UserId == userId && ss.Id > shuffleId &&
                                             ((s1.LanguageId == sourceLanguageId
                                               && s2.LanguageId == translationLanguageId)
                                              ||
                                              (s1.LanguageId == translationLanguageId
                                               && s2.LanguageId == sourceLanguageId)))
                                      orderby ss.Id ascending
                                      select new {st.Id, st.Image, ss, s1, s2}).Take(count);
                return
                    joinedSequence.AsEnumerable().OrderBy(e => e.ss.Id).Select(
                        e => ConverterEntities.ConvertToSourceWithTranslation(e.Id, e.Image, sourceLanguageId, e.s1, e.s2)).
                        ToList();
            });
            return result;
        }

        protected override SourceWithTranslation GetBySourceAndTranslation(long userId,
                                                                           long sourceId,
                                                                           long translationId) {
            SourceWithTranslation result = Adapter.ReadByContext(c => {
                var sourceWithTranslation = (from s1 in c.Sentence
                                             join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                             join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                             join ss in c.ShuffleSentence on st.Id equals ss.SentenceTranslationId
                                             where (ss.UserId == userId &&
                                                    ((st.SentenceId1 == sourceId
                                                      && st.SentenceId2 == translationId)
                                                     ||
                                                     (st.SentenceId2 == sourceId
                                                      && st.SentenceId1 == translationId)))
                                             select new {st.Id, st.Image, s1, s2}).FirstOrDefault();

                if (sourceWithTranslation == null) {
                    return null;
                }

                long sourceLanguageId = sourceWithTranslation.s1.Id == sourceId
                                            ? sourceWithTranslation.s1.LanguageId
                                            : sourceWithTranslation.s2.LanguageId;
                return ConverterEntities.ConvertToSourceWithTranslation(sourceWithTranslation.Id,
                                                                        sourceWithTranslation.Image,
                                                                        sourceLanguageId,
                                                                        sourceWithTranslation.s1,
                                                                        sourceWithTranslation.s2);
            });
            return result;
        }

        /// <summary>
        /// Метод добавляет в таблицу ShuffleSentence count случайных предложений с переводом
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="sourceLanguageId">язык, с которого нужно переводить</param>
        /// <param name="translationLanguageId">язык, на который нужно переводить</param>
        /// <param name="count">кол-во предложений, которые необходимо добавить</param>
        /// <returns>уникальное значение, которое присвоенно всем только, что вставленным записям</returns>
        protected override bool InsertShuffle(long userId,
                                              long sourceLanguageId,
                                              long translationLanguageId,
                                              int count) {
            bool result = Adapter.ExecuteStoredProcedure("insert_shuffle_sentences", (int) SentenceType.Separate,
                                                         userId, sourceLanguageId,
                                                         translationLanguageId, count);
            if (!result) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "SentencesQuery.InsertShuffle для пользователя {0} не удалось вставить новые предложения. Язык источник {1}, язык для перевода {2}, кол-во вставляемых предложений {3}",
                    userId, sourceLanguageId, translationLanguageId, count);
            }
            return result;
        }

        private static SentenceTranslation GetSentenceTranslation(StudyLanguageContext context,
                                                                  PronunciationForUser sourceSentence,
                                                                  PronunciationForUser translationSentence) {
            IQueryable<SentenceTranslation> sentencesWithTranslationsQuery = from s1 in context.Sentence
                                                                             join st in context.SentenceTranslation on
                                                                                 s1.Id equals st.SentenceId1
                                                                             join s2 in context.Sentence on
                                                                                 st.SentenceId2 equals s2.Id
                                                                             where
                                                                                 ((s1.LanguageId
                                                                                   == sourceSentence.LanguageId
                                                                                   && s1.Text == sourceSentence.Text
                                                                                   &&
                                                                                   s2.LanguageId
                                                                                   == translationSentence.LanguageId
                                                                                   &&
                                                                                   s2.Text == translationSentence.Text)
                                                                                  ||
                                                                                  (s1.LanguageId
                                                                                   == translationSentence.LanguageId
                                                                                   &&
                                                                                   s1.Text == translationSentence.Text
                                                                                   &&
                                                                                   s2.LanguageId
                                                                                   == sourceSentence.LanguageId
                                                                                   && s2.Text == sourceSentence.Text))
                                                                             select st;
            SentenceTranslation sentenceTranslation = sentencesWithTranslationsQuery.FirstOrDefault();
            return sentenceTranslation;
        }

        private SourceWithTranslation CreateSentencencesWithTranslation(SentenceType type,
                                                                        PronunciationForUser source,
                                                                        PronunciationForUser translation,
                                                                        byte[] image = null,
                                                                        int? rating = null) {
            SourceWithTranslation result = null;
            Adapter.ActionByContext(context => {
                Sentence sourceSentence = GetOrAddSentenceToContext(source.LanguageId, source.Text, context);
                Sentence translationSentence = GetOrAddSentenceToContext(translation.LanguageId, translation.Text,
                                                                         context);
                context.SaveChanges();
                if (IdValidator.IsInvalid(sourceSentence.Id) || IdValidator.IsInvalid(translationSentence.Id)) {
                    return;
                }
                var sentenceTranslation = new SentenceTranslation
                {SentenceId1 = sourceSentence.Id, SentenceId2 = translationSentence.Id};
                SetSentenceTranslation(sentenceTranslation, type, image, rating);
                context.SentenceTranslation.Add(sentenceTranslation);
                context.SaveChanges();
                if (IdValidator.IsInvalid(sentenceTranslation.Id)) {
                    return;
                }
                result = ConverterEntities.ConvertToSourceWithTranslation(sentenceTranslation.Id, sentenceTranslation.Image, source.LanguageId,
                                                                          sourceSentence,
                                                                          translationSentence);
            });
            return result;
        }

        private static void SetSentenceTranslation(SentenceTranslation sentenceTranslation,
                                                   SentenceType type,
                                                   byte[] image,
                                                   int? rating) {
            sentenceTranslation.Image = image;
            sentenceTranslation.Rating = rating;
            sentenceTranslation.Type |= (int) type;
        }

        private Sentence GetOrAddSentenceToContext(long languageId, string text, StudyLanguageContext context) {
            string trimmedText = text.Trim();
            IQueryable<Sentence> sentenceQuery = from s in context.Sentence
                                                 where s.LanguageId == languageId && s.Text == trimmedText
                                                 select s;
            Sentence result = sentenceQuery.FirstOrDefault();
            if (result == null) {
                ISpeaker speaker = _speakerFactory.Create(languageId);
                result = new Sentence
                {LanguageId = languageId, Text = trimmedText, Pronunciation = speaker.ConvertTextToAudio(trimmedText)};
                context.Sentence.Add(result);
            }
            return result;
        }

        public override bool MarkAsShowed(long userId,
                                          UserLanguages userLanguages,
                                          long id) {
            Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку обновления и джоинов
                const string SQL_COMMAND = "update ShuffleSentence set IsShown=1"
                                           + " from ShuffleSentence as ss "
                                           + " inner join SentenceTranslation st on ss.SentenceTranslationId=st.Id"
                                           + " inner join Sentence as s1 on st.SentenceId1=s1.Id"
                                           + " inner join Sentence as s2 on st.SentenceId2=s2.Id"
                                           + " where ss.UserId={0} and ss.SentenceTranslationId={1}" +
                                           " and ((s1.LanguageId={2} and s2.LanguageId={3}) or (s1.LanguageId={3} and s2.LanguageId={2}))";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             userId, id, userLanguages.From.Id,
                                                             userLanguages.To.Id
                                                         });
            });
            return true;
        }

        public override bool DeleteUserHistory(long userId) {
            Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку удаления и джоинов
                const string SQL_COMMAND = "delete from ShuffleSentence where UserId={0}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             userId
                                                         });
            });
            return true;
        }

        /// <inheritdoc />
        public bool Clean(DateTime maxDateForRemove) {
            var res = Adapter.ActionByContext(c => {
                        const string SQL_COMMAND = "delete FROM [ShuffleSentence]"
                                                    + " FROM [ShuffleSentence] ss left join [User] u on ss.UserId = u.id"
                                                    + " where u.id is null";
                        c.Database.ExecuteSqlCommand(SQL_COMMAND);
                    });
            return res;
        }
    }
}