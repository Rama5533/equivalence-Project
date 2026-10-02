using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Entities;

public class EquivalencyMajor
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? NameEn { get; set; }

    public int InstitutionId { get; set; }

    public bool? IsActive { get; set; }

    public EquivalencyInstitution Institution { get; set; } = null!;

    public MajorKind Kind { get; set; }
    = MajorKind.UniversityMajor;
}