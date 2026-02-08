var connectionNotification = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5098/hubs/notification").build();


connectionNotification.on("LoadNotification", function (message) {
    if (localStorage.getItem("MANGA_HISTORY") !== '' && localStorage.getItem("MANGA_HISTORY") !== null) {
        let listHistory = stringToArray(localStorage.getItem("MANGA_HISTORY"));
        let index = $.inArray(message.toUpperCase(), listHistory);
        if (index !== -1) {
            // co trong array 
            $('#noti-history').show();
            updateNotiFollow(message);
        } else {
            // co trong array 
            console.log(111, 'ko');
        }
    }

        if (localStorage.getItem("MANGA_FOLLOW") !== ''&& localStorage.getItem("MANGA_FOLLOW") !== null) {
        let listFollow = stringToArray(localStorage.getItem("MANGA_FOLLOW"));
        let index = $.inArray(message.toUpperCase(), listFollow);
        if (index !== -1) {
            // co trong array 
            $('#noti-follow').show();
            updateNotiFollow(message);
        } else {
            // co trong array 
            console.log(111, 'ko');
        }
    }
});

connectionNotification.start().then(function () {
    // connectionNotification.send("LoadMessages");
}).catch(function (err) {
    return console.error(err.toString());
});

function updateNotiHistory(mangaId) {
    if (localStorage.getItem("LIST_ID_HISTORY") !== '' && localStorage.getItem("LIST_ID_HISTORY") !== null) {
        let listHistory = stringToArray(localStorage.getItem("LIST_ID_HISTORY"));
        let index = $.inArray(mangaId.toUpperCase(), listHistory);
        if (index === -1) {
            listHistory.push(mangaId.toUpperCase());
            localStorage.setItem("LIST_ID_HISTORY", listHistory);
            $('#noti-number-history').text(listHistory.length);
        }
    } else {
        localStorage.setItem("LIST_ID_FOLLOW", mangaId.toUpperCase());
    }
}
function updateNotiFollow(mangaId) {
    if (localStorage.getItem("LIST_ID_FOLLOW") !== '' &&localStorage.getItem("LIST_ID_FOLLOW") !== null ) {
        let listFollow = stringToArray(localStorage.getItem("LIST_ID_FOLLOW"));
        let index = $.inArray(mangaId.toUpperCase(), listFollow);
        if (index === -1) {
            listFollow.push(mangaId.toUpperCase());
            localStorage.setItem("LIST_ID_FOLLOW", listFollow);
            $('#noti-number-follow').text(listFollow.length);
        }
    }else {
        localStorage.setItem("LIST_ID_FOLLOW", mangaId.toUpperCase());
    }
}
