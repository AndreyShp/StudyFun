AllMaterialsSalesController = {
    Init: function () {
        var EMAIL_SELECTOR = "#emailField";

        var topPadding = 0;
        if (GlobalBusiness != null && GlobalBusiness.TopPanel != null) {
            topPadding = GlobalBusiness.TopPanel.GetHeight();
        }

        var selectedBlock = $("#selectedBlock");
        selectedBlock.sticky({ topSpacing: topPadding, center: true });

        var model = {
            consent: ko.observable(false),
        };

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
            var dataToPost = '{ uniqueDownloadId:"' + ServerData.UniqueDownloadId + '"}';
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
    }
};

$(function() {
    AllMaterialsSalesController.Init();
});