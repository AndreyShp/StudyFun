InnerGroupController = {
    Init: function () {
        var getTextElem = function (elem) {
            return elem.Text;
        };
        
        var getTextElemWithPronunciation = function (elem) {
            return Speaker.GetHtml(elem, ServerData.Patterns.SpeakerType);
        };

        var getUrlByElem = function (sentence) {
            var url = String.format(ServerData.Patterns.Url, ServerData.getItemForUrl(sentence.Source), ServerData.getItemForUrl(sentence.Translation));
            url = ServerData.GetPath(url);
            return url;
        };

        var autoPronounceCheckBox = new AutoPronounceCheckBox();

        var options = {
            emptyLink: ServerData.Patterns.EmptyLink,
            elems: ServerData.Elements,
           /* getUrl: function (action) {
                return ServerData.GetPath(ServerData.Patterns.Controller + "/" + action);
            },
            minCountToLoadPortion: ServerData.MinCountToLoadPortion,*/
            onShowedElem: null,
            getUrlByElem: getUrlByElem,
            getTextElem: getTextElemWithPronunciation,
            getHistoryObj: function (elem) {
                return { sourceText: getTextElem(elem.Source), translationText: getTextElem(elem.Translation) };
            },
            onSelectElem: $.proxy(function (elem) {
                UserKnowledge.SetAppropriateIcon(elem.Id);
                var isChecked = autoPronounceCheckBox.IsChecked();
                Speaker.SpeakElemPartByLanguage(isChecked, elem, ServerData.Patterns.SpeakerType);
                
                var sourceText = getTextElem(elem.Source);
                var translationText = getTextElem(elem.Translation);
                var altValue = String.format(ServerData.Patterns.ImageAlt, sourceText, translationText);
                //TODO: убрать дублирование идентификатора изображения
                $('#imageContainerId').attr('alt', altValue);
                var keywords = String.format(ServerData.Patterns.Keywords, sourceText, translationText);
                GlobalBusiness.setKeywords('keywords', keywords);
                var description = String.format(ServerData.Patterns.Description, sourceText, translationText);
                GlobalBusiness.setDescription('description', description);
            }, this),
            getTitle: function (elem) {
                var sourceText = getTextElem(elem.Source);
                var translationText = getTextElem(elem.Translation);
                return String.format(ServerData.Patterns.Title, sourceText, translationText);
            },
            getNewImage: function (elem) {
                if (elem.HasImage) {
                    return { NeedDisplay: true, Url: String.format(ServerData.Patterns.Image, elem.Id) };
                }
                return { NeedDisplay: false, Url: '' };
            },
            slideTimeout: 3000,
            needCheckExistence: true
        };
        this.seriesToogleElems = new SeriesToogleElems(options);
    },

    MoveToFirst: function () {
        this.seriesToogleElems.MoveToFirst();
        return false;
    },

    MoveToPrev: function () {
        this.seriesToogleElems.MoveToPrev();
        return false;
    },

    MoveToNext: function () {
        this.seriesToogleElems.MoveToNext();
        return false;
    },

    MoveToLast: function () {
        this.seriesToogleElems.MoveToLast();
        return false;
    },

    ToogleTranslation: function () {
        return this.seriesToogleElems.ToogleTranslation();
    },
    
    Shuffle: function () {
        this.seriesToogleElems.Shuffle();
    }    
};

$(function () {
    InnerGroupController.Init();
});