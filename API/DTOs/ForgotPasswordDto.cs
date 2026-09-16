namespace API.DTOs;

// هذا الـ DTO يستقبل إيميل المستخدم عند طلب نسيان كلمة المرور
public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}