/**
 * Created with JetBrains WebStorm.
 * User: valerio
 * Date: 29/01/13
 * Time: 23.03
 * To change this template use File | Settings | File Templates.
 */

/*$.Deferred = (function($) {
var oldDeferred = $.Deferred;

return function Deferred(fn) {
var d = oldDeferred(fn);
var oldThen = d.then;
d.then = d.promise().then = function(done, fail, progress) {
return oldThen.call(d, wrap(done), wrap(fail), wrap(progress));
};
return d;
};

function wrap(fn) {
return fn && function() {
try {
var val = fn.apply(this, arguments);
return (val && val.promise) ? val :
$.Deferred(function(d) {
d.resolve(val);
}).promise();
} catch (err) {
return $.Deferred(function(d) {
d.reject(err);
}).promise();
}
}
}
})(jQuery);*/

var jePromise = (function($) {
    var jePromise = {};
    jePromise.handleFile = function(file) {
        var deferred = $.Deferred(function(d) {
            var reader = new FileReader();
            // init the reader event handlers
            //reader.onprogress = handleReaderProgress; Commented out as we don't really need it
            reader.onloadend = function(evt) {
                d.resolve(evt.target.result);
            }
            // begin the read operation
            reader.readAsDataURL(file);
        });
        return deferred.promise();
    }

    // Uploads a file to the server
    jePromise.UploadFile = function(file) {
        var deferred = $.Deferred(function(d) {
            var xhr = new XMLHttpRequest();
            xhr.upload.addEventListener("progress", function (evt) {
                if (evt.lengthComputable) {
                    var percentageUploaded = parseInt(100 - (evt.loaded / evt.total * 100));
                    d.notify(percentageUploaded)
                }
            }, false);

            // File uploaded
            xhr.addEventListener("load", function () {
                d.notify(100);
            }, false);

            // file received/failed
            xhr.onreadystatechange = function (e) {
                if (xhr.readyState == 4) {
                    if (xhr.status == 200) {
                        var link = "<a href=\"" + xhr.responseText + "\" target=\"_blank\">" + fileName + "</a>";
                        d.resolve(link);
                    }
                    else {
                        d.reject("Error, resource not found.")
                    }
                }
            };

            xhr.open("POST", "/Home/Upload", true);

            // Set appropriate headers
            xhr.setRequestHeader("Content-Type", "multipart/form-data");
            xhr.setRequestHeader("X-File-Name", file.name);
            xhr.setRequestHeader("X-File-Size", file.size);
            xhr.setRequestHeader("X-File-Type", file.type);

            // Send the file
            xhr.send(file);
        });
        return deferred.promise();
    }

    return jePromise;
})(jQuery);