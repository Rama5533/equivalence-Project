using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace API.DTOs.EquivalencyApplication;

public class SubmitApplicationResponseDto
{
    public int ApplicationId { get; set; }

    public string Status { get; set; } = string.Empty;

    public int CurrentStep { get; set; }

    public DateTime SubmittedAt { get; set; }
}