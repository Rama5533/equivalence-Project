namespace API.DTOs;

// هذا الـ DTO يمثل تفاصيل طلب واحد للأدمن
public class AdminApplicationDetailsDto
{
    public int Id { get; set; }

    public string RequestNumber { get; set; } = string.Empty;

    // بيانات مقدم الطلب
    public string ApplicantName { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    // بيانات الطلب
    public string QualificationType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}