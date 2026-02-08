const newPasswordInput = document.getElementById("newPasswordInput");
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


$(document).ready(function() {
    $('#email').val(GetAccount().email);
    $('#newPasswordInput').val(GetAccount().password);
});
function login(ipAddress) {
    var checkboxValue = $('#remember:checked').val();
    var email = $('#email').val();
    var password = $('#newPasswordInput').val();
    $.ajax({
        url: 'http://localhost:5098/Auth/AuthLogin',
        type: "POST",
        data: {
            email: email,
            password: password,
            ipAddress: ipAddress
        },
        async: true,
        success: function (response) {
            OffLoading();
            if(checkboxValue === 'on'){
                SaveAccount(email,password);
            }else {
                SaveAccount('','');
            }
            localStorage.setItem("USER_DATA",JSON.stringify(response.data.value.userData));
            localStorage.setItem("ACCESS_TOKEN",response.data.value.accessToken);
            localStorage.setItem("MANGA_FOLLOW",response.data.value.listFollow);
            // ShowNotificationSuccess("Đăng nhập thành công");
            window.location.href = urlClient;
        },
        error: function (xhr, status, error) {
            ShowNotificationError(xhr.responseJSON.error??'Đăng nhập thất bại');
        }
    });
}
function GetIpUser() {
    OpenLoading();
    $.ajax({
        url: 'https://httpbin.org/ip',
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            // Dữ liệu trả về sẽ có dạng {"origin": "địa chỉ IP"}
            const ipAddress = data.origin;
            login(ipAddress);
        },
        error: function () {
            OffLoading();
            ShowNotificationError('Thử lại đường truyền không ổn định');
            // login(1111);
        }
    });
}