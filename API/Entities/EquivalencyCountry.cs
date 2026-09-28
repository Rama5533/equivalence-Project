using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities;

public class EquivalencyCountry
{
    public short Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public ICollection<EquivalencyInstitution> Institutions { get; set; }
        = new List<EquivalencyInstitution>();
}