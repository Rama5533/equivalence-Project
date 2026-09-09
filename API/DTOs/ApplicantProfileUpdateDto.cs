namespace API.DTOs;

public class ApplicantProfileUpdateDto
{
    public string DisplayName { get; set; } = "";

    public string? NationalId { get; set; }

    public string? IdentityType { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Nationality { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsAppNumber { get; set; }

    public string? AlternativeEmail { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }
}