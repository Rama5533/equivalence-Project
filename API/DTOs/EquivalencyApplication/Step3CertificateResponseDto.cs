using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.EquivalencyApplication;

public class Step3CertificateResponseDto
{
    public int ApplicationId { get; set; }

    public short CountryId { get; set; }

    public int InstitutionId { get; set; }

    public int MajorId { get; set; }

    public int GraduationYear { get; set; }

    public string? AdditionalNotes { get; set; }

    public int CurrentStep { get; set; }
}