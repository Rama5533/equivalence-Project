using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using API.Enums;

namespace API.Entities;

public class Applicant
{
    public string Id { get; set; } = null!;

    public string? IdentityType { get; set; }

    public required string DisplayName { get; set; }

    public string? ImageUrl { get; set; }

    public string? NationalId { get; set; }

    public string? IdImageUrl { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Nationality { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsAppNumber { get; set; }

    public string? AlternativeEmail { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    public List< ApplicantQualification> qualifications { get; set; }=[];

     public List< EquivalencyApplication> EquivalencyApplications { get; set; }=[];


    [JsonIgnore]
    public List<Photo> Photos { get; set; } = [];

    [JsonIgnore]
    [ForeignKey(nameof(Id))]
    public AppUser User { get; set; } = null!;
}