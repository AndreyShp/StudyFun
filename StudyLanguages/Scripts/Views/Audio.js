AudioController = {
    elem: null,   
    getUrl: function (url) {
        return ServerData.GetPath(url);
    },
    
    getTextElem: function(elem) {
        return elem.Text;
    },

    Init: function () {
        this.containers = {
            pronounceAfterShow: $('#pronounceAfterShow'),
            checkField: $('#checkField'),
        };
        
        this.containers.checkField
            .on('keyup', $.proxy(this.checkKeyUp, this))
            .focus();
        Global.focusClearField(this.containers.checkField);

        this.btnToogle = new ToggleBtn({
            block: $('#toogleBlock'),
            btn: $('#toogleBtn'),
            cookieName: null,
            captionShow: 'Показать слово с переводом &raquo;',
            captionHide: 'Спрятать слово с переводом &laquo;'
        });

        var markAsShowed = function (oldId, needTryAgain) {
            GlobalBusiness.markAsShowed(AudioController.getUrl(ServerData.Patterns.Urls.MarkAsShowed), oldId, needTryAgain);
        };

        var getUrlByElem = function (sentence) {
            var url = String.format(ServerData.Patterns.Urls.Translation, sentence.Source.Id, sentence.Translation.Id);
            return AudioController.getUrl(url);
        };

        var options = {
            actions: {
                loadPrev: ServerData.Patterns.Urls.LoadPrev,
                loadNext: ServerData.Patterns.Urls.NextPrev
            },
            emptyLink: ServerData.Patterns.EmptyLink,
            elems: ServerData.Elements,
            getUrl: this.getUrl,
            minCountToLoadPortion: ServerData.MinCountToLoadPortion,
            onShowedElem: markAsShowed,
            getUrlByElem: getUrlByElem,
            getTextElem: this.getTextElem,
            getHistoryObj: function(elem) {
                return { sourceId: elem.Source.Id, translationId: elem.Translation.Id };
            },
            getTitle: $.proxy(function(elem) {
                var sourceText = this.getTextElem(elem.Source);
                var translationText = this.getTextElem(elem.Translation);
                return String.format(ServerData.Patterns.Title, sourceText, translationText);
            }, this),
            onSelectElem: $.proxy(function (elem) {
                this.elem = elem;
                var sourceText = this.getTextElem(elem.Source);
                var translationText = this.getTextElem(elem.Translation);
                var altValue = String.format(ServerData.Patterns.ImageAlt, sourceText, translationText);
                //TODO: убрать дублирование идентификатора изображения
                $('#imageContainerId').attr('alt', altValue);
                var keywords = String.format(ServerData.Patterns.Keywords, sourceText, translationText);
                GlobalBusiness.setKeywords(keywords);
                var description = String.format(ServerData.Patterns.Description, sourceText, translationText);
                GlobalBusiness.setDescription(description);
            }, this),
            getNewImage: function (elem) {
                if (elem.HasImage) {
                    var imageUrl = String.format(ServerData.Patterns.Urls.Image, elem.Id);
                    imageUrl = AudioController.getUrl(imageUrl);
                    return { NeedDisplay: true, Url: imageUrl };
                }
                return { NeedDisplay: false, Url: '' };
            },
            onMoveAction: $.proxy(function (elem) {
                this.elem = elem;
                this.speakIfNeed();
                this.containers.checkField.val('').focus();
                if (this.message != null) {
                    this.message.Remove();
                    this.message = null;
                }
                this.changeErrorCheck(false);
            }, this)
        };

        this.seriesToogleElems = new SeriesToogleElems(options);
    },

    MoveToNext: function () {
        this.seriesToogleElems.MoveToNext();
        return false;
    },

    MoveToPrev: function() {
        this.seriesToogleElems.MoveToPrev();
        return false;
    },

    speakIfNeed: function () {
        var isChecked = this.containers.pronounceAfterShow.is(":checked");
        if (isChecked) {
            this.Speak();
        }
    },

    ToogleBlock: function() {
        return this.btnToogle.Toogle();
    },
    
    Speak: function () {
        if (this.elem) {
            Speaker.Speak(this.elem.Source.Id, ServerData.SpeakerHelper.Ids.Word);
            this.containers.checkField.focus();
        }
        return false;
    },
    
    Check: function () {
        var textToCheck = this.containers.checkField.val();
        var isEmpty = !textToCheck;
        this.changeErrorCheck(isEmpty);
        if (isEmpty) {
            return;
        }
        var url = this.getUrl(ServerData.Patterns.Urls.Check);
        $.post(url, { textToCheck: textToCheck, sourceId: this.elem.Source.Id }, $.proxy(function (data) {
            var success = data != null && data.success;
            var options = { isSuccess: success, removeLabels: true, container: this.getControlsContainer() };
            options.text = success ? 'Верно. Переводится как: ' + this.getTextElem(this.elem.Translation) : 'Неверно. Попробуйте еще раз';
            this.message = new Message(options);
            this.message.Show();
        }, this), "json");
    },

    changeErrorCheck: function(isShowError) {
        Global.setInputStyle(this.containers.checkField, isShowError);
    },
    
    getControlsContainer: function () {
        return $("#message_container");
    },

    checkKeyUp: function (event) {
        if (event.which == 13) {
            this.Check();
        } else {
            this.changeErrorCheck();
        }
        var enteredText = this.containers.checkField.val();
        var keyCode = event.keyCode;
        if (enteredText && (keyCode == 37 || keyCode == 39)) {
            event.stopPropagation();
        }
    }
};

$(function() {
    AudioController.Init();
});