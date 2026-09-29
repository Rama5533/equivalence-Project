using System;
using API.Enums;

namespace API.DTOs.EquivalencyApplication;

public class EquivalencyApplicationDto
{
    public int Id { get; set; }

    public QualificationType QualificationType { get; set; }

    public string Status { get; set; } = string.Empty;

    public int CurrentStep { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }
}