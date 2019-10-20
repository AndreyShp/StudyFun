ProfileController = {
    Init: function () {
        var CHANGE_UNIQUE_ID_BLOCK = '#changeUniqueIdBlock';

        var showMessage = function(success, text) {
            var options = { isSuccess: success, removeLabels: true, container: $('#profileMessageContainer'), text: text };
            var msgInner = new Message(options);
            msgInner.Show();
        };
        
        MailBlock({
            url: ServerData.Patterns.Urls.SendToMail,
            mailBlockSelector: '#mailBlock',
            emailFieldSelector: '#emailField',
            showBtnBlockSelector: '#showMailBlockBtn',
            sendBtnSelector: '#sendUniqueIdBtn',
            showMessage: showMessage,
            processedPost: function (success) {
                if (success) {
                    GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.ProfileSendUnique, { email: email });
                }
            }
        });

        CopyToClipboard({
            copyToClipboardBtnSelector: '#copyToClipboardBtn',
            textSelector: '#currentUniqueIdContainer'
        });

        var hideChangeUnique = function() {
            $(CHANGE_UNIQUE_ID_BLOCK).hide();
        };

        var newUniqueField = $('#newUniqueId');
        Global.focusClearField(newUniqueField);
        
        $('#showChangeUniqueBlockBtn').click(function () {
            var block = $(CHANGE_UNIQUE_ID_BLOCK);
            block.toggle();
            newUniqueField.focus();
        });

        $('#setNewUniqueIdBtn').click(function () {
            var currentUniqueIdContainer = $('#currentUniqueIdContainer');
            var newUniqueId = newUniqueField.val();
            if (newUniqueId == '' || currentUniqueIdContainer.text() == newUniqueId) {
                showMessage(false, 'Некорректные данные!<br />Введите идентификатор с другого устройства');
                Global.setInputStyle(newUniqueField, true);
                return;
            }

            var url = ServerData.Patterns.Urls.SetNewValue;
            $.post(url, { newValue: newUniqueId }, $.proxy(function (data) {
                var success = data != null && data.success;
                var text;
                if (success) {
                    text = 'Ваш идентификатор успешно изменен';
                    hideChangeUnique();
                    newUniqueField.val('');
                    currentUniqueIdContainer.text(newUniqueId);
                    //обновить панель знаний в меню
                    UserKnowledge.ShortInfo.Display();
                    GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.ProfileChangeUnique, { newUniqueId: newUniqueId });
                } else {
                    text = 'Извините, не удалось изменить идентификатор. Проверьте правильность введенного идентификатора.<br />' +
                        'Попробуйте еще раз через несколько секунд';
                }
                showMessage(success, text);
            }, this), "json");
        });
    }
};

$(function () {
    ProfileController.Init();
});