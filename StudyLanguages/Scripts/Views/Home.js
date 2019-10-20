HomeController = {
    Init: function() {
        var getUrl = function (url) {
            return ServerData.GetPath(url);
        };

        var markAsShowed = function (oldId, needTryAgain) {
            GlobalBusiness.markAsShowed(getUrl(ServerData.Patterns.Urls.MarkAsShowed), oldId, needTryAgain);
        };

        var getUrlByElem = function (sentence) {
            var url = String.format(ServerData.Patterns.Urls.Translation, sentence.Source.Id, sentence.Translation.Id);
            return getUrl(url);
        };

        var getTextElem = function(elem) {
            return elem.Text;
        };

        var getTextElemWithPronunciation = function(elem) {
            return Speaker.GetSentenceHtml(elem);
        };

        var autoPronounceCheckBox = new AutoPronounceCheckBox();
        var options = {
            actions: {
                loadPrev: ServerData.Patterns.Urls.LoadPrev,
                loadNext: ServerData.Patterns.Urls.NextPrev
            },
            emptyLink: ServerData.Patterns.EmptyLink,
            elems: ServerData.Elements,
            getUrl: getUrl,
            minCountToLoadPortion: ServerData.MinCountToLoadPortion,
            onShowedElem: markAsShowed,
            getUrlByElem: getUrlByElem,
            getTextElem: getTextElemWithPronunciation,
            getHistoryObj: function(elem) {
                return { sourceId: elem.Source.Id, translationId: elem.Translation.Id };
            },
            onSelectElem: $.proxy(function (elem) {
                UserKnowledge.SetAppropriateIcon(elem.Id);
                var isChecked = autoPronounceCheckBox.IsChecked();
                Speaker.SpeakElemPartByLanguage(isChecked, elem, ServerData.SpeakerHelper.Ids.Sentence);
                
                var sourceText = getTextElem(elem.Source);
                var translationText = getTextElem(elem.Translation);
                var keywords = String.format(ServerData.Patterns.Keywords, sourceText, translationText);
                GlobalBusiness.setKeywords('keywords', keywords);
                var description = String.format(ServerData.Patterns.Description, sourceText, translationText);
                GlobalBusiness.setDescription('description', description);
            }, this),
            getTitle: function(elem) {
                var sourceText = getTextElem(elem.Source);
                var translationText = getTextElem(elem.Translation);
                return String.format(ServerData.Patterns.Title, sourceText, translationText);
            },
            needCheckExistence: true
        };

        this.seriesToogleElems = new SeriesToogleElems(options);
    },

    MoveToNext: function() {
        this.seriesToogleElems.MoveToNext();
        return false;
    },

    MoveToPrev: function() {
        this.seriesToogleElems.MoveToPrev();
        return false;
    },

    ToogleTranslation: function() {
        return this.seriesToogleElems.ToogleTranslation();
    }
};

$(function() {
    HomeController.Init();
});