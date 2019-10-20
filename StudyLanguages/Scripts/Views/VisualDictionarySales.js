VisualDictionarySalesController = {
    TR_SELECTOR: "tr[data-id]",
    
    getIdByElem: function(elem) {
        var tr = elem.parents(VisualDictionarySalesController.TR_SELECTOR);
        var id = tr.data("id");
        return id;
    },

    Init: function () {
        var EMAIL_SELECTOR = "#emailField";

        var topPadding = 0;
        if (GlobalBusiness != null && GlobalBusiness.TopPanel != null) {
            topPadding = GlobalBusiness.TopPanel.GetHeight();
        }

        var selectedBlock = $("#selectedBlock");
        selectedBlock.sticky({ topSpacing: topPadding, center: true });

        var toMoneyFormat = function(value) {
            var summ = Math.round(value * 100);
            var result = parseInt(summ / 100);
            var kopeks = summ % 100;
            var strKopeks = kopeks;
            if (kopeks < 10) {
                strKopeks = "0" + strKopeks;
            }
            result += "," + strKopeks;
            return result;
        };

        var model = {
            consent: ko.observable(false),
            summPrice: ko.observable(),
            count: ko.observable(),
            hasCount: ko.observable()
        };

        var setModel = function(count, summPrice) {
            model.count(count);
            model.summPrice(toMoneyFormat(summPrice));
            model.hasCount(count > 0);
        };

        setModel(ServerData.SelectedCount, ServerData.SelectedSummPrice);

        var checkBoxes = $(VisualDictionarySalesController.TR_SELECTOR).find("input[type='checkbox']");
        var selectedAllCbx = $("#allCheckboxes");
        selectedAllCbx.click(function () {
            var isSelected = $(this).is(":checked");
            if (isSelected) {
                //NOTE: attr почему-то не работал!!!
                checkBoxes.prop("checked", true);
                setModel(ServerData.AllCount, ServerData.SummPrice);
            } else {
                checkBoxes.removeAttr("checked");
                setModel(0, 0);
            }
        });

        checkBoxes.click(function () {
            var count = 0;
            var isAllChecked = true;
            var summPrice = 0;
            $.each(checkBoxes, function (i, cb) {
                var checkbox = $(cb);
                if (!checkbox.is(":checked")) {
                    isAllChecked = false;
                    return true;
                }

                var id = VisualDictionarySalesController.getIdByElem(checkbox);
                var foundElem = GlobalBusiness.findById(id);
                if (foundElem != null) {
                    summPrice = Math.round(summPrice + foundElem.Price * 100);
                }

                count++;
                return true;
            });
            if (isAllChecked) {
                selectedAllCbx.prop("checked", true);
            } else {
                selectedAllCbx.removeAttr("checked");
            }

            setModel(count, isAllChecked ? ServerData.SummPrice : summPrice / 100);
        });

        function showMessage(success, text) {
            var options = { isSuccess: success, removeLabels: true, container: $('#salesMessage'), text: text };
            var msgInner = new Message(options);
            msgInner.Show();
        }

        MailBlock({
            url: ServerData.Urls.SendToMail,
            mailBlockSelector: '#mailBlock',
            emailFieldSelector: EMAIL_SELECTOR,
            showBtnBlockSelector: '#showMailBlockBtn',
            sendBtnSelector: '#sendUniqueIdBtn',
            showMessage: showMessage,
            dataToPostModifier: function (dataToPost) {
                dataToPost.uniqueDownloadId = ServerData.UniqueDownloadId;
            }
        });

        CopyToClipboard({
            copyToClipboardBtnSelector: '#copyToClipboardBtn',
            textSelector: '#salesUniqueId'
        });

        $("#buySelectedBtn").click(function () {
            $("#consentCbx").prop("checked", false);
            $(EMAIL_SELECTOR).val("");
            model.consent(false);
            
            $('#salesFinishStepWindow').modal('show');
            GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.BuyClick);
        });

        $('#payBtn').click(function () {
            var selectedIds = [];
            $.each(checkBoxes, function (i, cb) {
                var checkbox = $(cb);
                if (!checkbox.is(":checked")) {
                    return true;
                }

                var id = VisualDictionarySalesController.getIdByElem(checkbox);
                selectedIds.push(id);
                return true;
            });
            
            var dataToPost = '{ ids:' + JSON.stringify(selectedIds) + ', uniqueDownloadId:"' + ServerData.UniqueDownloadId + '"}';
            $.ajax({
                url: ServerData.Urls.Buy,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: dataToPost,
                success: function (data) {
                    var success = data != null && data.success;
                    if (success) {
                        location.href = data.paymentUrl;
                    } else {
                        var message = data != null ? data.message : "Извините, не удалось перейти к оплате.<br />" +
                            "Попробуйте еще раз через несколько секунд";
                        showMessage(false, message);
                    }
                }
            });
            
            GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.PayClick);
        });

        // Activates knockout.js
        ko.applyBindings(model);
        
        var elemScrollTo = $('#sales-scroll-to-elem');
        if (elemScrollTo.length > 0) {
            var MARGIN = 10;   //отступ
            var screenHeight = $(window).height();
            var scrollElemTop = elemScrollTo.offset().top;
            var scrollElemHeight = elemScrollTo.outerHeight();
            //topPadding - меню, selectedBlock - панель с ценой
            var positionToScroll = scrollElemTop - topPadding - selectedBlock.outerHeight() - MARGIN;
            if ((scrollElemTop + scrollElemHeight) >= screenHeight) {
                //элемент не видно на экране или видно, но не полностью - прокрутить до элемента
                $('html, body').scrollTop(positionToScroll);
            }
        }
    },
    
    ShowPreview: function (clickedElement) {
        var WORDS_CONTAINER = "wordsContainer";
        var WORDS_CONTAINER_SELECTOR = "#" + WORDS_CONTAINER;
        
        var id = VisualDictionarySalesController.getIdByElem($(clickedElement));
        var elem = GlobalBusiness.findById(id);

        var encodedName = encodeURIComponent(elem.Name);
        var imageUrl = String.format(ServerData.Urls.Image, encodedName);
        var body = "<div class='sales-preview-container'>"
            + "<div class='pull-left'>"
                + "<img src='" + imageUrl + "' alt='Визуальный словарь на тему &laquo;" + elem.Name + "&raquo;'></div>"
            + "<div class='pull-left sales-preview-words' id='" + WORDS_CONTAINER + "'></div>"
            + "</div>";
        
        var prettyWindow = new PrettyWindow({ header: "Предпросмотр визуального словаря &laquo;" + elem.Name + "&raquo;", body: body });

        $(WORDS_CONTAINER_SELECTOR).loading("Загрузка слов");
        var previewInfoUrl = String.format(ServerData.Urls.PreviewInfo, encodedName);
        $.getJSON(previewInfoUrl, function (areas) {
            $(WORDS_CONTAINER_SELECTOR).loading("hide");
            if (areas == null) {
                Global.showMessage(WORDS_CONTAINER_SELECTOR, false,
                            'Извините, не удалось получить слова.<br />' +
                            'Закройте окно предпросмотра и попробуйте еще раз через несколько секунд');
                return;
            }

            var areasToHtml = function(words) {
                var result = "<ul>";
                $.each(words, function (i, word) {
                    result += "<li>" + word.Source.Text + "</li>";
                    return true;
                });
                result += "</ul>";
                return result;
            };

            var middleIndex = parseInt(areas.length / 2);
            if (areas.length % 2 > 0) {
                middleIndex++;
            }
            
            var leftWords = areasToHtml(areas.slice(0, middleIndex));
            var rightWords = areasToHtml(areas.slice(middleIndex, areas.length));
            var wordsHtml = "<div class='pull-left'>" + leftWords + "</div><div class='pull-left'>" + rightWords + "</div>";
            
            $(WORDS_CONTAINER_SELECTOR).html(wordsHtml);
        });

        prettyWindow.Show();
    },
};

$(function() {
    VisualDictionarySalesController.Init();
});