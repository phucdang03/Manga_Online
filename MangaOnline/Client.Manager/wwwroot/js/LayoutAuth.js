const urlClient = "http://localhost:5030";
const urlServiceManga = "http://localhost:5098";
    
function SaveAccount(email,password) {
    localStorage.setItem("Email",email);
    localStorage.setItem("Password",password);
}
function GetAccount() {
    const acc = {
        email : localStorage.getItem("Email"),
        password : localStorage.getItem("Password"),
    }
    return acc;
}
function GetUserData() {
    const user = localStorage.getItem("USER_DATA");
    return $.parseJSON(user);
    // {"userId":"ef20386b-2335-4f65-87b2-819e1aef6612","userName":"traicv","email":"enm55606@nezid.com","role":"UserNormal","avartar":null}
}

function OpenLoading() {
    $("#loading-overlay").fadeIn();
}
function OffLoading() {
    $("#loading-overlay").fadeOut();
}