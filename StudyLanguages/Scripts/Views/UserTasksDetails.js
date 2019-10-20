UserTasksDetail = {
    Init: function () {
        var MESSAGE_CONTAINER_ID = '#messageContainer';
        var list = $('#commentsList');
        var lastShowedComment = list.children('li').length;

        var commentField = $('#commentInput');
        Global.focusClearField(commentField);
        GlobalBusiness.maxLengthAttention( { field: commentField, inputTitle: 'комментария' });

        if ($('#allRightsPanel').length > 0) {
            var deleteBtn = $('#deleteUserTaskBtn');
            var restoreBtn = $('#restoreUserTaskBtn');

            var restoreDelete = function (needRemove) {
                var taskKey = ServerData.Patterns.TaskKey;
                var url = ServerData.Patterns.Urls.RemoveOrRestore;
                $.post(url, { key: taskKey, needRemove: needRemove }, $.proxy(function(data) {
                    var success = data != null && data.success;
                    if (!success) {
                        Global.showMessage(MESSAGE_CONTAINER_ID, false,
                            'Извините, не удалось ' + (needRemove ? 'удалить' : 'восстановить') + ' задание.<br />' +
                                'Попробуйте еще раз через несколько секунд');
                        return;
                    }

                    if (needRemove) {
                        deleteBtn.hide();
                        restoreBtn.show();
                    } else {
                        restoreBtn.hide();
                        deleteBtn.show();
                    }
                }, this), "json");
            };

            deleteBtn.click(function () {
                restoreDelete(true);
            });
            
            restoreBtn.click(function () {
                restoreDelete(false);
            });
        }
        
        $('#addCommentBtn').click(function () {
            var comment = commentField.val().trim();
            if (comment == '') {
                Global.showMessage(MESSAGE_CONTAINER_ID, false, 'Некорректные данные!<br />Введите комментарий');
                Global.setInputStyle(commentField, true);
                return;
            }

            comment = Global.escapeHTML(comment);
            var authorId = ServerData.Patterns.AuthorId;
            var taskKey = ServerData.Patterns.TaskKey;
            var url = ServerData.Patterns.Urls.AddComment;
            $.post(url, { authorId: authorId, key: taskKey, comment: comment, lastShowedComment: lastShowedComment }, $.proxy(function (data) {
                var success = data != null && data.success;
                if (!success) {
                    Global.showMessage(MESSAGE_CONTAINER_ID, false,
                            'Извините, не удалось добавить комментарий.<br />' +
                            'Попробуйте еще раз через несколько секунд');
                    return;
                }
                
                commentField.val('');
                if (data.countNewComments != null && data.countNewComments > 0) {
                    lastShowedComment += data.countNewComments;
                    //удалить сообщение о том, что нет комментариев
                    $('#noCommentsMessage').remove();
                    //TODO: не отображать комментарии, а отобразить кнопку с тем, что есть новые комментарии
                    list.prepend(data.newComments);
                }
                GlobalBusiness.Counter.reachGoal(GlobalBusiness.Counter.Ids.UserTaskNewComment);
            }, this), "json");
        });
    }
};

$(function () {
    UserTasksDetail.Init();
});