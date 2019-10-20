using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BusinessLogic.Data.Auxiliary;
using BusinessLogic.Data.Comparison;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.Data.Money;
using BusinessLogic.Data.Rating;
using BusinessLogic.Data.Representation;
using BusinessLogic.Data.Sentence;
using BusinessLogic.Data.Video;
using BusinessLogic.Data.Word;
using BusinessLogic.Helpers;
using BusinessLogic.Logger;

namespace BusinessLogic.Data {
    public class StudyLanguageContext : DbContext {
        public StudyLanguageContext() : base("StudyLanguageContext") {}

        public DbSet<Language> Language { get; set; }

        public DbSet<Group> Group { get; set; }

        public DbSet<GroupWord> GroupWord { get; set; }

        public DbSet<GroupSentence> GroupSentence { get; set; }

        public DbSet<RatingByIp> RatingByIp { get; set; }

        public DbSet<User> User { get; set; }

        #region Вспомогательные данные

        public DbSet<CrossReference> CrossReference { get; set; }

        public DbSet<Interview> Interview { get; set; }

        #endregion

        #region Представление

        public DbSet<Representation.Representation> Representation { get; set; }

        public DbSet<RepresentationArea> RepresentationArea { get; set; }

        #endregion

        #region Предложения

        public DbSet<Sentence.Sentence> Sentence { get; set; }

        public DbSet<SentenceTranslation> SentenceTranslation { get; set; }

        public DbSet<ShuffleSentence> ShuffleSentence { get; set; }

        public DbSet<SentenceWord> SentenceWord { get; set; }

        #endregion

        #region Слова

        public DbSet<Word.Word> Word { get; set; }

        public DbSet<WordTranslation> WordTranslation { get; set; }

        //public DbSet<Association> Association { get; set; }

        public DbSet<ShuffleWord> ShuffleWord { get; set; }

        public DbSet<PopularWord> PopularWord { get; set; }

        #endregion

        #region Различия в использовании

        public DbSet<GroupComparison> GroupComparison { get; set; }

        public DbSet<ComparisonItem> ComparisonItem { get; set; }

        public DbSet<ComparisonRule> ComparisonRule { get; set; }

        public DbSet<ComparisonRuleExample> ComparisonRuleExample { get; set; }

        #endregion

        #region Знания

        public DbSet<UserKnowledge> UserKnowledge { get; set; }

        public DbSet<UserRepetitionInterval> UserRepetitionInterval { get; set; }

        #endregion

        #region Видео

        public DbSet<Video.Video> Video { get; set; }

        public DbSet<VideoSentence> VideoSentence { get; set; }

        public DbSet<TVSeries> TVSeries { get; set; }

        #endregion

        #region Деньги

        public DbSet<Payment> Payment { get; set; }

        public DbSet<PurchasedGoods> PurchasedGoods { get; set; }

        #endregion

        #region Системные данные

        public DbSet<LogEntry> LogEntry { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Language>().HasKey(e => e.Id);
            modelBuilder.Entity<Language>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Language>().Property(e => e.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Language>().Property(e => e.Image).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Language>().Property(e => e.ShortName).HasMaxLength(5).IsRequired();
            modelBuilder.Entity<Language>().ToTable("Language");

            modelBuilder.Entity<Sentence.Sentence>().HasKey(e => e.Id);
            modelBuilder.Entity<Sentence.Sentence>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Sentence.Sentence>().Property(e => e.Text).HasMaxLength(500).IsRequired();
            modelBuilder.Entity<Sentence.Sentence>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<Sentence.Sentence>().Property(e => e.Pronunciation);
            modelBuilder.Entity<Sentence.Sentence>().ToTable("Sentence");

            modelBuilder.Entity<PuzzleSentence>().HasKey(e => e.Id);
            modelBuilder.Entity<PuzzleSentence>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<PuzzleSentence>().Property(e => e.SentenceId).IsRequired();
            modelBuilder.Entity<PuzzleSentence>().Property(e => e.SourceType).IsRequired();
            modelBuilder.Entity<PuzzleSentence>().ToTable("PuzzleSentence");

            modelBuilder.Entity<SentenceTranslation>().HasKey(e => e.Id);
            modelBuilder.Entity<SentenceTranslation>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SentenceTranslation>().Property(e => e.SentenceId1).IsRequired();
            modelBuilder.Entity<SentenceTranslation>().Property(e => e.SentenceId2).IsRequired();
            modelBuilder.Entity<SentenceTranslation>().Property(e => e.Image);
            modelBuilder.Entity<SentenceTranslation>().Property(e => e.Rating);
            modelBuilder.Entity<SentenceTranslation>().Property(e => e.Type).IsRequired();
            modelBuilder.Entity<SentenceTranslation>().ToTable("SentenceTranslation");

            modelBuilder.Entity<ShuffleSentence>().HasKey(e => e.Id);
            modelBuilder.Entity<ShuffleSentence>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ShuffleSentence>().Property(e => e.UserId).IsRequired();
            modelBuilder.Entity<ShuffleSentence>().Property(e => e.SentenceTranslationId).IsRequired();
            modelBuilder.Entity<ShuffleSentence>().Property(e => e.IsShown).IsRequired();
            //modelBuilder.Entity<ShuffleSentence>().Property(e => e.Type).IsRequired();
            modelBuilder.Entity<ShuffleSentence>().ToTable("ShuffleSentence");

            modelBuilder.Entity<Word.Word>().HasKey(e => e.Id);
            modelBuilder.Entity<Word.Word>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Word.Word>().Property(e => e.Text).HasMaxLength(Data.Word.Word.TEXT_LENGTH).IsRequired();
            modelBuilder.Entity<Word.Word>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<Word.Word>().Property(e => e.Type).IsRequired();
            modelBuilder.Entity<Word.Word>().Property(e => e.Pronunciation);
            modelBuilder.Entity<Word.Word>().ToTable("Word");

            modelBuilder.Entity<Group>().HasKey(e => e.Id);
            modelBuilder.Entity<Group>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Group>().Property(e => e.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Group>().Property(e => e.IsVisible).IsRequired();
            modelBuilder.Entity<Group>().Property(e => e.Rating);
            modelBuilder.Entity<Group>().Property(e => e.Image);
            modelBuilder.Entity<Group>().Property(e => e.Type).IsRequired();
            modelBuilder.Entity<Group>().Property(e => e.LastModified).IsRequired();
            modelBuilder.Entity<Group>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<Group>().ToTable("Group");

            modelBuilder.Entity<GroupWord>().HasKey(e => e.Id);
            modelBuilder.Entity<GroupWord>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<GroupWord>().Property(e => e.GroupId).IsRequired();
            modelBuilder.Entity<GroupWord>().Property(e => e.WordTranslationId).IsRequired();
            modelBuilder.Entity<GroupWord>().Property(e => e.Rating);
            modelBuilder.Entity<GroupWord>().ToTable("GroupWord");

            modelBuilder.Entity<WordTranslation>().HasKey(e => e.Id);
            modelBuilder.Entity<WordTranslation>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<WordTranslation>().Property(e => e.WordId1).IsRequired();
            modelBuilder.Entity<WordTranslation>().Property(e => e.WordId2).IsRequired();
            modelBuilder.Entity<WordTranslation>().Property(e => e.Image);
            modelBuilder.Entity<WordTranslation>().Property(e => e.Rating);
            modelBuilder.Entity<WordTranslation>().ToTable("WordTranslation");

            modelBuilder.Entity<GroupSentence>().HasKey(e => e.Id);
            modelBuilder.Entity<GroupSentence>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<GroupSentence>().Property(e => e.GroupId).IsRequired();
            modelBuilder.Entity<GroupSentence>().Property(e => e.SentenceTranslationId).IsRequired();
            modelBuilder.Entity<GroupSentence>().Property(e => e.Rating);
            modelBuilder.Entity<GroupSentence>().ToTable("GroupSentence");

            modelBuilder.Entity<Interview>().HasKey(e => e.Id);
            modelBuilder.Entity<Interview>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Interview>().Property(e => e.Text).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Interview>().Property(e => e.CountAnswers).IsRequired();
            modelBuilder.Entity<Interview>().Property(e => e.ParentInterviewId);
            modelBuilder.Entity<Interview>().ToTable("Interview");

            modelBuilder.Entity<Representation.Representation>().HasKey(e => e.Id);
            modelBuilder.Entity<Representation.Representation>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Representation.Representation>().Property(e => e.Title).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Representation.Representation>().Property(e => e.Image).IsRequired();
            modelBuilder.Entity<Representation.Representation>().Property(e => e.IsVisible).IsRequired();
            modelBuilder.Entity<Representation.Representation>().Property(e => e.Height).IsRequired();
            modelBuilder.Entity<Representation.Representation>().Property(e => e.Width).IsRequired();
            modelBuilder.Entity<Representation.Representation>().Property(e => e.Rating);
            modelBuilder.Entity<Representation.Representation>().Property(e => e.WidthPercent);
            modelBuilder.Entity<Representation.Representation>().Property(e => e.LastModified).IsRequired();
            modelBuilder.Entity<Representation.Representation>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<Representation.Representation>().ToTable("Representation");

            modelBuilder.Entity<RepresentationArea>().HasKey(e => e.Id);
            modelBuilder.Entity<RepresentationArea>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RepresentationArea>().Property(e => e.RepresentationId).IsRequired();
            modelBuilder.Entity<RepresentationArea>().Property(e => e.LeftUpperX).IsRequired();
            modelBuilder.Entity<RepresentationArea>().Property(e => e.LeftUpperY).IsRequired();
            modelBuilder.Entity<RepresentationArea>().Property(e => e.RightBottomX).IsRequired();
            modelBuilder.Entity<RepresentationArea>().Property(e => e.RightBottomY).IsRequired();
            modelBuilder.Entity<RepresentationArea>().Property(e => e.WordTranslationId).IsRequired();
            modelBuilder.Entity<RepresentationArea>().Property(e => e.Rating);
            modelBuilder.Entity<RepresentationArea>().ToTable("RepresentationArea");

            modelBuilder.Entity<ShuffleWord>().HasKey(e => e.Id);
            modelBuilder.Entity<ShuffleWord>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ShuffleWord>().Property(e => e.UserId).IsRequired();
            modelBuilder.Entity<ShuffleWord>().Property(e => e.WordTranslationId).IsRequired();
            modelBuilder.Entity<ShuffleWord>().Property(e => e.Type).IsRequired();
            modelBuilder.Entity<ShuffleWord>().Property(e => e.IsShown).IsRequired();
            modelBuilder.Entity<ShuffleWord>().ToTable("ShuffleWord");

            modelBuilder.Entity<PopularWord>().HasKey(e => e.Id);
            modelBuilder.Entity<PopularWord>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<PopularWord>().Property(e => e.WordTranslationId).IsRequired();
            modelBuilder.Entity<PopularWord>().Property(e => e.Type).IsRequired();
            modelBuilder.Entity<PopularWord>().ToTable("PopularWord");

            modelBuilder.Entity<RatingByIp>().HasKey(e => e.Id);
            modelBuilder.Entity<RatingByIp>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<RatingByIp>().Property(e => e.PageType).IsRequired();
            modelBuilder.Entity<RatingByIp>().Property(e => e.EntityId).IsRequired();
            modelBuilder.Entity<RatingByIp>().Property(e => e.Ip).HasMaxLength(Rating.RatingByIp.IP_LENGTH).IsRequired();
            modelBuilder.Entity<RatingByIp>().ToTable("RatingByIp");

            modelBuilder.Entity<SentenceWord>().HasKey(e => e.Id);
            modelBuilder.Entity<SentenceWord>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SentenceWord>().Property(e => e.SentenceId).IsRequired();
            modelBuilder.Entity<SentenceWord>().Property(e => e.WordId).IsRequired();
            modelBuilder.Entity<SentenceWord>().Property(e => e.OriginalText).HasMaxLength(Data.Word.Word.TEXT_LENGTH).
                IsRequired();
            modelBuilder.Entity<SentenceWord>().Property(e => e.OrderInSentence).IsRequired();
            modelBuilder.Entity<SentenceWord>().Property(e => e.GrammarType).IsRequired();
            modelBuilder.Entity<SentenceWord>().ToTable("SentenceWord");

            modelBuilder.Entity<CrossReference>().HasKey(e => e.Id);
            modelBuilder.Entity<CrossReference>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<CrossReference>().Property(e => e.SourceId).IsRequired();
            modelBuilder.Entity<CrossReference>().Property(e => e.SourceType).IsRequired();
            modelBuilder.Entity<CrossReference>().Property(e => e.DestinationId).IsRequired();
            modelBuilder.Entity<CrossReference>().Property(e => e.DestinationType).IsRequired();
            modelBuilder.Entity<CrossReference>().ToTable("CrossReference");

            #region Различия в использовании

            modelBuilder.Entity<GroupComparison>().HasKey(e => e.Id);
            modelBuilder.Entity<GroupComparison>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<GroupComparison>().Property(e => e.Title).HasMaxLength(
                Comparison.GroupComparison.TITLE_LENGTH).IsRequired();
            modelBuilder.Entity<GroupComparison>().Property(e => e.Description);
            modelBuilder.Entity<GroupComparison>().Property(e => e.AdditionalInfo);
            modelBuilder.Entity<GroupComparison>().Property(e => e.IsVisible).IsRequired();
            modelBuilder.Entity<GroupComparison>().Property(e => e.Rating);
            modelBuilder.Entity<GroupComparison>().Property(e => e.LastModified).IsRequired();
            modelBuilder.Entity<GroupComparison>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<GroupComparison>().ToTable("GroupComparison");

            modelBuilder.Entity<ComparisonItem>().HasKey(e => e.Id);
            modelBuilder.Entity<ComparisonItem>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ComparisonItem>().Property(e => e.GroupComparisonId).IsRequired();
            modelBuilder.Entity<ComparisonItem>().Property(e => e.Title).HasMaxLength(
                Comparison.ComparisonItem.TITLE_LENGTH).IsRequired();
            modelBuilder.Entity<ComparisonItem>().Property(e => e.TitleTranslation).HasMaxLength(
                Comparison.ComparisonItem.TITLE_LENGTH).IsRequired();
            modelBuilder.Entity<ComparisonItem>().Property(e => e.Description);
            modelBuilder.Entity<ComparisonItem>().Property(e => e.Order).IsRequired();
            modelBuilder.Entity<ComparisonItem>().ToTable("ComparisonItem");

            modelBuilder.Entity<ComparisonRule>().HasKey(e => e.Id);
            modelBuilder.Entity<ComparisonRule>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ComparisonRule>().Property(e => e.ComparisonItemId).IsRequired();
            modelBuilder.Entity<ComparisonRule>().Property(e => e.Description).IsRequired();
            modelBuilder.Entity<ComparisonRule>().Property(e => e.Order).IsRequired();
            modelBuilder.Entity<ComparisonRule>().ToTable("ComparisonRule");

            modelBuilder.Entity<ComparisonRuleExample>().HasKey(e => e.Id);
            modelBuilder.Entity<ComparisonRuleExample>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ComparisonRuleExample>().Property(e => e.ComparisonRuleId).IsRequired();
            modelBuilder.Entity<ComparisonRuleExample>().Property(e => e.SentenceTranslationId).IsRequired();
            modelBuilder.Entity<ComparisonRuleExample>().Property(e => e.Description);
            modelBuilder.Entity<ComparisonRuleExample>().Property(e => e.Order).IsRequired();
            modelBuilder.Entity<ComparisonRuleExample>().ToTable("ComparisonRuleExample");

            #endregion

            modelBuilder.Entity<User>().HasKey(e => e.Id);
            modelBuilder.Entity<User>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<User>().Property(e => e.UniqueHash).HasMaxLength(UserUniqueId.USER_HASH_LEN).IsRequired();
            modelBuilder.Entity<User>().Property(e => e.LastActivity).IsRequired();
            modelBuilder.Entity<User>().Property(e => e.CreationDate).IsRequired();
            modelBuilder.Entity<User>().Property(e => e.CreationIp).HasMaxLength(Rating.RatingByIp.IP_LENGTH).IsRequired
                ();
            modelBuilder.Entity<User>().Property(e => e.LastIp).HasMaxLength(Rating.RatingByIp.IP_LENGTH).IsRequired();
            modelBuilder.Entity<User>().Property(e => e.Email).HasMaxLength(Data.User.EMAIL_LENGTH);
            modelBuilder.Entity<User>().ToTable("User");

            #region Мои знания

            modelBuilder.Entity<UserKnowledge>().HasKey(e => e.Id);
            modelBuilder.Entity<UserKnowledge>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<UserKnowledge>().Property(e => e.UserId).IsRequired();
            modelBuilder.Entity<UserKnowledge>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<UserKnowledge>().Property(e => e.Data);
            modelBuilder.Entity<UserKnowledge>().Property(e => e.DataId);
            modelBuilder.Entity<UserKnowledge>().Property(e => e.DataType).IsRequired();
            modelBuilder.Entity<UserKnowledge>().Property(e => e.Tip);
            modelBuilder.Entity<UserKnowledge>().Property(e => e.Status).IsRequired();
            modelBuilder.Entity<UserKnowledge>().Property(e => e.CreationDate).IsRequired();
            modelBuilder.Entity<UserKnowledge>().Property(e => e.DeletedDate).IsRequired();
            modelBuilder.Entity<UserKnowledge>().Property(e => e.SystemData);
            modelBuilder.Entity<UserKnowledge>().Property(e => e.Hash).HasMaxLength(Knowledge.UserKnowledge.HASH_LENGTH)
                .IsRequired();
            modelBuilder.Entity<UserKnowledge>().ToTable("UserKnowledge");

            modelBuilder.Entity<UserRepetitionInterval>().HasKey(e => e.Id);
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.UserId).IsRequired();
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.DataId).IsRequired();
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.DataType).IsRequired();
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.SourceType).IsRequired();
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.Mark).IsRequired();
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.RepetitionMark).IsRequired();
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.RepetitionTotal).IsRequired();
            modelBuilder.Entity<UserRepetitionInterval>().Property(e => e.NextTimeShow).IsRequired();
            modelBuilder.Entity<UserRepetitionInterval>().ToTable("UserRepetitionInterval");

            #endregion

            #region Видео

            modelBuilder.Entity<Video.Video>().HasKey(e => e.Id);
            modelBuilder.Entity<Video.Video>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Video.Video>().Property(e => e.Title).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<Video.Video>().Property(e => e.HtmlCode).IsRequired();
            modelBuilder.Entity<Video.Video>().Property(e => e.Image);
            modelBuilder.Entity<Video.Video>().Property(e => e.IsVisible).IsRequired();
            modelBuilder.Entity<Video.Video>().Property(e => e.Rating);
            modelBuilder.Entity<Video.Video>().Property(e => e.LastModified).IsRequired();
            modelBuilder.Entity<Video.Video>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<Video.Video>().Property(e => e.Type).IsRequired();
            modelBuilder.Entity<Video.Video>().ToTable("Video");

            modelBuilder.Entity<VideoSentence>().HasKey(e => e.Id);
            modelBuilder.Entity<VideoSentence>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<VideoSentence>().Property(e => e.VideoId).IsRequired();
            modelBuilder.Entity<VideoSentence>().Property(e => e.Source);
            modelBuilder.Entity<VideoSentence>().Property(e => e.Translation);
            modelBuilder.Entity<VideoSentence>().Property(e => e.Order).IsRequired();
            modelBuilder.Entity<VideoSentence>().ToTable("VideoSentence");

            modelBuilder.Entity<TVSeries>().HasKey(e => e.Id);
            modelBuilder.Entity<TVSeries>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TVSeries>().Property(e => e.Info).IsRequired();
            modelBuilder.Entity<TVSeries>().Property(e => e.UrlPart).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<TVSeries>().Property(e => e.ParentId).IsRequired();
            modelBuilder.Entity<TVSeries>().Property(e => e.IsVisible).IsRequired();
            modelBuilder.Entity<TVSeries>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<TVSeries>().Property(e => e.DataType).IsRequired();
            modelBuilder.Entity<TVSeries>().ToTable("TVSeries");

            #endregion

            #region Деньги

            modelBuilder.Entity<Payment>().HasKey(e => e.Id);
            modelBuilder.Entity<Payment>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Payment>().Property(e => e.Price).IsRequired();
            modelBuilder.Entity<Payment>().Property(e => e.Status).IsRequired();
            modelBuilder.Entity<Payment>().Property(e => e.Description).IsRequired();
            modelBuilder.Entity<Payment>().Property(e => e.UserId).IsRequired();
            modelBuilder.Entity<Payment>().Property(e => e.CreationDate).IsRequired();
            modelBuilder.Entity<Payment>().Property(e => e.PaymentDate).IsRequired();
            modelBuilder.Entity<Payment>().Property(e => e.System).IsRequired();
            modelBuilder.Entity<Payment>().ToTable("Payment");

            modelBuilder.Entity<PurchasedGoods>().HasKey(e => e.Id);
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.Price).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.Status).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.UniqueDownloadId).HasMaxLength(
                IdGenerator.DOWNLOAD_ID_LEN).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.FullDescription).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.ShortDescription).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.UserId).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.PurchaseDate).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.PostToCustomerDate).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.PaymentId).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.Goods).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().Property(e => e.LanguageId).IsRequired();
            modelBuilder.Entity<PurchasedGoods>().ToTable("PurchasedGoods");

            #endregion

            #region Системные данные

            modelBuilder.Entity<LogEntry>().HasKey(e => e.Id);
            modelBuilder.Entity<LogEntry>().Property(e => e.Id).HasDatabaseGeneratedOption(
                DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<LogEntry>().Property(e => e.MessageType).IsRequired();
            modelBuilder.Entity<LogEntry>().Property(e => e.Message).IsRequired();
            modelBuilder.Entity<LogEntry>().ToTable("LogEntry");

            #endregion
        }
    }
}