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
        public string ApplicantId { get; set; }= string.Empty;
        public QualificationType qualificationType { get; set; }
        public Applicant Applicant { get; set; }=null!;
    }
}