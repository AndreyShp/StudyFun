GapsTrainerController = {
    ITEM_CLASS: 'gaps-trainer-item',
    MESSAGE_CONTAINER_ID: '#messageContainer',
    INPUT_SELECTOR: 'input.gaps-trainer-field',
    elems: null,
    index: 0,
    count: 10,
    actionOnPortionElems: function(action) {
        var endIndex = this.index + this.count;
        if (endIndex > this.elems.length) {
            endIndex = this.elems.length;
        }
        var result = this.elems.slice(this.index, endIndex);
        $.each(result, action);
    },
    clearContainerStyle: function (input) {
        input.removeClass('alert-danger').removeClass('alert-success');
    },
    setContainerStyle: function (input, isSuccess) {
        var jInput = $(input);
        this.clearContainerStyle(jInput);
        var cls;
        if (isSuccess) {
            cls = 'alert-success';
        } else {
            cls = 'alert-danger';
        }
        jInput.addClass(cls);
    },
    getContainerByElem: function(elem) {
        var container = $('.' + GapsTrainerController.ITEM_CLASS + '[data-id="' + elem.Id + '"]');
        return container.length > 0 ? container : null;
    },
    getOriginalTextByElem: function(elem) {
        return elem.Original.Text;
    },
    showElements: function () {
        var table = '<table class="table">';
        var attr = 'autofocus';
        this.actionOnPortionElems(function (index, elem) {
            table += '<tr>';
            table += '<td class="' + GapsTrainerController.ITEM_CLASS + '" data-id="' + elem.Id + '">';
            for (var i = 0; i < elem.TextForUser.length; i++) {
                var ch = elem.TextForUser[i];
                if (ch == ServerData.Patterns.GapChar) {
                    table += '<input class="gaps-trainer-field form-control" type="text" maxLength="1" autocomplete="off" ' + attr + '/>';
                    attr = '';
                } else {
                    table += ch;
                }
                table += ' ';
            }
            table += '</td>';
            table += '<td class="gaps-trainer-answer">' + Speaker.GetHtml(elem.Original, ServerData.Patterns.SpeakerType) + '</td>';
            table += '<td class="gaps-trainer-translation">' + Speaker.GetHtml(elem.Translation, ServerData.Patterns.SpeakerType) + '</td>';
            table += '</tr>';
            return true;
        });
        table += '</table>';
        $('#gapsTrainerContainer').html(table);

        var inputs = $(GapsTrainerController.INPUT_SELECTOR);

        var inputHasValue = function (input) {
            var val = $(input).val();
            var len = val.length;
            return len > 0;
        };

        var setFocusToInputField = function (curField, direction) {
            var index = inputs.index(curField);
            var newIndex = index + direction;
            if (newIndex >= 0 && newIndex < inputs.length) {
                inputs.eq(newIndex).focus();
            } /* else {
            fields.first().focus();
        }*/
        };

        var getCaretPosition = function (input) {
            var iCaretPos = 0;
            if (document.selection) {
                // IE Support
                input.focus();
                var oSel = document.selection.createRange();
                oSel.moveStart('character', -input.value.length);
                iCaretPos = oSel.text.length;
            } else if (input.selectionStart || input.selectionStart == '0') {
                // Firefox support
                iCaretPos = input.selectionStart;
            }

            // Return results
            return (iCaretPos);
        };

        inputs.keypress(function (e) {
            var keyCode = e.keyCode || e.which;
            var hasValue = inputHasValue(this);
            var caretPosition = getCaretPosition(this);
            if (((caretPosition == 0 && !hasValue) || caretPosition == 1) && keyCode == 39) {
                //->
                setFocusToInputField(this, 1);
            } else if (caretPosition == 0 && (keyCode == 8 || keyCode == 37)) {
                //backspace или <-
                setFocusToInputField(this, -1);
            }
        }).on('input', function () {
            if (inputHasValue(this)) {
                //фокус на следующее поле
                setFocusToInputField(this, 1);
            } else {
                //фокус на предыдущее поле
                setFocusToInputField(this, -1);
            }
        }).focus(function () {
            GapsTrainerController.clearContainerStyle($(this));
        });
        
        this.answerToogleBtn = new ToggleBtn({
            block: $('.gaps-trainer-answer'),
            btn: $('#answerToogleBtn'),
            cookieName: null,
            captionShow: 'Показать правильный ответ &raquo;',
            captionHide: 'Спрятать правильный ответ &laquo;',
            isHidden: true
        });

        this.translationToogleBtn = new ToggleBtn({
            block: $('.gaps-trainer-translation'),
            btn: $('#translationToogleBtn'),
            cookieName: null,
            captionShow: 'Показать перевод &raquo;',
            captionHide: 'Спрятать перевод &laquo;',
            isHidden: true
        });
        
        this.isAnswerClicked = false;
        this.isTranslationClicked = false;
    },
    
    toogleLoadNextBtn: function () {  
        var loadNextBtn = $('#loadNextPortionBtn');
        if ((this.index + this.count) < this.elems.length) {
            loadNextBtn.show();
        } else {
            loadNextBtn.hide();
        }
    },

    Init: function () {
        this.elems = GlobalBusiness.shuffle(ServerData.Elements);

        this.showElements();
        this.toogleLoadNextBtn();
    },
    
    ToogleAnswer: function () {
        this.isAnswerClicked = true;
        this.answerToogleBtn.Toogle();
    },

    ToogleTranslation: function () {
        this.isTranslationClicked = true;
        this.translationToogleBtn.Toogle();
    },

    Check: function() {
        var gapChar = ServerData.Patterns.GapChar;
        var getIndexGap = function(textForUser, prevGap) {
            return textForUser.indexOf(gapChar, prevGap);
        };
        var isSuccess = true;
        this.actionOnPortionElems(function (index, elem) {
            var container = GapsTrainerController.getContainerByElem(elem);

            var textForUser = elem.TextForUser;
            var originalText = GapsTrainerController.getOriginalTextByElem(elem);
            var indexGap = -1;
            var inputs = container.find(GapsTrainerController.INPUT_SELECTOR);
            $.each(inputs, function (jIndex, input) {
                indexGap = getIndexGap(textForUser, indexGap + 1);
                var val = $(input).val();
                if (val.length == 0) {
                    GapsTrainerController.setContainerStyle(input, false);
                    isSuccess = false;
                    return true;
                }

                var rightChar = originalText.charAt(indexGap).toLowerCase();
                if (val.toLowerCase() == rightChar) {
                    GapsTrainerController.setContainerStyle(input, true);
                } else {
                    GapsTrainerController.setContainerStyle(input, false);
                    isSuccess = false;
                }
                
                return true;
            });
            
            if (isSuccess) {
                var msg = 'Ошибок нет. Отличная работа!';
                if (GapsTrainerController.isAnswerClicked) {
                    msg += ' Попробуйте в следующий раз не смотреть ответ.';
                } else if (GapsTrainerController.isTranslationClicked) {
                    msg += ' Попробуйте в следующий раз не смотреть перевод.';
                } else {
                    msg += ' Продолжайте в том же духе.';
                }
                Global.showMessage(GapsTrainerController.MESSAGE_CONTAINER_ID, true, msg);
            } else {
                Global.showMessage(GapsTrainerController.MESSAGE_CONTAINER_ID, false, 'Вы допустили ошибки. Обратите внимание на ячейки подсвеченные красным цветом');
            }
            return true;
        });
    },
    
    LoadNextPortion: function () {
        var newIndex = this.index + this.count;
        if (newIndex < this.elems.length) {
            //есть, что показать
            this.index = newIndex;
            this.showElements();
        }
        this.toogleLoadNextBtn();
    }
};

$(function() {
    GapsTrainerController.Init();
});