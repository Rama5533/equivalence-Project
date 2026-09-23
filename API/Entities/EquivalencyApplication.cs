using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.Entities;
    public class EquivalencyApplication
    {
        public int Id { get; set; }
        public string ApplicantId { get; set; }=string.Empty;
        public QualificationType QualificationType { get; set; }
        public string Status { get; set; }="Draft";
        public DateTime CreatedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public Applicant Applicant { get; set; }=null!;
    }
