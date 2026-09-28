using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities;

public class EquivalencyInstitution
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? NameEn { get; set; }

    public short CountryId { get; set; }

    public bool? IsActive { get; set; }

    public EquivalencyCountry Country { get; set; } = null!;

    public ICollection<EquivalencyMajor> Majors { get; set; }
        = new List<EquivalencyMajor>();
}