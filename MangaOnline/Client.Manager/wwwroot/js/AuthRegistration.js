const phoneNumber = document.getElementById("phoneNumber");
const email = document.getElementById("email");
const password = document.getElementById("password");
const passwordConfirmed = document.getElementById("passwordConfirmed");
const submitBtn = document.getElementById("submitBtn");

let notiPhoneNumber = document.getElementById("noti-phoneNumber");
let notiEmail = document.getElementById("noti-email");
let notiPassword = document.getElementById("noti-password");
let notiPasswordConfirmed = document.getElementById("noti-passwordConfirmed");
let notiFullName = document.getElementById("noti-fullName");


submitBtn.addEventListener("click", function(e) {
    let fail = false;
    e.preventDefault();
    
    notiPhoneNumber.innerHTML = "";
    notiEmail.innerHTML = "";
    notiPassword.innerHTML = "";
    notiPasswordConfirmed.innerHTML = "";
    notiFullName.innerHTML = "";

    if($('#fullName').val().length<6){
        notiFullName.innerHTML = "Tên người dùng ít nhất 6 ký tự";
        fail = true;
    }
    if(!/^0\d{9}$/.test(phoneNumber.value)){
        notiPhoneNumber.innerHTML = "số điện thoại sai";
        fail = true;
    }
    if(!/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email.value)){
        notiEmail.innerHTML = "email sai";
        fail = true;
    }
    if(password.value.length<6){
        notiPassword.innerHTML = "mật khẩu ít nhất 6 ký tự";
        fail = true;
    }
    if(password.value!==passwordConfirmed.value){
        notiPasswordConfirmed.innerHTML = "mật khẩu không trùng khớp";
        fail = true;
    }
    if(!fail){
        Register();
    }
});

const passwordConfirmedInput = document.getElementById("passwordConfirmed");
function showPasswordConfirmed() {
    const btnPasswordConfirmed = document.getElementById("btnPasswordConfirmed");
    if (passwordConfirmedInput.type === "password") {
        passwordConfirmedInput.type = "text";
        btnPasswordConfirmed.innerHTML = '<i class="icon ion-ios-eye-off icon-custom-password"></i>';
    } else {
        passwordConfirmedInput.type = "password";
        btnPasswordConfirmed.innerHTML = '<i class="icon ion-ios-eye icon-custom-password"></i>';
    }
}

const newPasswordInput = document.getElementById("password");
function showNewPassword() {
    const btnNewPassword = document.getElementById("btnNewPassword");
    if (newPasswordInput.type === "password") {
        newPasswordInput.type = "text";
        btnNewPassword.innerHTML = '<i class="icon ion-ios-eye-off icon-custom-password"></i>';
    } else {
        newPasswordInput.type = "password";
        btnNewPassword.innerHTML = '<i class="icon ion-ios-eye icon-custom-password"></i>';
    }
}
function Register() {
    $.ajax({
        url: 'http://localhost:5098/Auth/AuthRegister',
        type: "POST",
        data: {
            FullName: $('#fullName').val(),
            Email: $('#email').val(),
            Password: $('#password').val(),
            PhoneNumber: $('#phoneNumber').val(),
        },
        success: function (response) {
            window.location.href = 'http://localhost:5030/Auth/AuthLogin';
        },
        error: function (xhr, status, error) {
            ShowNotificationError(xhr.responseJSON.error??'Tạo thất bại');
        }
    });
}
