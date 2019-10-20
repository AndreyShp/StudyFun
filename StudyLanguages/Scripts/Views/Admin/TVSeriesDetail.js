TVSeriesDetailAdmin = {
    removedIndexes: [],
    
    UpdateSubtitle: function (btn) {
        var index = $(btn).parent().index();
        var subtitle = ServerData.Subtitles[index];
        alert(subtitle.Text);
    },
    
    RemoveSubtitle: function (btn) {
        var index = $(btn).parent().index();
        var subtitle = ServerData.Subtitles[index];
        alert(subtitle.Text);
    }
};