using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities;

public class Institution
{
    public short InstituteID { get; set; }

    public string InstituteName { get; set; } = string.Empty;

    public string? InstituteNameEn { get; set; }

    public short CountryID { get; set; }

    public bool? IsActive { get; set; }
}