using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.Entities;

public class EquivalencyApplication
{
    public int Id { get; set; }

    // Applicant who submitted the application
    public string ApplicantId { get; set; } = string.Empty;

    public Applicant Applicant { get; set; } = null!;

    // Qualification requested
    public QualificationType QualificationType { get; set; }

    // Current step in the application workflow
    public int CurrentStep { get; set; } = 1;

    // Country
    public short? CountryId { get; set; }

    public EquivalencyCountry? Country { get; set; }

    // Institution / University
    public int? InstitutionId { get; set; }

    public EquivalencyInstitution? Institution { get; set; }

    // Major / Specialization
    public int? MajorId { get; set; }

    public EquivalencyMajor? Major { get; set; }

    // Certificate information
    public int? GraduationYear { get; set; }

    // Application information
    public string Status { get; set; } = "Draft";

    public string? AdditionalNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? SubmittedAt { get; set; }

    // Documents uploaded for this application
    public ICollection<ApplicationDocument> Documents { get; set; }
        = new List<ApplicationDocument>();
}