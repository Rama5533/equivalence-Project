namespace API.DTOs.EquivalencyApplication;

public class Step2ApplicantDto
{
    public int ApplicationId { get; set; }

    public string ApplicantId { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string? NationalId { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsProfileComplete { get; set; }

    public List<string> MissingFields { get; set; } = new();

    public int CurrentStep { get; set; }
}