namespace API.DTOs;

// هذا الـ DTO يمثل البيانات اللي يحتاجها الأدمن
// لعرض الطلبات في جدول لوحة التحكم
public class AdminApplicationDto
{
    // رقم الطلب داخل قاعدة البيانات
    public int Id { get; set; }

    // رقم الطلب اللي رح يظهر بالواجهة
    public string RequestNumber { get; set; } = string.Empty;

    // اسم مقدم الطلب
    public string ApplicantName { get; set; } = string.Empty;

    // نوع المؤهل: بكالوريوس، ماجستير...
    public string QualificationType { get; set; } = string.Empty;

    // حالة الطلب الحالية
    public string Status { get; set; } = string.Empty;

    // تاريخ الطلب
    public DateTime Date { get; set; }
}