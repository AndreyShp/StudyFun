TVSeriesDetailController = {
    Init: function () {
        var options = {
            videoContainer: "#mainPlayer",
            subtitlesContainer: $("#subtitles"),
            currentSubtitleClass: "tv-series-current-subtitle",
            subtitles: ServerData.Subtitles
        };
        this.customVideo(options);

        var anotherVideos = $("#anotherVideos");
        $.getJSON(ServerData.Urls.AnotherSeries, null, function (response) {
            if (response == null || !response.length) {
                return;
            }

            for (var i = 0; i < response.length; i++) {
                var item = response[i];
                anotherVideos.append(String.format('<a href="{0}">Серия {1}</a>', item.Url, item.Episode));
                if (i + 1 < response.length) {
                    anotherVideos.append(" | ");
                }
            }
            anotherVideos.show();
        });
    },
    customVideo: function (options) {
        var VOLUME_COOKIE_NAME = "OurVideoVolume";

        var subtitlesContainer = options.subtitlesContainer;
        var subtitles = options.subtitles;
        var index = 0;
        var showedIndex = -1;
        var maxSubtitleIndex = subtitles.length - 1;

        var subtitlesElems = subtitlesContainer.find("li");
        subtitlesContainer.scrollTop(0);
        var scrollDeviation = $(subtitlesElems[0]).offset().top;

        var defaultVolume = Global.getCookie(VOLUME_COOKIE_NAME);
        if (defaultVolume == null || isNaN(defaultVolume) || !isFinite(defaultVolume)) {
            defaultVolume = 0.3;
        }

        $(options.videoContainer).mediaelementplayer({
            startVolume: defaultVolume,
            success: function (mediaElement, domObject) {
                var containerToOwnFeatures = $(".mejs-layers");
                var ourSubtitles = $("<div id='ourVideoSubtitles' class='tv-series-video-subtitles'>");
                containerToOwnFeatures.prepend(ourSubtitles);

                var MAX_SKIP_SECONDS = 3;
                var COUNT_SUBTITLES_BEFORE_CURRENT = 3;
                var curTime = 0;
                mediaElement.addEventListener('timeupdate', function (e) {
                    var timeSkip = Math.abs(mediaElement.currentTime - curTime);
                    if (timeSkip > MAX_SKIP_SECONDS) {
                        //прыжок во времени больше чем на MAX_SKIP_SECONDS сек - спрятать показанный субтитр
                        ourSubtitles.hide().html("");
                        subtitlesElems.removeClass(options.currentSubtitleClass);
                    }

                    curTime = mediaElement.currentTime;

                    var subtitle = subtitles[index];
                    /*subtitle.Text
                    subtitle.TranslationText
                    subtitle.TimeFrom*/
                    var timeFrom = subtitle.TimeFrom;
                    var timeTo = subtitle.TimeTo;
                    if (timeFrom <= curTime && timeTo >= curTime && index != showedIndex) {
                        showedIndex = index;
                        //console.log("!!! " + index + " " + timeFrom + "..." + timeTo + " - " + curTime);

                        var subtitleText = subtitle.Text.replace("\r\n", "<br />");
                        ourSubtitles.show().html(subtitleText);
                        subtitlesElems.removeClass(options.currentSubtitleClass);
                        var currentSubtitle = subtitlesElems.eq(showedIndex);
                        currentSubtitle.addClass(options.currentSubtitleClass);

                        //прокрутить на нужный субтитр - чтобы он был третьим 
                        if (showedIndex >= COUNT_SUBTITLES_BEFORE_CURRENT) {
                            var scrollTop = subtitlesContainer.scrollTop() + subtitlesElems.eq(showedIndex - COUNT_SUBTITLES_BEFORE_CURRENT).offset().top - scrollDeviation;
                            subtitlesContainer.scrollTop(scrollTop);
                        }
                        return;
                    }
                    
                    do {
                        if (curTime > timeTo && index < maxSubtitleIndex && subtitles[index + 1].TimeFrom <= curTime) {
                            //console.log("inc " + index + " " + timeFrom + "..." + timeTo + " - " + curTime);
                            index++;
                            showedIndex = 0;
                        } else if (curTime < timeFrom && index > 0 && subtitles[index - 1].TimeTo >= curTime) {
                            //console.log("dec " + index + " " + timeFrom + "..." + timeTo + " - " + curTime);
                            index--;
                            showedIndex = 0;
                        } else {
                            break;
                        }
                    } while (index > 0 && index <= maxSubtitleIndex)
                }, false);
                mediaElement.addEventListener('volumechange', function (e) {
                    //изменение громкости сохраняем в куке
                    Global.setCookie(VOLUME_COOKIE_NAME, mediaElement.volume);
                }, false);
               
                mediaElement.play();
            },
            error: function () {
                //TODO: выводить сообщение
            }
        });
    }
};

$(function() {
    TVSeriesDetailController.Init();
});