using System.ComponentModel;

namespace Service.MangaOnline.Commons;

public enum NotificationEnum
{
    [Description("Email đã được đăng ký")]
    EmailExists,
    [Description("Email sai")]
    SendEmailError,
    [Description("Yêu cầu bị từ chối")]
    RequestFailed,
    [Description("Xác nhận email thành công")]
    VerifyEmailSuccess,
    [Description("Đăng nhập thất bại")]
    LoginFailed,
    [Description("Sai mật khẩu")]
    PasswordFailed,
    [Description("Thay đổi mật khẩu thành công")]
    ChangePasswordSuccess,
    [Description("không tìm thấy User")]
    UserNotFound,
    [Description("Đã đăng ký tài khoản")]
    RegisterAccountSuccess,
    [Description("File size vượt quá dung lượng cho phép")]
    SizeImageLimit,
    [Description("Chúc mừng bạn trở thành người dùng vip")]
    RoleUp
}