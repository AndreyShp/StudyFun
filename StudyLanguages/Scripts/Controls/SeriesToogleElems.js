SeriesToogleElems = function (options) {
    var containers = {
        source: $('#sourceContainerId'),
        translated: $('#translatedContainerId'),
        translatedToogle: $('#translationToogleId'),
        image: $('#imageContainerId'),
        firstBtn: $('#firstBtn'),
        firstLink: $('#firstBtn a'),
        prevBtn: $('#prevBtn'),
        prevLink: $('#prevBtn a'),
        nextBtn: $('#nextBtn'),
        nextLink: $('#nextBtn a'),
        lastBtn: $('#lastBtn'),
        lastLink: $('#lastBtn a'),
        pageNumber: $('input.pager-page-number'),
        slideShowBtn: $('#slide_show_btn')
    };

    var isPageNumberExist = containers.pageNumber.length > 0;
    if (isPageNumberExist) {
        var setPageNumber = $.proxy(function(index) {
            containers.pageNumber.val(index + 1);
        }, this);

        var onPageEnter = $.proxy(function() {
            var dirtyIndex = containers.pageNumber.val();
            var newIndex = parseInt(dirtyIndex);
            if (isNaN(newIndex) || !isFinite(dirtyIndex)) {
                setPageNumber(this.index);
                return;
            }
            var countElems = this.countElems();
            if (newIndex < 1 || newIndex > countElems) {
                setPageNumber(this.index);
                return;
            }
            this.index = newIndex - 1;
            this.showElemByIndex(this.index);
            this.setEnableBtns();
        }, this);
        containers.pageNumber.on('blur', onPageEnter).on('keypress', function(e) {
            var code = (e.keyCode ? e.keyCode : e.which);
            if (code == 13) {
                //Enter keycode
                onPageEnter();
            }
        });
    }

    var updateImageIfNeed = function (elem) {
        if (containers.image.length == 0 && options.getNewImage == null) {
            return;
        }
        var newImage = options.getNewImage(elem);
        if (newImage.NeedDisplay) {
            containers.image.attr('src', newImage.Url).css('display', 'inline');
        } else {
            containers.image.hide();
        }
    };

    this.sourceLanguageId = 0;
    this.translationLanguageId = 0;
   
    this.changeLanguageObserver = function(sourceLanguageId, translationLanguageId) {
        this.sourceLanguageId = sourceLanguageId;
        this.translationLanguageId = translationLanguageId;
        this.showElemByIndex(this.index);
    };

    this.hasNextElem = function() {
        var countElems = this.countElems();
        return this.index < countElems - 1;
    };

    this.hasTheSameElem = function (elem) {
        var isFound = false;
        $.each(this.elems, function(i, e) {
            isFound = e.Id == elem.Id;
            return !isFound;
        });
        return isFound;
    };

    this.MoveToLast = function () {
        return this.moveNextByIndex(this.countElems() - 1);
    };

    this.getCurrentElem = function() {
        return this.elems[this.index];
    };

    this.MoveToNext = function() {
        return this.moveNextByIndex(this.index + 1);
    };
    
    this.Shuffle = function () {
        this.elems = GlobalBusiness.shuffle(this.elems);
        this.showElemByIndex(this.index);
    },

    this.moveNextByIndex = function(newIndex) {
        if (!this.hasNextElem()) {
            return false;
        }
        if (options.onShowedElem) {
            var oldId = this.getIdByIndex(this.index);
            options.onShowedElem(oldId);
        }
        this.index = newIndex;
        this.showElemByIndex(this.index);
        this.loadNextPortion();
        this.setEnablePrevBtns(true);
        this.cleanExcessElemsIfNeed();

        if (options.onMoveAction) {
            options.onMoveAction(this.getCurrentElem());
        }
        return true;
    };

    this.hasPrevElem = function() {
        return this.index > 0;
    };

    this.hasNotLoadElems = function(idElem) {
        return idElem === 0;
    };

    this.loadPrevPortion = function () {
        this.setEnablePrevBtns(this.hasPrevElem());
        if (options.minCountToLoadPortion == null) {
            return;
        }
        if (this.index - options.minCountToLoadPortion > 0) {
            return;
        }
        var idElem = this.getIdByIndex(0);
        if (this.hasNotLoadElems(idElem)) {
            //не подгружать - пользователь зашел посмотреть на предложение, которого у него нет
            return;
        }
        //подгрузить предыдущие элементы
        this.loadPortionElems(options.actions.loadPrev, idElem, function (index, elem) {
            var isFound = this.hasTheSameElem(elem);
            if (isFound) {
                //текущий элемент уже был добавлен
                return false;
            }
            this.elems.splice(index, 0, elem);
            this.index++;
        });
    };

    this.loadNextPortion = function() {
        this.setEnableNextBtns(this.hasNextElem());
        if (options.minCountToLoadPortion == null) {
            return;
        }
        var countElems = this.countElems();
        if (this.index + options.minCountToLoadPortion < countElems) {
            return;
        }
        var idElem = this.getIdByIndex(countElems - 1);
        if (this.hasNotLoadElems(idElem)) {
            //не подгружать - пользователь зашел посмотреть на предложение, которого у него нет
            return;
        }
        //подгрузить следующие элементы
        this.loadPortionElems(options.actions.loadNext, idElem, function (index, elem) {
            var isFound = this.hasTheSameElem(elem);
            if (isFound) {
                //текущий элемент уже был добавлен
                return false;
            }
            this.elems.push(elem);
        });
    };

    this.cleanExcessElemsIfNeed = function () {
        if (options.minCountToLoadPortion == null) {
            return;
        }
        var maxPortion = options.minCountToLoadPortion * 3;
        while (this.index > maxPortion) {
            this.elems.splice(0, 1);
            this.index--;
        }
        while (this.countElems() - this.index > maxPortion) {
            this.elems.pop();
        }
    },

    this.setEnablePrevBtns = function (hasPrevElem) {
        this.setEnabledBtn(containers.prevBtn, hasPrevElem);
        if (containers.firstBtn) {
            this.setEnabledBtn(containers.firstBtn, hasPrevElem);
        }
    };
    
    this.setEnableNextBtns = function (hasNextElem) {
        this.setEnabledBtn(containers.nextBtn, hasNextElem);
        if (containers.lastBtn) {
            this.setEnabledBtn(containers.lastBtn, hasNextElem);
        }
    };

    this.setEnableBtns = function() {
        this.setEnablePrevBtns(this.hasPrevElem());
        this.setEnableNextBtns(this.hasNextElem());
    };

    this.setEnabledBtn = function (btn, isEnabled) {
        var disabledBtnClass = 'disabled';
        if (isEnabled) {
            btn.removeClass(disabledBtnClass);
        } else {
            btn.addClass(disabledBtnClass);
        }
    };

    this.loadPortionElems = function (actionName, id, eachLoadedElem) {
        $.getJSON(options.getUrl(actionName), { id: id }, $.proxy(function (response) {
            if (response == null || response.length == 0) {
                this.showMessageToUser();
                return;
            }
            $.each(response, $.proxy(eachLoadedElem, this));
            
            loadKnowledgeInfo(response, null);
            this.setEnableBtns();
        }, this));
    };

    this.getIdByIndex = function(index) {
        return this.elems[index].Id;
    };

    this.MoveToPrev = function() {
        return this.moveToPrevByIndex(this.index - 1);
    };
    
    this.MoveToFirst = function () {
        return this.moveToPrevByIndex(0);
    };

    this.moveToPrevByIndex = function (newIndex) {
        if (!this.hasPrevElem()) {
            return false;
        }
        this.index = newIndex;
        this.showElemByIndex(this.index);
        this.loadPrevPortion();
        this.setEnableNextBtns(containers.nextBtn, true);
        this.cleanExcessElemsIfNeed();
        
        if (options.onMoveAction) {
            options.onMoveAction(this.getCurrentElem());
        }
        return true;
    };

    this.isLanguagesEquals = function(elem, languageId) {
        return elem.LanguageId == languageId;
    };

    this.needChangeElems = function(elem) {
        if (this.sourceLanguageId <= 0 || this.translationLanguageId <= 0) {
            //языки не менялись после загрузки
            return false;
        }
        return this.isLanguagesEquals(elem.Source, this.translationLanguageId) && this.isLanguagesEquals(elem.Translation, this.sourceLanguageId);
    };

    this.prepareElemByLanguages = function (elem) {
        if (this.needChangeElems(elem)) {
            //пользователь сменил языки - поменять предложения местами, перед отображением
            var newTranslation = elem.Source;
            elem.Source = elem.Translation;
            elem.Translation = newTranslation;
        }
        return elem;
    };

    this.showElemByIndex = function (index) {
        var elem = this.prepareElemByLanguages(this.elems[index]);
        if (isPageNumberExist) {
            setPageNumber(index);
        }
        updateImageIfNeed(elem);
        var sourceText = options.getTextElem(elem.Source);
        var translationText = options.getTextElem(elem.Translation);
        this.changeTitleAndUrl(elem);
        this.changePrevNextLinks(index);
        containers.source.html(sourceText);
        containers.translated.html(translationText);
    };

    this.changeTitleAndUrl = function (elem) {
        if (options.onSelectElem != null) {
            options.onSelectElem(elem);
        }
        var title = options.getTitle(elem);
        $('title').html(title);
        var url = options.getUrlByElem(elem);
        window.history.replaceState(options.getHistoryObj(elem), title, url);
        SharePanel.Update(url, title);
    };

    this.getUrlByIndex = function (index) {
        var elem = this.prepareElemByLanguages(this.elems[index]);
        var url = options.getUrlByElem(elem);
        return url;
    };

    this.changePrevNextLinks = function(index) {
        var url = options.emptyLink;
        var firstUrl = options.emptyLink;
        if (this.hasPrevElem()) {
            url = this.getUrlByIndex(index - 1);
            firstUrl = this.getUrlByIndex(0);
        }
        containers.prevLink.attr('href', url);
        if (containers.firstLink) {
            containers.firstLink.attr('href', firstUrl);
        }

        url = options.emptyLink;
        var lastUrl = options.emptyLink;
        if (this.hasNextElem()) {
            url = this.getUrlByIndex(index + 1);
            lastUrl = this.getUrlByIndex(this.countElems() - 1);
        }
        containers.nextLink.attr('href', url);
        if (containers.lastLink) {
            containers.lastLink.attr('href', lastUrl);
        }
    };

    this.countElems = function() {
        return this.elems.length;
    };

    this.ToogleTranslation = function () {
        this.translationToogleBtn.Toogle();
        return false;
    };

    this.showMessageToUser = function() {
        //TODO: сообщить пользователю, что что-то пошло не так:(
    };

    var needCheckExistence = options.needCheckExistence;
    var loadKnowledgeInfo = function(elems, needSearchCurrentId) {
        if (needCheckExistence !== true) {
            return;
        }
        var ids = [];
        $.each(elems, function(index, elem) {
            ids.push(elem.Id);
        });
        UserKnowledge.Load(ids, needSearchCurrentId);
    };

    //подписаться на изменение языка
    LanguagePanel.AddObserver($.proxy(this.changeLanguageObserver, this));

    this.elems = options.elems || [];
    loadKnowledgeInfo(this.elems, true);
    this.index = 0;
    $.each(this.elems, $.proxy(function (index, elem) {
        if (elem.IsCurrent) {
            this.index = index;
            return false;
        }
        return true;
    }, this));

    if (containers.translatedToogle.length > 0) {
        this.translationToogleBtn = new ToggleTranslateBtn(containers);
    }

    this.showElemByIndex(this.index);
    this.loadPrevPortion();
    this.loadNextPortion();

    if (containers.slideShowBtn) {
        var slideOptions = {
            timeout: options.slideTimeout || 5000,
            container: containers.slideShowBtn,
            forwardAction: $.proxy(this.MoveToNext, this),
            backAction: $.proxy(this.MoveToPrev, this)
        };
        var slideShow = new function (opt) {
            this.toForward = true;

            this.action = function () {
                var action = this.toForward ? opt.forwardAction : opt.backAction;
                var hasElementsToShow = action();
                if (!hasElementsToShow) {
                    //меняем направление и продолжаем движение в обратном направлении
                    this.toForward = !this.toForward;
                }
            };

            var clickBtn = function (classToRemove, classToAdd, tooltip, action) {
                opt.container.removeClass(classToRemove).addClass(classToAdd).attr('title', tooltip).off('click').click(action);
            };

            this.play = function () {
                clickBtn('glyphicon-play', 'glyphicon-pause', 'Нажмите, чтобы остановить просмотр как слайд-шоу', $.proxy(this.pause, this));
                this.intervalId = setInterval($.proxy(this.action, this), opt.timeout);
            };
            this.pause = function () {
                if (this.intervalId) {
                    clearInterval(this.intervalId);
                    this.intervalId = null;
                }
                clickBtn('glyphicon-pause', 'glyphicon-play', 'Нажмите, чтобы запустить просмотр как слайд-шоу', $.proxy(this.play, this));
            };
            return this;
        }(slideOptions);
        slideShow.pause();
    }
    
    $(document).keyup($.proxy(function (event) {
        var keyCode = event.keyCode;
        if (keyCode == 37) {
            this.MoveToPrev();
            return;
        }
        if (keyCode == 39) {
            this.MoveToNext();
            return;
        }
    }, this));
};