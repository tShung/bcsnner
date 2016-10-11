(function (undefined) {

    'use strict';

    $(document).ready(function () {
        // Grab elements, create settings, etc.
        var canvas = document.getElementById("canvas");
        if (canvas === null) return;
        var context = canvas.getContext("2d");
        var video = document.getElementById("video");

        var videoSelect = document.querySelector('select#videoSource');
        //var videoSelect = $('#videoSource');
        videoSelect.onchange = start;

        var handelVideo = function (stream) {
            window.stream = stream;
            var url = window.URL || window.webkitURL;
            video.src = url ? url.createObjectURL(stream) : stream;
            video.play();
        }
        var errBack = function (error) {
            var msg = "Video capture error: ";
            console.log(msg, error.code);
            alert(msg);
        };

        //crop starting pos and size needs to be double
        var crop = $("#cropArea")[0];
        var x = (crop.clientLeft + crop.offsetLeft); //104
        var y = (crop.clientTop + crop.offsetTop); //204
        var cwidth = crop.offsetWidth;
        var cheight = crop.offsetHeight;

        var vwidth = 0;
        var vheight = 0;

        //add event listener to set canvas
        video.addEventListener('canplay', function (e) {
            if (video.videoWidth > 0) {
                vheight = video.videoHeight;
                vwidth = video.videoWidth;
                canvas.setAttribute('width', cwidth);
                canvas.setAttribute('height', cheight);
            }
        }, false);

        //check browser support
        navigator.getUserMedia = navigator.getUserMedia || //Standard
                          navigator.webkitGetUserMedia ||  //WebKit-prefixed
                          navigator.mozGetUserMedia ||     //Firefox-prefixed
                          navigator.msGetUserMedia;        //

        //callback for enumerateDevice()
        function gotDevices(sourceInfos) {
            var len = sourceInfos.length;
            for (var i = len - 1; i >= 0 ; i--) {
                var sourceInfo = sourceInfos[i];
                var option = document.createElement('option');
                option.value = sourceInfo.deviceId;
                if (i === len - 1)
                    option.selected = true;
                if (sourceInfo.kind === 'videoinput') {
                    option.text = sourceInfo.label || 'camera ' + (videoSelect.length + 1);
                    videoSelect.appendChild(option);
                } else {
                    console.log('Some other kind of source: ', sourceInfo);
                }
            }
            start();
        };

        //mediaStreamTrack.getsource() callback
        function gotSources(sourceInfos) {
            var len = sourceInfos.length;
            for (var i = len - 1; i >= 0; i--) {
                var sourceInfo = sourceInfos[i];
                var option = document.createElement('option');
                option.value = sourceInfo.id;
                if (i == len - 1)
                    option.selected = true;
                if (sourceInfo.kind === 'video') {
                    option.text = sourceInfo.label || 'camera ' + (videoSelect.length + 1);
                    videoSelect.appendChild(option);
                } else {
                    console.log('Some other kind of source: ', sourceInfo);
                }
            }
            start();
        };

        function useMediaStreamTrack() {
            if (typeof MediaStreamTrack === 'undefined' ||
                typeof MediaStreamTrack.getSources === 'undefined') {
                alert('This browser does not support MediaStreamTrack.\n\nTry Chrome.');
                start();
            } else {
                MediaStreamTrack.getSources(gotSources);
            }
        };

        if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
            console.log("enumerateDevices() not supported.");
            //alert('this browser does not support enumerateDevices');
            useMediaStreamTrack();
        }
        else {
            navigator.mediaDevices.enumerateDevices()
            .then(gotDevices);
        };

        function start() {
            if (window.stream) {
                video.src = null;
                window.stream.stop();
            };
            //$('#camera').text(videoSelect.value);
            if (navigator.getUserMedia) {
                var videoSource = videoSelect.value;
                var videoObj = {
                    video: {
                        optional: [{ sourceId: videoSource }]
                        //, deviceId: videoSource ? { exact: videoSource } : undefined
                    },
                    audio: false
                };
                navigator.getUserMedia(videoObj, handelVideo, errBack);
            }
            else {
                var msg = "getUserMedia() not supported in your browser";
                console.log(msg);
                alert(msg);
            }
        };
        
        //start();

        //take a phote and submit to server
        $("#snap").click(takePhone);
        $("#scann").click(takePhone);

        function takePhone() {
            context.drawImage(video, 0, 0, vwidth, vheight, 0, 0, cwidth, cheight);
            //context.drawImage(video, 0, 0, width, height);
            var image = canvas.toDataURL("image/png");
            image = image.replace('data:image/png;base64,', '');
            $('#scannedImage').val(image);
            $('#scannForm').submit();
        }

        //var front = false;
        //document.getElementById('flip-button').onclick = function () {
        //    front = !front;
        //    if (navigator.getUserMedia) {
        //        var videoObj = {
        //            video: {
        //                facingMode: (front ? "user" : "environment")
        //            },
        //            audio: false
        //        };
        //        navigator.getUserMedia(videoObj, handelVideo, errBack);
        //    }
        //};

    });
})();