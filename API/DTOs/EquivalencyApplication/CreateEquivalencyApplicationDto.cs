using System.Collections.Generic;
using API.Enums;

namespace API.DTOs.EquivalencyApplication;

public class CreateEquivalencyApplicationDto
{
    public QualificationType QualificationType { get; set; }

    public List<PreviousQualificationDto> PreviousQualifications { get; set; }
        = new();
}

public class PreviousQualificationDto
{
    public QualificationType QualificationType { get; set; }

    public short CountryId { get; set; }

    public int InstitutionId { get; set; }

    public int GraduationYear { get; set; }
}