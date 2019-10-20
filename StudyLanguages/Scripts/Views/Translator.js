TranslatorController = {
    containers: {
        searchField: $('#search'),
        translationContainer: $('#translationContainer')
    },
    lastSearch: null,
    lastTranslate: null,
    skipSearch: null,
        
    getUrl: function (url) {
        return ServerData.GetPath(url);
    },

    loadQuery: function(query, processCallback) {
        this.lastSearch = query;
        if (this.skipSearch === true) {
            this.skipSearch = null;
            return;
        }

        $.getJSON(this.getUrl(ServerData.Patterns.Urls.SearchUrl), { query: query }, $.proxy(function (response) {
            if (response == null) {
                return;
            }
            if (this.lastSearch == query) {
                if (response.IsChangedLanguage) {
                    //сменить языки на панели
                    LanguagePanel.OnChangeLanguage();
                }
                if (response.NewPattern) {
                    this.lastSearch = response.NewPattern;
                    this.containers.searchField.val(this.lastSearch);
                    this.skipSearch = true;
                    this.containers.searchField.keyup();
                }
                if (response.Words != null) {
                    processCallback(response.Words);
                }
            }
        }, this));
    },

    getText: function(item) {
        return Speaker.GetWordHtml(item);
    },

    select: function (selectedItem) {
        this.showTranslationByQuery(selectedItem);
        return selectedItem;
    },

    showTranslationByQuery: function (query) {
        if (!query) {
            return false;
        }
        this.lastTranslate = query;
        $.getJSON(this.getUrl(ServerData.Patterns.Urls.GetTranslations), { query: query }, $.proxy(function (response) {
            var translationHtml;
            if (this.lastTranslate != query) {
                //пользователь уже переводит что-то другое
                return;
            }
            var title;
            var url;
            if (response != null && response.length > 0) {
                translationHtml = this.getTranslationsAsHtml(response);
                title = String.format(ServerData.Patterns.Title, query, response[0].Text);
                var languages = LanguagePanel.GetLanguages();
                url = String.format(ServerData.Patterns.Urls.SpecialUrl, languages.from.ShortName, languages.to.ShortName, query);
            } else {
                translationHtml = 'Извините, перевод для "' + query + '" не найден';
                title = ServerData.Patterns.TitleWithoutQuery;
                url = ServerData.Patterns.Urls.UrlWithoutQuery;
            }
            url = this.getUrl(url);
            this.containers.translationContainer.html(translationHtml);
            this.changeTitleAndUrl(title, url, query);
            this.containers.searchField.select();
        }, this));
        return true;
    },
    
    changeTitleAndUrl: function (title, url, query) {
        $('title').html(title);
        window.history.replaceState({ query: query }, title, url);
        SharePanel.Update(url, title);
    },

    getTranslationsAsHtml: function(response) {
        var result = $('<ol>');
        $.each(response, $.proxy(function(index, translation) {
            var translationText = this.getText(translation);
            var translationItem = $('<li>').html(translationText);
            result.append(translationItem);
            return true;
        }, this));
        return result;
    },

    searchKeyUp: function (event) {
        if (event.which == 13) {
            this.Translate(false);
        }
        this.clearIfFieldEmpty();
    },
    
    clearIfFieldEmpty: function () {
        var searchField = this.containers.searchField.val();
        if (!searchField) {
            this.containers.translationContainer.html('');
            this.changeTitleAndUrl(ServerData.Patterns.TitleWithoutQuery, ServerData.Patterns.Urls.UrlWithoutQuery);
        }
        this.changeErrorSearch();
    },

    Init: function() {
        var options = { source: $.proxy(this.loadQuery, this), updater: $.proxy(this.select, this) };
        this.containers.searchField
            .on('keyup', $.proxy(this.searchKeyUp, this))
            .on('input paste', $.proxy(this.clearIfFieldEmpty(), this))
            .on('click', $.proxy(this.changeErrorSearch, this))
            .typeahead(options).focus();
    },
    
    Translate: function (showMessage) {
        var query = this.containers.searchField.val();
        var isSend = this.showTranslationByQuery(query);
        if (!isSend && showMessage === true) {
            this.changeErrorSearch(true);
        }
    },
    
    changeErrorSearch: function(isShowError) {
        Global.setInputStyle(this.containers.searchField, isShowError);
    }
};

$(function() {
    TranslatorController.Init();
});