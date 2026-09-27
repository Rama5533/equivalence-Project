using API.Enums;

namespace API.Entities;

public class EquivalencyApplication
{
    public int Id { get; set; }

    // صاحب الطلب
    public string ApplicantId { get; set; } = string.Empty;

    // نوع المؤهل
    public QualificationType QualificationType { get; set; }

    // حالة الطلب
    public string Status { get; set; } = "Draft";

    // تاريخ إنشاء الطلب
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // تاريخ تقديم الطلب
    public DateTime? SubmittedAt { get; set; }

    // صاحب الطلب
    public Applicant Applicant { get; set; } = null!;
}