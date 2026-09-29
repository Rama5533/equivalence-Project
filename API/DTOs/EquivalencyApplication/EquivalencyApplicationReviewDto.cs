using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using API.Enums;

namespace API.DTOs.EquivalencyApplication;

public class EquivalencyApplicationReviewDto
{
    public int ApplicationId { get; set; }

    public QualificationType QualificationType { get; set; }

    public string Status { get; set; } = string.Empty;

    public int CurrentStep { get; set; }

    public ApplicantReviewDto Applicant { get; set; } = null!;

    public CertificateReviewDto Certificate { get; set; } = null!;

    public List<DocumentReviewDto> Documents { get; set; } = [];
}