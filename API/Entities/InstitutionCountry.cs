using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities;

public class InstitutionCountry
{
    public short ID { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;
}