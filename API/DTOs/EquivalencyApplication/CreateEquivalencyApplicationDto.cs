using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.DTOs.EquivalencyApplication;

public class CreateEquivalencyApplicationDto
{
    public QualificationType QualificationType { get; set; }
}
