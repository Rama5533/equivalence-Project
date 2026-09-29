using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.DTOs.EquivalencyApplication;

public class ApplicationReviewDto
{
    public int Id { get; set; }

    public QualificationType QualificationType { get; set; }

    public string Status { get; set; } = string.Empty;

    public int CurrentStep { get; set; }


    // Applicant information
    public string ApplicantId { get; set; } = string.Empty;

    public string? ApplicantName { get; set; }

    public string? NationalId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }


    // Certificate information
    public short? CountryId { get; set; }

    public string? CountryName { get; set; }

    public string? CountryNameEn { get; set; }


    public int? InstitutionId { get; set; }

    public string? InstitutionName { get; set; }

    public string? InstitutionNameEn { get; set; }


    public int? MajorId { get; set; }

    public string? MajorName { get; set; }

    public string? MajorNameEn { get; set; }


    public int? GraduationYear { get; set; }

    public string? AdditionalNotes { get; set; }


    // Documents
    public int DocumentsCount { get; set; }

    public List<ApplicationDocumentReviewDto> Documents { get; set; }
        = new();
}


public class ApplicationDocumentReviewDto
{
    public int Id { get; set; }

    public DocumentType DocumentType { get; set; }

    public string? FileName { get; set; }

    public string Url { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

}
