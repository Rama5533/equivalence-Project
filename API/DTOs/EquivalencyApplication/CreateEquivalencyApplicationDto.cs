using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.DTOs.EquivalencyApplication;

public class CreateEquivalencyApplicationDto
{
    public QualificationType QualificationType { get; set; }

    public PreviousQualificationDto? PreviousQualification { get; set; }
}
public class PreviousQualificationDto
{
    public QualificationType QualificationType { get; set; }

    public short CountryId { get; set; }

    public int InstitutionId { get; set; }

    public int GraduationYear { get; set; }
}