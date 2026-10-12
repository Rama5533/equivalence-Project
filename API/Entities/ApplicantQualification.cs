using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;
using API.Entities;

namespace API.Entities
{
    public class ApplicantQualification
    {
        public int Id { get; set; }

        public string ApplicantId { get; set; } = string.Empty;

        public QualificationType QualificationType { get; set; }

        public Applicant Applicant { get; set; } = null!;

        public short CountryId { get; set; }

        public EquivalencyCountry Country { get; set; } = null!;

        public int InstitutionId { get; set; }

        public EquivalencyInstitution Institution { get; set; } = null!;

        public int GraduationYear { get; set; }
    }
}