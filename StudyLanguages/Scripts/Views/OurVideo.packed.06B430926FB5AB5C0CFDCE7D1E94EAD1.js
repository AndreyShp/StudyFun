﻿OurVideoController={Init:function(){var n={videoContainer:"#mainPlayer"};this.customVideo(n)},customVideo:function(n){var i=$(".mejs-layers"),t=$("<div id='ourVideoSubtitles' class='our-video-subtitles'>");i.prepend(t),$(n.videoContainer).mediaelementplayer({success:function(n){n.addEventListener("timeupdate",function(){t.html("Время "+n.currentTime+" сек."),$("#currentTime").html(n.currentTime)},!1),n.addEventListener("volumechange",function(){},!1),n.play()},error:function(){}})}},$(function(){OurVideoController.Init()});
