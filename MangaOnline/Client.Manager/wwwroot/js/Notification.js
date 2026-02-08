function ShowNotificationSuccess(content) {
    $("#contentAlertNotificationSuccess").text(content);
    $("#alertNotificationSuccess").show();
    setTimeout(function () {
        $("#alertNotificationSuccess").hide();
    }, 3000);
}

function ShowNotificationError(content) {
    $("#contentAlertNotificationError").text(content);
    $("#alertNotificationError").show();
    setTimeout(function () {
        $("#alertNotificationError").hide();
    }, 3000);
}

function CloseNotificationSuccess() {
    $("#alertNotificationSuccess").hide();
}
function CloseNotificationError() {
    $("#alertNotificationError").hide();
}