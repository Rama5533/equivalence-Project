using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.EquivalencyApplication;

public class CertificateReviewDto
{
    public short CountryId { get; set; }

    public string CountryName { get; set; } = string.Empty;

    public int InstitutionId { get; set; }

    public string InstitutionName { get; set; } = string.Empty;

    public int MajorId { get; set; }

    public string MajorName { get; set; } = string.Empty;

    public int GraduationYear { get; set; }

    public string? AdditionalNotes { get; set; }
}