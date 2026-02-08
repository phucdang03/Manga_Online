const urlClient = "http://localhost:5030";
const urlServiceManga = "http://localhost:5098";

$(document).ready(function () {
    if(GetUserData()!=null){
        const user = GetUserData();
        const role = user.role;
        console.log(role);
        $('#btn-user').show();
        switch (role) {
            case 'Admin':
                // $('#user-manager').show();
                $('#manga-manager').show();
                $('#admin-icon').show();
                $('#add-manga-manager').show();
                break;
            case 'UserNormal':
                $('#pay-history').show();
                $('#user-levelUp').show();
                break;
            case 'UserVip':
                $('#userVIp').show();
                $('#pay-history').show();
                break;
        }
        if(user.avartar!==null){
            $('#avartar-user').attr('src', `/image/avartar-user/${user.avartar}`);
        }
    }else {
        $('#btn-login').show();
    }
    updateListHistoryMangaId();
    loadNotiHistory();
    updateListFollowMangaId();
    loadNotiFollow();
})

function GetUserData() {
    const user = localStorage.getItem("USER_DATA");
    return $.parseJSON(user);
    // {"userId":"ef20386b-2335-4f65-87b2-819e1aef6612","userName":"traicv","email":"enm55606@nezid.com","role":"UserNormal","avartar":null}
}
function LogOut() {
    localStorage.setItem('USER_DATA', null);
    localStorage.setItem('ACCESS_TOKEN', null);
    localStorage.setItem('MANGA_FOLLOW', '');
    window.location.href = 'http://localhost:5030/Auth/AuthLogin';
}
function formatView(view) {
    if (view >= 1000000000) {
        return (num / 1000000000).toFixed(1).replace(/\.0$/, '') + 'B';
    }
    if (view >= 1000000) {
        return (view / 1000000).toFixed(1).replace(/\.0$/, '') + 'M';
    }
    if (view >= 1000) {
        return (view / 1000).toFixed(1).replace(/\.0$/, '') + 'K';
    }
    return view;
}

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

// Hàm để lấy giá trị của tham số từ URL
function GetURLParameter(name) {
    var url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)");
    var results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function arrayToString(arr) {
    return arr.join(",");
}

function stringToArray(str) {
    return str.split(","); 
}

function updateListHistoryMangaId() {
    const user = GetUserData();
    if (user !== null) {
        $.ajax({
            url: 'http://localhost:5098/manga/ReadingHistoryId?userId=' + user.userId,
            type: "GET",
            success: function (response) {
                localStorage.setItem("MANGA_HISTORY", response);
            }
        });
    }
}
function updateListFollowMangaId() {
    const user = GetUserData();
    if (user!==null){
        $.ajax({
            url: 'http://localhost:5098/manga/FollowMangaId?userId='+user.userId,
            type: "GET",
            success: function (response) {
                localStorage.setItem("MANGA_FOLLOW",response);
            }
        });
    }
}
function loadNotiHistory() {
    if (localStorage.getItem("LIST_ID_HISTORY") !== '' && localStorage.getItem("LIST_ID_HISTORY") !== null) {
        let listHistory = stringToArray(localStorage.getItem("LIST_ID_HISTORY"));
        $('#noti-history').show();
        $('#noti-number-history').text(listHistory.length);
    } else {
        $('#noti-history').hide();
    }
}
function resetNotiHistory() {
    localStorage.setItem("LIST_ID_HISTORY", '');
    loadNotiHistory();
}
function loadNotiFollow(){
    if (localStorage.getItem("LIST_ID_FOLLOW") !== '' &&localStorage.getItem("LIST_ID_FOLLOW") !== null ) {
        let listFollow = stringToArray(localStorage.getItem("LIST_ID_FOLLOW"));
        $('#noti-follow').show();
        $('#noti-number-follow').text(listFollow.length);
    }else {
        $('#noti-follow').hide();
    }
}
function resetNotiFollow() {
    localStorage.setItem("LIST_ID_FOLLOW", '');
    loadNotiFollow();
}

function OpenLoading() {
    $("#loading-overlay").fadeIn();
}
function OffLoading() {
    $("#loading-overlay").fadeOut();
}