using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Knowledge;
using BusinessLogic.Validators;
using StudyLanguages.Models.Trainer;

namespace StudyLanguages.Helpers.Trainer {
    public class TrainerHelper {
        private const int MAX_COUNT_ITEMS_TO_GET = 10;

        private readonly UserLanguages _userLanguages;
        private readonly IUserRepetitionIntervalQuery _userRepetitionIntervalQuery;

        public TrainerHelper(IUserRepetitionIntervalQuery userRepetitionIntervalQuery, UserLanguages userLanguages) {
            _userLanguages = userLanguages;
            _userRepetitionIntervalQuery = userRepetitionIntervalQuery;
        }

        public TrainerModel GetTrainerModel(HttpRequestBase request) {
            var model = new TrainerModel(_userLanguages);
            List<UserRepetitionIntervalItem> repetitionIntervalItems =
                _userRepetitionIntervalQuery.GetRepetitionIntervalItems(_userLanguages.From.Id,
                                                                        _userLanguages.To.Id,
                                                                        MAX_COUNT_ITEMS_TO_GET);

            foreach (UserRepetitionIntervalItem repetitionItem in
                repetitionIntervalItems ?? new List<UserRepetitionIntervalItem>(0)) {
                var trainerItem = new TrainerItem();

                var userKnowledge = repetitionItem.Data as UserKnowledgeItem;
                if (userKnowledge != null) {
                    var dataType = userKnowledge.DataType;
                    //тренируемся на запоминание пользовательских данных
                    SpeakerDataType speakerType = KnowledgeHelper.GetSpeakerType(dataType);
                    var parsedData = userKnowledge.ParsedData as SourceWithTranslation;

                    trainerItem.NextTimeToShow = KnowledgeHelper.ConvertDateTimeToJsTicks(repetitionItem.NextTimeToShow);
                    trainerItem.DataId = repetitionItem.DataId;
                    trainerItem.DataType = (int) repetitionItem.DataType;
                    trainerItem.HtmlSource = OurHtmlHelper.GetSpeakerHtml(parsedData.Source, speakerType);
                    trainerItem.HtmlTranslation = OurHtmlHelper.GetSpeakerHtml(parsedData.Translation, speakerType);
                    trainerItem.SourceLanguageId = parsedData.Source.LanguageId;
                    trainerItem.TranslationLanguageId = parsedData.Translation.LanguageId;
                    trainerItem.ImageUrl = parsedData.HasImage ? GetImageUrl(request, parsedData.Id, dataType) : null;
                }

                if (IdValidator.IsValid(trainerItem.DataId) && trainerItem.HtmlSource != null
                    && trainerItem.HtmlTranslation != null) {
                    //данные такого типа, могут быть представлены
                    model.AddItem(trainerItem);
                }
            }
            return model;
        }

        private static string GetImageUrl(HttpRequestBase request, long id, KnowledgeDataType dataType) {
            string controllerName;
            switch (dataType) {
                case KnowledgeDataType.WordTranslation:
                    controllerName = RouteConfig.GROUP_WORD_CONTROLLER;
                    break;
                case KnowledgeDataType.SentenceTranslation:
                    controllerName = RouteConfig.GROUP_SENTENCE_CONTROLLER;
                    break;
                default:
                    controllerName = null;
                    break;
            }
            return controllerName != null
                       ? UrlBuilder.GetImageUrlById(request, controllerName,
                                                    id.ToString(CultureInfo.InvariantCulture))
                       : null;
        }

        public JsonResult SetMark(HttpRequestBase request, KnowledgeMark mark,
                                  UserRepetitionIntervalItem repetitionInterval) {
            //выставляем оценку
            if (!_userRepetitionIntervalQuery.SetMark(repetitionInterval, mark)) {
                return JsonResultHelper.Error();
            }

            TrainerModel model = GetTrainerModel(request);
            return
                JsonResultHelper.GetUnlimitedJsonResult(
                    new {sourceLanguageId = _userLanguages.From.Id, items = model.Items});
        }
    }
}