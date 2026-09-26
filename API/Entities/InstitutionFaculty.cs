using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities;

public class InstitutionFaculty
{
    public int FacultyID { get; set; }

    public short InstituteID { get; set; }

    public int CampusID { get; set; }

    public string FacultyName { get; set; } = string.Empty;

    public string? FacultyNameEn { get; set; }

    public bool? IsActive { get; set; }
}