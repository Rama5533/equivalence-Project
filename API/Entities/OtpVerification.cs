namespace API.Entities;

// هذا الكيان يخزن OTP بشكل مؤقت
// ويُستخدم سواء للتحقق من الإيميل وقت التسجيل
// أو للمصادقة الثنائية وقت تسجيل الدخول
public class OtpVerification
{
    public int Id { get; set; }

    // الإيميل اللي رح نبعث عليه الكود
    public string Email { get; set; } = string.Empty;

    // كود الـ OTP
    public string OtpCode { get; set; } = string.Empty;

    // وقت انتهاء صلاحية الكود
    public DateTime ExpiresAt { get; set; }

    // يحدد سبب استخدام الـ OTP
    // Register أو Login2FA
    public string Purpose { get; set; } = string.Empty;

    // هل تم التحقق من الكود بنجاح؟
    public bool IsVerified { get; set; } = false;

    // وقت إنشاء الكود
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}