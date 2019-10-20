UserTasksNew = {
    Init: function () {
        var MESSAGE_CONTAINER_ID = '#messageContainer';

        var taskField = $('#taskInput');
        Global.focusClearField(taskField);
        GlobalBusiness.maxLengthAttention({ field: taskField, inputTitle: 'задания' });

        $('#addNewTaskBtn').click(function () {
            var task = taskField.val().trim();
            if (task == '') {
                Global.showMessage(MESSAGE_CONTAINER_ID, false, 'Некорректные данные!<br />Введите задание для других пользователей');
                Global.setInputStyle(taskField, true);
                return;
            }
            
            task = Global.escapeHTML(task);
            var addTaskUrl = ServerData.Patterns.Urls.AddTask;
            $.post(addTaskUrl, { task: task }, $.proxy(function (data) {
                var success = data != null && data.success;
                var text;
                if (success) {
                    text = 'Ваше задание успешно добавлено';
                    taskField.val('');
                    window.location = data.urlToRedirect;
                    
                    GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.UserTaskNew);
                } else {
                    text = 'Извините, не удалось добавить задание.<br />' +
                        'Попробуйте еще раз через несколько секунд';
                }

                Global.showMessage(MESSAGE_CONTAINER_ID, success, text);
            }, this), "json");
        });
    }
};

$(function () {
    UserTasksNew.Init();
});