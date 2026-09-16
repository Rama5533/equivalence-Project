namespace API.DTOs;

// هذا الـ DTO يستقبل بيانات إعادة تعيين كلمة المرور:
// الإيميل + التوكن + كلمة المرور الجديدة
public class ResetPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}