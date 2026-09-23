namespace API.DTOs;

// هذا الـ DTO يستقبل إيميل المستخدم
// حتى نولّد OTP ونبعته على الإيميل
public class SendOtpDto
{
    public string Email { get; set; } = string.Empty;
}