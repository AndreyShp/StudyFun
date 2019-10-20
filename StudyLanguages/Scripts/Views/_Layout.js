Global = {
    setCookie: function(name, value, options) {
        //установить куку, на 20 лет
        options = $.extend({ expires: 7300, path: '/' }, options);
        $.cookie(name, value, options);
    },
    getCookie: function(name) {
        return $.cookie(name);
    },
    escapeHTML: function (text) {
        return $('<div/>').text(text).html();
    },
    initLeftPinnedPanel: function(topPanel) {
        var panel = $('.left-pinned-panel');
        $(window).scroll(function() {
            var top = $(window).scrollTop();
            var height = $(window).height();
            var maxScroll = topPanel.height() + height * 0.05;

            var panelTop = topPanel.offset().top;
            var paddingTop;
            if (panelTop >= top) {
                paddingTop = topPanel.height();
            } else {
                paddingTop = 0;
            }
            paddingTop += 5;
            panel.css('padding-top', paddingTop);
            if (top > maxScroll) {
                panel.show();
            } else {
                panel.hide();
            }
        });
        panel.click(function() {
            $("html, body").animate({ scrollTop: 0 }, "fast");
        });
    },
    setInputStyle: function(input, isShowError) {
        input[isShowError === true ? "addClass" : "removeClass"]("alert-danger");
    },
    isValidEmailAddress: function(email) {
        var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
        return pattern.test(email);
    },
    focusClearField: function (field) {
        field
            .on('input paste', $.proxy(Global.setInputStyle, null, field))
            .on('click', $.proxy(Global.setInputStyle, null, field));
    },
    showMessage: function (idContainer, success, text) {
        var options = { isSuccess: success, removeLabels: true, container: $(idContainer), text: text };
        var msgInner = new Message(options);
        msgInner.Show();
    }
};

String.format = function() {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }
    return s;
};

SharePanel = {
    replaceParamInUrl: function(url, param, value) {
        var regExp = new RegExp(param + '=[^&]+', 'gi');
        var result = url.replace(regExp, param + '=' + encodeURIComponent(value));
        return result;
    },
    Update: function(url, title) {
        var sharesBtns = $('.b-share a');
        $.each(sharesBtns, function(index, shareElem) {
            var shareBtn = $(shareElem);
            var shareUrl = shareBtn.attr('href');
            shareUrl = SharePanel.replaceParamInUrl(shareUrl, 'url', url);
            shareUrl = SharePanel.replaceParamInUrl(shareUrl, 'title', title);
            shareBtn.attr('href', shareUrl);
        });
    }
};

SearchPanel = function() {
    if (window.SearchPanelData == null) {
        //на этой странице нет панели поиска
        return null;
    }

    this.Search = function() {
        var value = this.searchField.val();
        this.select(value);

        return false;
    };

    this.RemoveFilter = function() {
        this.searchField.val('');
        this.Search();

        return false;
    };

    this.select = function(item) {
        //отфильтрованы результаты или нет
        var foundElems = 0;

        $.each(this.elems, function(index, elem) {
            var foundIndex = elem.LowerName.indexOf(item);
            if (foundIndex == -1) {
                //TODO: поиск по ассоциациям
                foundElems++;
                elem.container.hide();
                return true;
            }
            elem.container.show();
            return true;
        });

        if (foundElems >= 1) {
            this.filteredIcon.show();
        } else {
            this.filteredIcon.hide();
        }
    };

    this.SortBy = function(type) {
        Global.setCookie(SearchPanelData.SortCookieName, type);
        location.reload();
        return false;
    };

    this.elems = SearchPanelData.Elements;
    if (this.elems == null || this.elems.length == 0) {
        return null;
    }

    var typeAheadSource = [];
    $.each(this.elems, function(index, elem) {
        var elemId = String.format(SearchPanelData.PatternToSearchContainer, elem.Id);
        elem.container = $('#' + elemId);

        typeAheadSource.push(elem.LowerName);
    });

    this.searchField = $('#' + SearchPanelData.SearchField);
    this.searchField.focus();
    this.searchField.typeahead({ source: typeAheadSource, updater: $.proxy(this.select, this) });

    this.filteredIcon = $('#' + SearchPanelData.FilteredIcon);
    return this;
}();

LanguagePanel = {
    fromLanguage: null,
    toLanguage: null,
    observers: [],
    IsShowed: false,
    getImageTag: function(language) {
        var imageName = ServerData.GetPath('Content/images/' + language.Image);
        return '<img src="' + imageName + '" alt="' + language.Title + '" class="country-flag-icon img-rounded" />';
    },
    OnChangeLanguage: function() {
        var newToLanguage = this.fromLanguage;
        var newFromLanguage = this.toLanguage;
        this.SetLanguages(newFromLanguage, newToLanguage);
        Global.setCookie('languages', newFromLanguage.Id + ';' + newToLanguage.Id);
        $.each(this.observers, function(index, observer) {
            observer(newFromLanguage.Id, newToLanguage.Id);
        });
    },
    AddObserver: function(observer) {
        this.observers.push(observer);
    },
    SetLanguages: function(fromLanguage, toLanguage) {
        this.fromLanguage = fromLanguage;
        this.toLanguage = toLanguage;
        var imageFrom = this.getImageTag(fromLanguage);
        var imageTo = this.getImageTag(toLanguage);
        $('#languagesPanel').html(imageFrom + '<span class="glyphicon glyphicon-chevron-right" style="font-size:11px;"></span>' + imageTo);
    },
    GetLanguages: function() {
        return { from: this.fromLanguage, to: this.toLanguage };
    },
    GetSourceLanguageId: function() {
        return this.fromLanguage.Id;
    },
    ShowPanel: function() {
        this.IsShowed = true;
        $('#languagesPanel').show();
    }
};

LanguageObserverHelper = function(options) {
    var action = (options != null ? options.action : null) || function() {
    };

    var initialSourceLanguageId = ServerData.Languages.from.Id;
    var rowsInfo = [];
    var changeLanguageObserver = function(sourceLanguageId) {
        var sourceProperty;
        var translationProperty;
        if (initialSourceLanguageId == sourceLanguageId) {
            sourceProperty = 'source';
            translationProperty = 'translation';
        } else {
            translationProperty = 'source';
            sourceProperty = 'translation';
        }
        $.each(rowsInfo, function(index, rowInfo) {
            rowInfo.sourceElem.html(rowInfo[sourceProperty]);
            rowInfo.translationElem.html(rowInfo[translationProperty]);

            action(rowInfo.sourceElem, rowInfo.translationElem);
        });
    };

    this.AddRow = function(row, sourceLanguageId) {
        var sourceElem = $(row).find('.source');
        var translationElem = $(row).find('.translation');

        var currentLanguageId = LanguagePanel.GetSourceLanguageId();
        if ($.isNumeric(sourceLanguageId) && sourceLanguageId != currentLanguageId) {
            var translationHtml = translationElem.html();
            var sourceHtml = sourceElem.html();

            translationElem.html(sourceHtml);
            sourceElem.html(translationHtml);
        }

        action(sourceElem, translationElem);

        rowsInfo.push({
            sourceElem: sourceElem,
            source: sourceElem.html(),
            translationElem: translationElem,
            translation: translationElem.html()
        });
    };

    //подписаться на изменение языка
    LanguagePanel.AddObserver(changeLanguageObserver);
};

PopupAlert = {
    visibleMessages: [],
    Show: function(opt) {
        var option = $.extend({ text: 'Что-то не так:(', header: 'Ошибка' }, typeof opt == "string" ? { text: opt } : opt);

        var txt = option.text;
        if (!txt || $.inArray(txt, PopupAlert.visibleMessages) != -1) {
            //либо текст не указан, либо такое сообщение уже есть
            return;
        }
        PopupAlert.visibleMessages.push(txt);

        var alertContent = $('<div class="alert alert-danger fade in popup-alert-ctrl">'
            + '<button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>'
            + '<h4>' + option.header + '</h4>'
            + '<p>' + txt + '</p>'
            + '</div>').bind('closed.bs.alert', function() {
                PopupAlert.visibleMessages.splice($.inArray(txt, PopupAlert.visibleMessages), 1);
            });

        var minHeight = 0;
        if (GlobalBusiness != null && GlobalBusiness.TopPanel != null) {
            minHeight = GlobalBusiness.TopPanel.GetHeight();
        }
        $.each($('.popup-alert-ctrl'), function(index, elem) {
            var height = $(elem).offset().top + $(elem).outerHeight();
            var maxHeight = $(window).height() + alertContent.height();
            if (height > minHeight && height < maxHeight) {
                minHeight = height;
            }
        });

        if (minHeight > 0) {
            alertContent.css('top', minHeight + 10 + 'px');
        }
        $('body').append(alertContent);
    }
};

ToggleTranslateBtn = function(opt) {
    var btn = new ToggleBtn({
        block: opt.translated,
        btn: opt.translatedToogle,
        cookieName: opt.skipCookie !== 'true' ? 'hideTranslation' : null,
        captionShow: 'Показать перевод &raquo;',
        captionHide: 'Спрятать перевод &laquo;',
        isHidden: opt.isHidden
    });

    this.Toogle = function() {
        return btn.Toogle();
    };

    this.IsVisible = function() {
        return btn.IsVisible();
    };

    this.Hide = function() {
        return btn.Hide();
    };
};

ToggleBtn = function(opt) {
    var options = opt;
    var hideOrShow = function(needToHide) {
        var btnLabel;
        if (needToHide) {
            btnLabel = options.captionShow;
        } else {
            btnLabel = options.captionHide;
        }
        options.block.toggle();
        options.btn.html(btnLabel);
    };

    this.Toogle = function() {
        var needToHide = this.IsVisible();
        if (options.cookieName != null) {
            Global.setCookie(options.cookieName, needToHide);
        }
        hideOrShow(needToHide);
        return false;
    };

    var needHideTransalation = options.cookieName != null ? Global.getCookie(options.cookieName) : false;
    if (needHideTransalation === "true" || options.isHidden === true) {
        hideOrShow(true);
    }

    this.IsVisible = function() {
        return options.block.is(':visible');
    };

    this.Hide = function() {
        if (this.IsVisible()) {
            hideOrShow(true);
        }
    };
};

Speaker = {
    audio: null,
    Speak: function(id, type) {
        if (this.audio != null) {
            this.audio.pause();
        }

        this.audio = new Audio();
        var mp3Support = this.audio.canPlayType('audio/mp3');
        var isSupportMp3 = mp3Support != '' && mp3Support != 'no';
        if (!isSupportMp3) {
            PopupAlert.Show('Извините, мы не можем воспроизвести mp3-файл в вашем браузере:( Весь функционал сайта работает в браузерах Firefox и Chrome.');
        }

        var url = ServerData.SpeakerHelper.GetUrl(id, type, isSupportMp3);
        this.audio.setAttribute("src", url);
        this.audio.play();
    },

    SpeakIfNeed: function(elem, type) {
        var result = this.NeedSpeak(elem);
        if (result) {
            this.Speak(elem.Id, type);
        }
        return result;
    },

    GetWordHtml: function(word) {
        return this.GetHtml(word, ServerData.SpeakerHelper.Ids.Word);
    },

    GetSentenceHtml: function(sentence) {
        return this.GetHtml(sentence, ServerData.SpeakerHelper.Ids.Sentence);
    },

    GetHtml: function(elem, type) {
        var html = this.NeedSpeak(elem) ? String.format(ServerData.SpeakerHelper.GetHtml(elem.Id, type)) : '';
        return html + elem.Text;
    },

    NeedSpeak: function(elem) {
        return elem.HasPronunciation;
    },

    SpeakElemPartByLanguage: function(isChecked, elem, speakerType) {
        if (!isChecked || !elem) {
            return;
        }

        var sourceLanguageId = LanguagePanel.GetSourceLanguageId();
        var source;
        var translation;
        if (elem.Source.LanguageId == sourceLanguageId) {
            source = elem.Source;
            translation = elem.Translation;
        } else {
            source = elem.Translation;
            translation = elem.Source;
        }
        var speakElem;
        if (Speaker.NeedSpeak(source)) {
            speakElem = source;
        } else {
            speakElem = translation;
        }
        Speaker.SpeakIfNeed(speakElem, speakerType);
    }
};

GlobalBusiness = {
    markAsShowed: function(url, oldId, needTryAgain) {
        $.post(url, { id: oldId }, function(data) {
            var success = data != null && data.success;
            if (!success && needTryAgain !== false) {
                //пробуем послать еще раз
                GlobalBusiness.markAsShowed(url, oldId, false);
            }
        }, "json");
    },

    setMetaTag: function(nameOfTag, content) {
        $("meta[name='" + nameOfTag + "']").attr('content', content);
    },

    setKeywords: function(content) {
        GlobalBusiness.setMetaTag('keywords', content);
    },

    setDescription: function(content) {
        GlobalBusiness.setMetaTag('description', content);
    },

    showExamplesInWindow: function(text, examples) {
        var body = '<ol>';
        $.each(examples, function(i, example) {
            body += '<li>' + example + '</li>';
        });
        body += '<ol>';
        var prettyWindow = new PrettyWindow({ header: text, body: body });
        prettyWindow.Show();
    },

    newVisitor: function(urlNewVisitor) {
        setTimeout(function() {
            var url = ServerData.GetPath(urlNewVisitor);
            $.post(url);
        }, 10000);
    }, /*

    getTopNavPanel: function() {
        return $('div.navbar');
    }*/
    
    TopPanel: new function () {
        var panel = $('div.navbar');
        this.GetPanel = function() {
            return panel;
        };
        
        this.GetHeight = function() {
            return panel.height();
        };

        var panelHeight = this.GetHeight();
        var windowHeight = $(window).height();
        this.IsFixedToTop = panelHeight < windowHeight * 0.5;
        if (this.IsFixedToTop) {
            //высота меню не превышает 50% экрана - фиксировать "приклеить" панель кверху
            $('body').css('padding-top', '60px');
            panel.removeClass('navbar-static-top');
            panel.addClass('navbar-fixed-top');
            panel.affix();
        }
    },

    Counter: {
        Ids: {
            AddKnowledge: 'ADD_KNOWLEDGE',
            Download: 'DOWNLOAD',
            MyKnowledge: 'MY_KNOWLEDGE',
            FeedbackPost: 'FEEDBACK_POST',
            InterviewWindow: 'INTERVIEW_WINDOW',
            ProfileChangeUnique: 'PROFILE_CHANGE_UNIQUE',
            ProfileSendUnique: 'PROFILE_SEND_UNIQUE',
            UserTaskNew: 'USER_TASK_NEW',
            UserTaskNewComment: 'USER_TASK_NEW_COMMENT',
            BuyClick: 'BUY_CLICK',
            PayClick: 'PAY_CLICK',
            CloseTopBanner: 'CLOSE_TOP_BANNER'
        },
        reachGoal: function (name, params) {
            if (!ServerData.YandexMetrikaId) {
                return;
            }

            var counter = window['yaCounter' + ServerData.YandexMetrikaId];
            if (counter) {
                counter.reachGoal(name, params);
            }
        }
    },
    
    Interview: function (interviewBtn) {
        var INTERVIEW_WINDOW = '#interviewWindow';
        var INTERVIEW_WINDOW_CONTAINER = '#interviewWindowContainer';
        var INTERVIEW_COOKIE_NAME = 'interviewCookie';

        var alreadyInterviewed = Global.getCookie(INTERVIEW_COOKIE_NAME);
        if (alreadyInterviewed) {
            return;
        }

        var html = '<div class="modal fade" id="interviewWindow" role="dialog" aria-labelledby="interviewWindowLabel" aria-hidden="true">'
        + '<div class="modal-dialog">'
        + '<div class="modal-content">'
        + '<div class="modal-header">'
        + '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>'
        + '<h4 class="modal-title" id="interviewWindowTitle">Анкета опроса</h4>'
        + '</div>'
        + '<div class="modal-body modal-scrollable" id="interviewWindowContainer">'
        + '</div>'
        + '<div class="modal-footer">'
        + '<button type="button" class="btn btn-default" data-dismiss="modal" tabindex="3">Закрыть</button>'
        + '<button type="button" class="btn btn-success" id="interviewPostBtn" tabindex="4">Голосовать</button>'
        + '</div>'
        + '</div>'
        + '</div>'
        + '</div>';

        $('body').append(html);

        $('#interviewPostBtn').click(function () {
            var checkedAnswersIds = [];
            var checkedAnswers = $('[data-interview-answer]:checked');
            $.each(checkedAnswers, function(i, checkedAnswer) {
                var answerId = $(checkedAnswer).attr('data-interview-answer');
                checkedAnswersIds.push(answerId);
            });

            if (checkedAnswersIds.length == 0) {
                $(INTERVIEW_WINDOW).modal('hide');
                return;
            }

            $.ajax({
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                type: 'POST',
                url: ServerData.Interview.AnswerInterviewUrl,
                data: '{answersIds:' + JSON.stringify(checkedAnswersIds) + '}',
                success: $.proxy(function (response) {
                    var success = response != null && response.success;
                    if (success) {
                        Global.setCookie(INTERVIEW_COOKIE_NAME, true, { expires: null });
                        interviewBtn.hide();
                    }
                    $(INTERVIEW_WINDOW).modal('hide');
                }, this)
            });
        });

        var showWindow = function (items) {
            var htmlInterview = '';
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                htmlInterview += String.format('<span class="interview-question">{0}</span><div class="interview-answers-container">', item.Question);
                for (var j = 0; j < item.Answers.length; j++) {
                    var answer = item.Answers[j];
                    htmlInterview +=
                        String.format('<div class="checkbox"><label><input type="checkbox" data-interview-answer="{0}"> {1}</label></div>', answer.Id, answer.Text);
                }
                htmlInterview += '</div></div>';
            }
            
            $(INTERVIEW_WINDOW_CONTAINER).html(htmlInterview);
            $(INTERVIEW_WINDOW).modal('show');
        };

        $.getJSON(ServerData.Interview.GetInterviewsUrl, function (response) {
            if (response == null || response.length == 0) {
                interviewBtn.hide();
                return;
            }

            interviewBtn.css('display', 'inline-block');
            interviewBtn.click(function () {
                showWindow(response);
                GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.InterviewWindow);
            });
        });
    },
    
    maxLengthAttention: function(config) {
        if (!config || !config.field) {
            return;
        }

        var input = config.field;
        var maxLength = input.attr('maxLength');

        if (!maxLength || isNaN(maxLength)) {
            return;
        }

        var parsedMaxLength = parseInt(maxLength);
        var getRemainCountChars = function() { return parsedMaxLength - input.val().length; };
        
        var remainCountCharsCtrlId = input.attr('id') + 'RemainCountChars';

        var attentionMsg = String.format('<div class="max-length-attention-message">Максимальная длина {0}{1} символов. У вас осталось <span id="{2}">{3}</span></div>',
            (config.inputTitle ? config.inputTitle + ' ' : ''), maxLength, remainCountCharsCtrlId, getRemainCountChars());
        $(attentionMsg).insertAfter(input);

        input.on('keyup', function () {
            var remainCountChars = getRemainCountChars();
            $('#' + remainCountCharsCtrlId).text(remainCountChars);
        });
    },
    
    shuffle: function (elemsToShuffle) {
        var result = [];

        var getRandomIndex = function (unwantedIndex) {
            for (var i = 0; i < 3; i++) {
                var rndIndex = Math.random() * elemsToShuffle.length;
                var randomIndex = parseInt(rndIndex);
                if (randomIndex != unwantedIndex) {
                    return randomIndex;
                }
            }
            return unwantedIndex;
        };

        while (elemsToShuffle.length > 0) {
            var index = getRandomIndex(result.length);
            var elemToMove = elemsToShuffle[index];
            result.push(elemToMove);
            //удалить элемент из массива
            elemsToShuffle.splice(index, 1);
        }
        
        return result;
    },
    
    closeTopBanner: function (containerSelector, cookieName) {
        Global.setCookie(cookieName, true);
        $(containerSelector).remove();
        GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.CloseTopBanner);
    },
    
    closeTopMessage: function (containerSelector, cookieName) {
        Global.setCookie(cookieName, true);
        $(containerSelector).remove();
    },
    
    findById: function (id, elems) {
        elems = elems || (ServerData.Elements || []);
        
        var result = null;
        $.each(elems, function (y, e) {
            var isFound = e.Id == id;
            if (isFound) {
                result = e;
            }
            return !isFound;
        });
        return result;
    }
};

UserKnowledgePopupPanel = function (topPanel) {
    var initialUrl = ServerData.PopupTrainerInitialUrl;
    if (initialUrl == null) {
        return;
    }

    var setOffset = function (elem) {
        var offset = {
            //5 и 45 сдвиги влево и вверх по умолчанию
            left: $(window).width() - $(elem).width() - 15,
            top: topPanel.height() + 5
        };
        elem.offset(offset);
    };

    var createIcon = function () {
        var result = $(
            '<div class="popup-knowledge-repeat-btn" title="У вас есть знания, которые нужно повторить" style="display:none;">' +
                '<span class="glyphicon glyphicon-time"></span>' +
            '</div>');
        $('body').append(result);

        setOffset(result);
        return result;
    };

    var createWindow = function(html, onHide) {
        var closeBtnId = 'popupKnowledgeClose';
        var result = $(
            '<div id="popupKnowledgePanel" class="modal-dialog popup-knowledge-panel" id="popupPanel" style="display:none;">' +
                '<div class="modal-content">' +
                '<div class="modal-header">' +
                '<button type="button" class="close" id="' + closeBtnId + '">&times;</button>' +
                '<h4 class="modal-title" id="myModalLabel">Повторяем знания</h4>' +
                '</div>' +
                '<div class="modal-body text-center">' + html + '</div>' +
                '</div>' +
                '</div>');

        $('body').append(result);

        setOffset(result);
        //TODO: подписаться на событие перетаскивания и сохранять координаты + восстанавливать координаты
        result.draggable();

        $('#' + closeBtnId).click(function() {
            result.hide({ complete: onHide });
        });
        return result;
    };

    var loadData = $.proxy(function () {
        this.smartTrainer.ClearItems();
        this.icon.hide();
        this.popupLanguagePanel.hide();
        
        $.getJSON(ServerData.PopupTrainerLoadDataUrl, $.proxy(function (response) {
            if (response == null || response.length == 0) {
                //продолжать пинговать сервер
                setTimeout(loadData, 10000);
                return;
            }
            this.smartTrainer.SetItems(response);
            //отобразить тренажер с данными
            this.icon.show();
        }, this));
    }, this);

    $.getJSON(initialUrl, $.proxy(function(response) {
        if (response == null || response.html == null) {
            return;
        }

        var items = response.items;
        
        this.icon = createIcon();
        this.popupLanguagePanel = createWindow(response.html, $.proxy(function () {
            if (this.smartTrainer.HasItems()) {
                this.icon.show();
            }
        }, this));
        this.icon.click($.proxy(function () {
            this.icon.hide({
                duration: 200,
                complete: $.proxy(function () {
                    if (this.smartTrainer.HasItems()) {
                        this.popupLanguagePanel.show();
                    }
                }, this)
            });
            GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.MyKnowledge);
        }, this));

        this.smartTrainer = new SmartTrainer({
            setMarkUrl: ServerData.PopupTrainerSetMarkUrl,
            sourceLanguageId: response.sourceLanguageId,
            OnEmptyData: loadData,
            loop: false,
            repetitionType: ServerData.PopupTrainerRepetitionType
        });

        if (items.length > 0) {
            this.smartTrainer.SetItems(items);
            this.icon.show();
        } else {
            loadData();
        }
        
        $(window).resize($.proxy(function () {
            setOffset(this.icon);
            setOffset(this.popupLanguagePanel);
        }, this));
    }, this));
};

SmartTrainer = function(options) {
    this.sourceLanguageId = options.sourceLanguageId;
    this.currentItem = null;
    var repetitionType = options.repetitionType;

    this.containers = {
        source: $('#trainerSourceContainer'),
        translation: $('#trainerTranslationContainer'),
        panel: $('#trainerPanelBtns'),
        translatedBtn: $('#trainerTranslationToogleId'),
        image: $('#imageContainerId')
    };
    
    var hasImage = this.containers.image.length > 0;
    this.translationToogleBtn = new ToggleTranslateBtn({
        translated: this.containers.translation,
        translatedToogle: this.containers.translatedBtn,
        skipCookie: true,
        isHidden: true
    });

    var setItem = $.proxy(function (index) {
        if (index >= this.items.length) {
            return false;
        }

        this.index = index;
        this.currentItem = this.items[index];
        if (hasImage && this.currentItem.ImageUrl) {
            this.containers.image.attr('src', this.currentItem.ImageUrl);
            this.containers.image.show();
        } else if (hasImage) {
            this.containers.image.hide();
        }
        
        var sourceHtml = this.currentItem.HtmlSource;
        var translationHtml = this.currentItem.HtmlTranslation;
        if (this.sourceLanguageId != null && this.currentItem.SourceLanguageId != this.sourceLanguageId) {
            sourceHtml = this.currentItem.HtmlTranslation;
            translationHtml = this.currentItem.HtmlSource;
        }

        this.containers.source.html(sourceHtml);
        this.containers.translation.html(translationHtml);
        return true;
    }, this);

    this.items = [];

    //если есть языковая панель - подписаться на изменение языка
    if (LanguagePanel.IsShowed) {
        LanguagePanel.AddObserver($.proxy(function(sourceLanguageId) {
            this.sourceLanguageId = sourceLanguageId;
            setItem(this.index);
        }, this));
    }
    this.containers.panel.show();

    this.containers.translatedBtn.click($.proxy(function() {
        this.translationToogleBtn.Toogle();
    }, this));

    var onEmptyData = options.OnEmptyData;
    var mergeItems = $.proxy(function(serverItems) {
        if (serverItems == null || serverItems.length == 0) {
            if (onEmptyData != null) {
                onEmptyData();
            }
            return;
        }
        
        var items = this.items;
        $.each(items, $.proxy(function(itemIndex, item) {
            item.NeedRemove = true;
            return true;
        }, this));

        //объединяем данные сохраненные на клиенте с теми, которые пришли с сервера
        $.each(serverItems, function(serverItemIndex, serverItem) {
            var isFound = false;
            serverItem.TimeToShow = new Date(serverItem.NextTimeToShow);
            $.each(items, function(itemIndex, item) {
                isFound = item.DataId == serverItem.DataId;
                if (isFound) {
                    items[itemIndex] = serverItem;
                }
                return !isFound;
            });
            if (!isFound) {
                //что-то новенькое - такого не было
                items.push(serverItem);
            }
        });

        //удаляем те, которые не пришли в этот раз с сервера
        for (var i = items.length - 1; i >= 0; i--) {
            var item = items[i];
            if (item.NeedRemove === true) {
                items.splice(i, 1);
            }
        }

        //упорядочиваем
        items.sort(function(item1, item2) {
            return item1.NextTimeToShow - item2.NextTimeToShow;
        });

        //обновляем this.index
        $.each(items, $.proxy(function(index, it) {
            var needContinue = true;
            if (/*it.NextTimeToShow <= this.currentItem.NextTimeToShow &&*/ it.DataId != this.currentItem.DataId) {
                //отобразить первый элемент у которого время для показа, меньше чем время у текущего элемента
                this.index = index - 1;
                needContinue = false;
            }
            return needContinue;
        }, this));

        /*//TODO: добавил для отладки
        $.each(this.items, function(idx, item) {
            console.log(item.DataId + ' ' + item.TimeToShow);
        });
        console.log('index: ' + this.index);*/
    }, this);

    var loop = options.loop;
    var setMarkUrl = options.setMarkUrl;
    this.SetMark = function(mark) {
        if (this.currentItem == null) {
            return;
        }

        this.translationToogleBtn.Hide();

        var item = { DataId: this.currentItem.DataId, DataType: this.currentItem.DataType };
        var dataToPost = '{mark:' + mark + ',item:' + JSON.stringify(item);
        if (repetitionType != null) {
            dataToPost += ',repetitionType:' + repetitionType;
        }
        dataToPost += '}';

        //показать следующие данные
        var hasElement = setItem(this.index + 1);
        if (!hasElement) {
            if (loop) {
                //начинаем показывать сначала
                setItem(0);
            } else {
                //данных нет
                onEmptyData();
            }
        }

        $.ajax({
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            url: setMarkUrl,
            data: dataToPost,
            success: $.proxy(function (data) {
                if (data.success !== false) {
                    mergeItems(data.items);
                } else {
                    var message = (data != null ? data.message : null) || 'Извините, что-то пошло не так и мы не смогли поставить оценку:(. Попробуйте позже.';
                    PopupAlert.Show(message);
                }
            }, this)
        });
    };

    this.SetItems = function (items) {
        if (this.items.length > 0) {
            mergeItems(items);
        } else {
            this.items = items;
        }
        setItem(0);
    };

    this.ClearItems = function () {
        this.items = [];
        this.currentItem = null;
    };

    this.HasItems = function() {
        return this.items.length > 0;
    };

    var setMark = $.proxy(this.SetMark, this);
    var btns = this.containers.panel.find('[data-mark]');
    $.each(btns, $.proxy(function(index, btn) {
        $(btn).click(function() {
            setMark($(this).attr('data-mark'));
        });
    }, this));
};

UserKnowledgeShortInfo = function() {
    var idTodayCount = 'knowledgeTodayCount';
    var idTotalCount = 'knowledgeTotalCount';

    var setCount = function(countToday, countTotal) {
        var styleGetter = function(val) {
            return ' style="color:' + (val > 0 ? '#468847' : '#B94A48') + '"';
        };

        var html = '<span id="' + idTodayCount + '"' + styleGetter(countToday) + ' title="Кол-во ваших знаний за сегодня">' + countToday + '</span>'
            + '<span style="color:#999;">/</span>'
            + '<span id="' + idTotalCount + '"' + styleGetter(countTotal) + ' title="Кол-во ваших знаний за все время">' + countTotal + '</span>';
        $('#knowledgeShortInfo').html(html);
    };

    this.Display = function() {
        var url = ServerData.GetPath(ServerData.Knowledge.GetStatisticUrl);
        $.getJSON(url, $.proxy(function(response) {
            if (response == null || response.Today == null || response.Total == null) {
                setCount(0, 0);
                return;
            }

            var countToday = response.Today.Total || 0;
            var countTotal = response.Total.Total || 0;
            setCount(countToday, countTotal);
        }, this));
    };

    this.Increment = function() {
        var todayCountCtrl = $('#' + idTodayCount);
        var totalCountCtrl = $('#' + idTotalCount);

        var todayCount = parseInt(todayCountCtrl.html());
        var totalCount = parseInt(totalCountCtrl.html());
        if (!isNaN(todayCount) && !isNaN(totalCount)) {
            todayCountCtrl.html();
            totalCountCtrl.html;
            setCount(todayCount + 1, totalCount + 1);
        }
    };
};

UserKnowledge = {
    savedInfo: null,
    okIds: [],
    ShortInfo: new UserKnowledgeShortInfo(),
    
    GetManySelectorByType: function (type) {
        return "[data-knowledge-many='" + type + "']";
    },
    
    GetSelectorByType: function (type) {
        return "[data-knowledge-type='" + type + "']";
    },

    Add: function(ctrl) {
        var url = ServerData.Knowledge != null ? ServerData.Knowledge.AddUrl : null;
        if (!url) {
            return;
        }

        var knowledgeItem = this.getKnowlegeItem(ctrl);
        var dataId = knowledgeItem.DataId;
        var dataType = knowledgeItem.DataType;
        $.ajax({
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            url: ServerData.GetPath(url),
            data: '{ knowledgeItem:' + JSON.stringify(knowledgeItem) + '}',
            success: $.proxy(function(data) {
                var success = data != null && data.success;
                if (success) {
                    this.incrementIfNeed(ctrl);
                    this.setOkIconToCtrl(ctrl);
                    this.setOkIconToManyIfNeed(dataType);
                } else {
                    var message = (data != null ? data.message : null) || 'Извините, мы не смогли добавить данные на обучение. Попробуйте позже.';
                    PopupAlert.Show(message);
                }
            }, this)
        });

        GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.AddKnowledge, { id: dataId, type: dataType });
    },

    AddMany: function(dataType) {
        var url = ServerData.Knowledge != null ? ServerData.Knowledge.AddManyUrl : null;
        if (!url) {
            return;
        }

        var controls = $(UserKnowledge.GetSelectorByType(dataType));
        var knowledgeItems = [];
        var getItem = $.proxy(this.getKnowlegeItem, this);
        $.each(controls, function(i, ctrl) {
            var knowledgeItem = getItem(ctrl);
            knowledgeItems.push(knowledgeItem);
        });

        $.ajax({
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            url: ServerData.GetPath(url),
            data: '{ knowledgeItems:' + JSON.stringify(knowledgeItems) + '}',
            success: $.proxy(function(data) {
                var success = data != null && data.success;
                if (success) {
                    $.each(controls, $.proxy(function(i, ctrl) {
                        this.incrementIfNeed(ctrl);
                        this.setOkIconToCtrl(ctrl);
                    }, this));

                    this.setOkIconToManyIfNeed(dataType);
                } else {
                    var message = (data != null ? data.message : null) || 'Извините, мы не смогли добавить данные на обучение. Попробуйте позже.';
                    PopupAlert.Show(message);
                }
            }, this)
        });
        
        GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.AddKnowledge, { id: 'many', type: dataType });
    },

    getKnowlegeItem: function(ctrl) {
        var id = this.getDataIdByCtrl(ctrl);
        var type = $(ctrl).data('knowledgeType');
        var knowledgeItem = { DataId: id, DataType: type };
        return knowledgeItem;
    },

    getDataIdByCtrl: function(ctrl) {
        return $(ctrl).data('knowledgeId');
    },

    RebindClick: function (id, selector) {
        $(selector).removeAttr('onClick').unbind('click').click(function () {
            $(this).data('knowledgeId', id);
            UserKnowledge.Add(this);
        });
    },

    ShowBtn: function(ctrl) {
        var selector = UserKnowledge.GetSelectorByType(ServerData.KnowledgeDataType);
        var actionPanel = $(ctrl).find(selector);
        actionPanel.show();
    },

    CheckExistenceIds: function() {
        var ids = [];
        var selector = UserKnowledge.GetSelectorByType(ServerData.KnowledgeDataType);
        var controls = $(selector);
        var getDataIdByCtrl = this.getDataIdByCtrl;
        $.each(controls, function(i, ctrl) {
            var id = getDataIdByCtrl(ctrl);
            ids.push(id);
        });

        this.Load(ids);
    },

    Load: function (ids, needSearchCurrentId) {
        var dataType = ServerData.KnowledgeDataType;
        if (dataType == null) {
            return;
        }

        if (needSearchCurrentId === true) {
            var ctrl = $(UserKnowledge.GetSelectorByType(dataType));
            var id = this.getDataIdByCtrl(ctrl);
            ids.push(id);
        }
        var getDataIdByCtrl = this.getDataIdByCtrl;
        var setOkIcon = $.proxy(this.setOkIconToCtrl, this);
        var url = ServerData.GetPath(ServerData.Knowledge.GetExistenceIdsUrl);
        $.ajax({
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            url: url,
            data: '{ids:' + JSON.stringify(ids) + ',dataType:' + dataType + '}',
            success: $.proxy(function (response) {
                var foundIds = response != null ? response : [];
                if (foundIds.length == null || foundIds.length == 0) {
                    return;
                }
                $.each(foundIds, $.proxy(function(index, id) {
                    this.addOkId(id);
                }, this));
                
                var controls = $(UserKnowledge.GetSelectorByType(dataType));
                $.each(controls, function(i, control) {
                    var idByControl = getDataIdByCtrl(control);
                    var foundIndex = $.inArray(idByControl, foundIds);
                    var isFound = foundIndex != -1;
                    if (isFound) {
                        setOkIcon(control);
                        foundIds.splice(foundIndex, 1);
                    }
                    return true;
                });

                this.setOkIconToManyIfNeed(dataType);
            }, this)
        });
    },

    SetAppropriateIcon: function (id) {
        var dataType = ServerData.KnowledgeDataType;
        var selector = UserKnowledge.GetSelectorByType(dataType);
        var ctrl = $(selector);
        var isOk = this.isInOkIds(id);
        if (isOk) {
            this.setOkIconToCtrl(ctrl);
        } else {
            var isBtnToAdd = this.isBtnToAdd(ctrl);
            if (isBtnToAdd) {
                return;
            }
            var title = this.savedInfo != null ? this.savedInfo.title : 'Добавить данные ко мне на обучение';
            $(ctrl).removeClass('glyphicon-ok').addClass('glyphicon-plus cursor-pointer').css('color', '').attr('title', title);
            this.RebindClick(id, selector);
        }
    },

    isBtnToAdd: function(ctrl) {
        return $(ctrl).hasClass('glyphicon-plus');
    },

    incrementIfNeed: function(ctrl) {
        if (this.isBtnToAdd(ctrl)) {
            this.ShortInfo.Increment();
        }
    },

    setOkIconToCtrl: function(ctrl) {
        if (this.savedInfo == null) {
            this.savedInfo = {
                title: $(ctrl).attr('title')
            };
        }

        var id = this.getDataIdByCtrl(ctrl);
        this.addOkId(id);

        $(ctrl).attr('onclick', '').unbind('click')
            .removeClass('glyphicon-plus cursor-pointer').addClass('glyphicon-ok').css('color', '#4CAE4C')
            .attr('title', 'Ранее было добавлено к Вам на стену знаний');
    },

    isInOkIds: function(id) {
        var foundIndex = $.inArray(id, this.okIds);
        return foundIndex != -1;
    },

    addOkId: function(id) {
        if (id != null && !this.isInOkIds(id)) {
            this.okIds.push(id);
        }
    },

    setOkIconToManyIfNeed: function (dataType) {
        var btn = UserKnowledge.GetSelectorByType(dataType);
        var hasBtnsToAdd = $(btn).hasClass('glyphicon-plus');
        if (hasBtnsToAdd) {
            return;
        }
        //меняем иконку для кнопки массового добавления
        var manyBtn = $(UserKnowledge.GetManySelectorByType(dataType));
        this.setOkIconToCtrl(manyBtn);
    },
    
    /*BindCtrlOnHover: function (ctrl) {
        var selector = UserKnowledge.GetSelectorByType(ServerData.KnowledgeDataType);
        var actionPanel = $(ctrl).find(selector);
        $(ctrl).mouseover(function () {
            actionPanel.css('visibility', 'visible');
        }).mouseleave(function () {
            actionPanel.css('visibility', 'hidden');
        });
    }*/
};

AutoPronounceCheckBox = function() {
    var AUTO_PRONOUNCE_CONTAINER_ID = '#autoPronounce';
    var AUTO_PRONOUNCE_COOKIE_NAME = 'autoPronounce';

    this.IsChecked = function() {
        return $(AUTO_PRONOUNCE_CONTAINER_ID).is(":checked");
    };

    var needPronounce = Global.getCookie(AUTO_PRONOUNCE_COOKIE_NAME);
    if (needPronounce == "true") {
        $(AUTO_PRONOUNCE_CONTAINER_ID).attr('checked', true);
    }

    $(AUTO_PRONOUNCE_CONTAINER_ID).click($.proxy(function() {
        var needAutoPronounce = this.IsChecked();
        Global.setCookie(AUTO_PRONOUNCE_COOKIE_NAME, needAutoPronounce, { expires: null });
    }, this));
};

Message = function (opt) {
    this.options = opt;
    this.Show = function () {
        if (this.options.removeLabels) {
            this.Remove();
        }
        var text = String.format('<span class="label label-{0} clickable-element" onClick="$(this).remove();">{1}</span>',
            this.options.isSuccess ? 'success' : 'danger', this.options.text);
        this.options.container.append(text);
        this.options.container.show();
    };

    this.Remove = function () {
        var existLabels = this.options.container.children('.label');
        existLabels.remove();
    };
};

PrettyWindow = function(options) {
    $('#prettyModalWindow').remove();

    var header = options.header;
    var body = options.body;
    var html = '<div id="prettyModalWindow" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">'
        + '<div class="modal-dialog">'
        + '<div class="modal-content">'
        + '<div class="modal-header">'
        + '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>'
        + '<h4 id="myModalLabel">' + header + '</h4>'
        + '</div>'
        + '<div class="modal-body">' + body + '</div>'
        + '<div class="modal-footer">'
        + '<button class="btn" data-dismiss="modal" aria-hidden="true">Закрыть</button>'
        + '</div>'
        + '</div>'
        + '</div>'
        + '</div>';

    $('body').append(html);
    
    this.Show = function() {
        $('#prettyModalWindow').modal('show');
    };
};

FeedbackWindow = new function () {
    var EMAIL_INPUT_ID = '#feedbackWindowEmail';
    var MESSAGE_INPUT_ID = '#feedbackWindowMessage';
    var POST_BTN_ID = '#feedbackPostBtn';
    var FEEDBACK_WINDOW = '#feedbackWindow';
    var MESSAGE_CONTAINER = '#feedbackMessageContainer';

    var enablePostBtn = function() {
        $(POST_BTN_ID).removeAttr('disabled');
    };
    
    var clearFields = function () {
        $(EMAIL_INPUT_ID).val('');
        $(MESSAGE_INPUT_ID).val('');
    };

    var changeErrorCheck = function (ctrl, isShowError) {
        Global.setInputStyle(ctrl, isShowError);
    };

    var showErrorMessage = function(text) {
        var msg = new Message({
            isSuccess: false,
            removeLabels: true,
            text: text,
            container: $(MESSAGE_CONTAINER)
        });
        msg.Show();
    };

    var postClick = function () {
        var message = $(MESSAGE_INPUT_ID).val().trim();
        if (message == '') {
            changeErrorCheck($(MESSAGE_INPUT_ID), true);
            showErrorMessage('Некорректные данные!<br />Введите сообщение');
            return;
        }
        
        var email = $(EMAIL_INPUT_ID).val().trim();
        if (email != '' && !Global.isValidEmailAddress(email)) {
            changeErrorCheck($(EMAIL_INPUT_ID), true);
            showErrorMessage('Некорректные данные!<br />Введите правильный адрес электронной почты');
            return;
        }

        email = Global.escapeHTML(email);
        message = Global.escapeHTML(message);
        
        $(POST_BTN_ID).attr('disabled', 'disabled');
        var url = ServerData.Feedback.PostUrl;
        $.post(url, { email: email, message: message }, $.proxy(function (data) {
            enablePostBtn();
            var success = data != null && data.success;
            var options = { isSuccess: success, removeLabels: true, container: $(MESSAGE_CONTAINER) };
            if (success) {
                options.text = 'Спасибо! Ваше сообщение успешно отправлено';
                clearFields();
            } else {
                options.text = 'Извините, но Ваше сообщение не удалось отправить.<br />Попробуйте еще раз через несколько секунд';
            }
            var msgInner = new Message(options);
            msgInner.Show();
        }, this), "json");
        
        GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.FeedbackPost);
    };

    var html = '<div class="modal fade" id="feedbackWindow" role="dialog" aria-labelledby="feedbackWindowLabel" aria-hidden="true">'
        + '<div class="modal-dialog">'
        + '<div class="modal-content">'
        + '<div class="modal-header">'
        + '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>'
        + '<h4 class="modal-title" id="feedbackWindow">Форма обратной связи</h4>'
        + '</div>'
        + '<div class="modal-body">'
        + '<textarea class="form-control feedback-window-text-area" rows="5" placeholder="Ваше сообщение" id="feedbackWindowMessage" autofocus tabindex="1"></textarea>'
        + '<div class="input-group">'
        + '<span class="input-group-addon">@</span>'
        + '<input class="form-control" type="text" placeholder="Адрес электронной почты(необязательно для заполнения)" id="feedbackWindowEmail" tabindex="2">'
        + '</div>'
        + '<span class="feedback-tip">(адрес электронной почты следует указывать, если хотите получить ответ)</span>'
        + '</div>'
        + '<div class="modal-footer">'
        + '<div id="feedbackMessageContainer" style="display:none;" class="text-left pull-left"></div>'
        + '<button type="button" class="btn btn-default" data-dismiss="modal" tabindex="3">Закрыть</button>'
        + '<button type="button" class="btn btn-success" id="feedbackPostBtn" tabindex="4">Отправить</button>'
        + '</div>'
        + '</div>'
        + '</div>'
        + '</div>';

    $('body').append(html);
    $(POST_BTN_ID).click(postClick);

    Global.focusClearField($(MESSAGE_INPUT_ID));
    $(MESSAGE_INPUT_ID).focus();

    Global.focusClearField($(EMAIL_INPUT_ID));
    $(EMAIL_INPUT_ID).focus();

    this.Show = function () {
        $(FEEDBACK_WINDOW).modal('show');
        enablePostBtn();
        clearFields();
        $(MESSAGE_INPUT_ID).focus();
        return false;
    };

    this.Hide = function() {
        $(FEEDBACK_WINDOW).modal('hide');
    };
}();

CopyToClipboard = function (options) {
    var btn = $(options.copyToClipboardBtnSelector);
    var path = ServerData.GetPath("Scripts/Controls/ZeroClipboard.swf");

    btn.attr("data-clipboard-text", $(options.textSelector).text());

    ZeroClipboard(btn, {
        moviePath: path,
        debug: false
    });
};

MailBlock = function(options) {
    var mailBlockSelector = options.mailBlockSelector;
    var showMessage = options.showMessage;
    var processedPost = options.processedPost;
    var url = options.url;
    
    var hideMailBlock = function() {
        $(mailBlockSelector).hide();
    };

    var emailField = $(options.emailFieldSelector);
    Global.focusClearField(emailField);

    $(options.showBtnBlockSelector).click(function() {
        var block = $(mailBlockSelector);
        block.toggle();
        emailField.focus();
    });

    $(options.sendBtnSelector).click(function() {
        var email = emailField.val().trim();
        if (email == '' || !Global.isValidEmailAddress(email)) {
            showMessage(false, 'Некорректные данные!<br />Введите правильный адрес электронной почты');
            Global.setInputStyle(emailField, true);
            return;
        }

        var dataToPost = { email: email };
        if (options.dataToPostModifier != null) {
            options.dataToPostModifier(dataToPost);
        }
        $.post(url, dataToPost, function(data) {
            var success = data != null && data.success;
            var text;
            if (success) {
                text = 'Вам на почту успешно отправлен идентификатор';
                hideMailBlock();
                emailField.val('');
            } else {
                text = 'Извините, не удалось отправить идентификатор. Проверьте правильность введенного адреса электронной почты.<br />' +
                    'Попробуйте еще раз через несколько секунд';
            }
            showMessage(success, text);
            if (processedPost != null) {
                processedPost(success);
            }
        }, "json");
    });
};

(function ($) {
    $.fn.loading = function (action) {
        if (action === "hide") {
            return this.html("").removeClass("loading");
        } else if (action === "show") {
            action = "Загрузка";
        }
        //считаем, что action это текст
        return this.html(action + "&hellip;").addClass("loading");
    };
}(jQuery));

$(function() {
    if (ServerData && ServerData.Languages) {
        LanguagePanel.SetLanguages(ServerData.Languages.from, ServerData.Languages.to);
        LanguagePanel.ShowPanel();
    }
    
    var topPanel = GlobalBusiness.TopPanel.GetPanel();
    var showContentBelowTopMenu = function() {
        var contentContainer = $('#contentContainer');
        var offset = contentContainer.offset();
        if (!offset) {
            return;
        }

        var navPanelHeight = topPanel.height() + 10;
        //var top = offset.top;
        //if (top < navPanelHeight) {
        var difference = Math.round(navPanelHeight - offset.top);
        offset.top = navPanelHeight;
        contentContainer.offset(offset);
        //}

        if (difference != 0) {
            //нижнюю панель сдвинуть на столько же
            var bottomPanel = $('.our-navbar-bottom');
            var bottomOffset = bottomPanel.offset();
            var marginTop = bottomPanel.css('padding-top').replace('px', '');
            if ($.isNumeric(marginTop)) {
                if (difference > 0) {
                    difference += parseInt(marginTop);
                } else {
                    difference -= parseInt(marginTop);
                }
            }
            bottomOffset.top += difference;
            bottomPanel.offset(bottomOffset);
        }
    };

    //нужно для IE
    showContentBelowTopMenu();
    $(window).resize($.proxy(function() {
        showContentBelowTopMenu();
    }));

    Global.initLeftPinnedPanel(topPanel);

    UserKnowledge.ShortInfo.Display();

    new UserKnowledgePopupPanel(topPanel);

    var interviewBtn = $('#interviewBtn');
    if (interviewBtn.length > 0) {
        GlobalBusiness.Interview(interviewBtn);
    }


    function removeOldCookie() {
        var cookieName = 'uniqueUser';
        var id = Global.getCookie(cookieName);

        var countCookies = 0;
        var theCookies = document.cookie.split(';');
        for (var i = 1 ; i <= theCookies.length; i++) {
            var cookie = theCookies[i - 1];

            if (cookie.indexOf(cookieName) > -1) {
                countCookies++;
            }
        }

        if (countCookies > 1) {
            Global.setCookie(cookieName, id, { domain: '', path: '/', expires: -1 });
        }
    }

    removeOldCookie();
});