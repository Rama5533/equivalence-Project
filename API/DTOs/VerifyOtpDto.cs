namespace API.DTOs;

// هذا الـ DTO يستقبل الإيميل وكود الـ OTP
// حتى نتحقق إذا الكود صحيح ولسا ما انتهت صلاحيته
public class VerifyOtpDto
{
    public string Email { get; set; } = string.Empty;

    public string Otp { get; set; } = string.Empty;
}