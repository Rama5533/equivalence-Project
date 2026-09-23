using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.DTOs.EquivalencyApplication;

public class EquivalencyApplicationDto
{
    public int Id { get; set; }

    public QualificationType QualificationType { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }
}