using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.EquivalencyApplication;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using API.Enums;
using Microsoft.AspNetCore.Http;

namespace API.Services;

public class EquivalencyApplicationService : IEquivalencyApplicationService
{
    private readonly AppDbContext _context;

    public EquivalencyApplicationService(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // STEP 1
    // Create a new draft application
    // =========================================================

    public async Task<EquivalencyApplicationDto> CreateDraftAsync(
        string applicantId,
        CreateEquivalencyApplicationDto dto)
    {
        // 1. Check that the applicant exists
        var applicantExists = await _context.Applicants
            .AnyAsync(a => a.Id == applicantId);

        if (!applicantExists)
        {
            throw new KeyNotFoundException("Applicant not found.");
        }

        // 2. Get required previous qualifications
        var requiredQualifications = await _context.QualificationRequirements
            .Where(r => r.QualificationType == dto.QualificationType)
            .Select(r => r.RequiredQualificationType)
            .ToListAsync();

        // 3. Get qualifications already owned by the applicant
        var applicantQualifications = await _context.ApplicantQualifications
            .Where(q => q.ApplicantId == applicantId)
            .Select(q => q.qualificationType)
            .ToListAsync();

        // 4. Check all required qualifications
        foreach (var requiredQualification in requiredQualifications)
        {
            if (!applicantQualifications.Contains(requiredQualification))
            {
                throw new InvalidOperationException(
                    $"Applicant must have {requiredQualification} qualification first.");
            }
        }

        // 5. Create the application draft
        var application = new EquivalencyApplication
        {
            ApplicantId = applicantId,
            QualificationType = dto.QualificationType,
            CurrentStep = 1,
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        };

        // 6. Save application
        _context.EquivalencyApplications.Add(application);

        await _context.SaveChangesAsync();

        // 7. Return created application
        return new EquivalencyApplicationDto
        {
            Id = application.Id,
            QualificationType = application.QualificationType,
            Status = application.Status,
            CurrentStep = application.CurrentStep,
            CreatedAt = application.CreatedAt,
            SubmittedAt = application.SubmittedAt
        };
    }

    // =========================================================
    // STEP 2
    // Get applicant profile data
    // =========================================================
    public async Task<Step2ApplicantDto> CompleteStep2Async(
        string applicantId,
        int applicationId)
    {
        // Find the application belonging to the logged-in applicant
        var application = await _context.EquivalencyApplications
            .Include(a => a.Applicant)
            .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(a =>
                a.Id == applicationId &&
                a.ApplicantId == applicantId);

        if (application == null)
        {
            throw new KeyNotFoundException(
                "Application not found.");
        }

        // Step 2 can only be reached after Step 1
        if (application.CurrentStep < 1)
        {
            throw new InvalidOperationException(
                "Step 1 must be completed first.");
        }

        var applicant = application.Applicant;

        // Required fields for Step 2
        var missingFields = new List<string>();

        // 1. Full Name
        if (string.IsNullOrWhiteSpace(applicant.DisplayName))
        {
            missingFields.Add("FullName");
        }

        // 2. National ID
        if (string.IsNullOrWhiteSpace(applicant.NationalId))
        {
            missingFields.Add("NationalId");
        }

        // 3. Email from AppUser
        if (string.IsNullOrWhiteSpace(applicant.User.Email))
        {
            missingFields.Add("Email");
        }

        // 4. Address = Proof of Residence
        if (string.IsNullOrWhiteSpace(applicant.Address))
        {
            missingFields.Add("Address");
        }

        // 5. Phone Number
        if (string.IsNullOrWhiteSpace(applicant.PhoneNumber))
        {
            missingFields.Add("PhoneNumber");
        }

        // Check if all required data exists
        var isProfileComplete = missingFields.Count == 0;

        // If something is missing,
        // keep the application on Step 1.
        if (!isProfileComplete)
        {
            return new Step2ApplicantDto
            {
                ApplicationId = application.Id,
                ApplicantId = applicant.Id,

                FullName = applicant.DisplayName,
                NationalId = applicant.NationalId,
                Email = applicant.User.Email,
                Address = applicant.Address,
                PhoneNumber = applicant.PhoneNumber,

                IsProfileComplete = false,
                MissingFields = missingFields,

                CurrentStep = application.CurrentStep
            };
        }

        // All required profile data exists.
        // Move the application to Step 2.
        if (application.CurrentStep == 1)
        {
            application.CurrentStep = 2;

            await _context.SaveChangesAsync();
        }

        return new Step2ApplicantDto
        {
            ApplicationId = application.Id,
            ApplicantId = applicant.Id,

            FullName = applicant.DisplayName,
            NationalId = applicant.NationalId,
            Email = applicant.User.Email,
            Address = applicant.Address,
            PhoneNumber = applicant.PhoneNumber,

            IsProfileComplete = true,
            MissingFields = new List<string>(),

            CurrentStep = application.CurrentStep
        };
    }

    // =========================================================
// STEP 3
// =========================================================

public async Task<Step3CertificateResponseDto> CompleteStep3Async(
    string applicantId,
    int applicationId,
    Step3CertificateDto dto)
{
    var application = await _context.EquivalencyApplications
        .FirstOrDefaultAsync(a =>
            a.Id == applicationId &&
            a.ApplicantId == applicantId);

    if (application == null)
    {
        throw new KeyNotFoundException(
            "Application not found.");
    }

    // Step 2 must be completed first
    if (application.CurrentStep < 2)
    {
        throw new InvalidOperationException(
            "Step 2 must be completed first.");
    }

    // -----------------------------------------
    // Validate Country
    // -----------------------------------------

    var countryExists = await _context.EquivalencyCountries
        .AnyAsync(c => c.Id == dto.CountryId);

    if (!countryExists)
    {
        throw new InvalidOperationException(
            "Invalid country.");
    }

    // -----------------------------------------
    // Validate Institution
    // Institution must belong to Country
    // -----------------------------------------

    var institutionExists = await _context.EquivalencyInstitutions
        .AnyAsync(i =>
            i.Id == dto.InstitutionId &&
            i.CountryId == dto.CountryId);

    if (!institutionExists)
    {
        throw new InvalidOperationException(
            "Invalid institution for the selected country.");
    }

    // -----------------------------------------
    // Validate Major
    // Major must belong to Institution
    // -----------------------------------------

    var majorExists = await _context.EquivalencyMajors
        .AnyAsync(m =>
            m.Id == dto.MajorId &&
            m.InstitutionId == dto.InstitutionId);

    if (!majorExists)
    {
        throw new InvalidOperationException(
            "Invalid major for the selected institution.");
    }

    // -----------------------------------------
    // Validate Graduation Year
    // -----------------------------------------

    var currentYear = DateTime.UtcNow.Year;

    if (dto.GraduationYear < 1900 ||
        dto.GraduationYear > currentYear)
    {
        throw new InvalidOperationException(
            "Invalid graduation year.");
    }

    // -----------------------------------------
    // Save Step 3
    // -----------------------------------------

    application.CountryId = dto.CountryId;
    application.InstitutionId = dto.InstitutionId;
    application.MajorId = dto.MajorId;
    application.GraduationYear = dto.GraduationYear;
    application.AdditionalNotes = dto.AdditionalNotes;

    // Move from Step 2 → Step 3
    if (application.CurrentStep == 2)
    {
        application.CurrentStep = 3;
    }

    await _context.SaveChangesAsync();

    return new Step3CertificateResponseDto
    {
        ApplicationId = application.Id,

        CountryId = application.CountryId!.Value,

        InstitutionId = application.InstitutionId!.Value,

        MajorId = application.MajorId!.Value,

        GraduationYear = application.GraduationYear!.Value,

        AdditionalNotes = application.AdditionalNotes,

        CurrentStep = application.CurrentStep
    };
    }

    // =========================================================
// STEP 4 - GET REQUIRED DOCUMENTS
// =========================================================

public async Task<List<RequiredDocumentDto>> GetRequiredDocumentsAsync(
    string applicantId,
    int applicationId)
{
    var application = await _context.EquivalencyApplications
        .FirstOrDefaultAsync(a =>
            a.Id == applicationId &&
            a.ApplicantId == applicantId);

    if (application == null)
    {
        throw new KeyNotFoundException(
            "Application not found.");
    }

    if (application.CurrentStep < 3)
    {
        throw new InvalidOperationException(
            "Step 3 must be completed first.");
    }

if (application.QualificationType != QualificationType.Secondary &&
    application.QualificationType != QualificationType.Bachelor)
{
    throw new InvalidOperationException(
        "Document requirements for this qualification are not configured yet.");
}

    return new List<RequiredDocumentDto>
    {
        new()
        {
            DocumentType = DocumentType.PersonalPhoto,
            Name = "Recent Personal Photo",
            Required = true
        },

        new()
        {
            DocumentType = DocumentType.PassportOrApprovedId,
            Name = "Certified Passport or Approved Identification Document",
            Required = true
        },

        new()
        {
            DocumentType = DocumentType.BachelorCertificate,
            Name = "Certified Bachelor Certificate or Equivalent",
            Required = true
        },

        new()
        {
            DocumentType = DocumentType.BachelorTranscript,
            Name = "Certified Transcript for Required Subjects and Academic Years",
            Required = true
        }
    };
}

// =========================================================
// STEP 4 - UPLOAD DOCUMENT
// =========================================================

public async Task<UploadDocumentResponseDto> UploadDocumentAsync(
    string applicantId,
    int applicationId,
    DocumentType documentType,
    IFormFile file)
{
    var application = await _context.EquivalencyApplications
        .FirstOrDefaultAsync(a =>
            a.Id == applicationId &&
            a.ApplicantId == applicantId);

    if (application == null)
    {
        throw new KeyNotFoundException(
            "Application not found.");
    }

    if (application.CurrentStep < 3)
    {
        throw new InvalidOperationException(
            "Step 3 must be completed first.");
    }

if (application.QualificationType != QualificationType.Secondary &&
    application.QualificationType != QualificationType.Bachelor)
{
    throw new InvalidOperationException(
        "Document requirements for this qualification are not configured yet.");
}

    if (file == null || file.Length == 0)
    {
        throw new InvalidOperationException(
            "File is required.");
    }

    var allowedExtensions = new[]
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".pdf"
    };

    var extension = Path.GetExtension(file.FileName)
        .ToLowerInvariant();

    if (!allowedExtensions.Contains(extension))
    {
        throw new InvalidOperationException(
            "Only JPG, JPEG, PNG, and PDF files are allowed.");
    }

    // Maximum 10 MB
    const long maxFileSize = 10 * 1024 * 1024;

    if (file.Length > maxFileSize)
    {
        throw new InvalidOperationException(
            "File size cannot exceed 10 MB.");
    }

    // Check if this document was uploaded before
    var existingDocument = await _context.ApplicationDocuments
        .FirstOrDefaultAsync(d =>
            d.EquivalencyApplicationId == applicationId &&
            d.DocumentType == documentType);

    if (existingDocument != null)
    {
        throw new InvalidOperationException(
            "This document has already been uploaded.");
    }

    var uploadsFolder = Path.Combine(
        Directory.GetCurrentDirectory(),
        "wwwroot",
        "uploads",
        "applications",
        applicationId.ToString());

    Directory.CreateDirectory(uploadsFolder);

    var uniqueFileName =
        $"{Guid.NewGuid()}{extension}";

    var filePath = Path.Combine(
        uploadsFolder,
        uniqueFileName);

    await using (var stream = new FileStream(
        filePath,
        FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var url =
        $"/uploads/applications/{applicationId}/{uniqueFileName}";

    var document = new ApplicationDocument
    {
        EquivalencyApplicationId = applicationId,
        DocumentType = documentType,
        Url = url,
        FileName = file.FileName,
        UploadedAt = DateTime.UtcNow
    };

    _context.ApplicationDocuments.Add(document);

    await _context.SaveChangesAsync();

    return new UploadDocumentResponseDto
    {
        ApplicationId = applicationId,
        DocumentId = document.Id,
        DocumentType = document.DocumentType,
        FileName = document.FileName,
        Url = document.Url,
        UploadedAt = document.UploadedAt
    };
}

// =========================================================
// STEP 4 - COMPLETE
// =========================================================

public async Task CompleteStep4Async(
    string applicantId,
    int applicationId)
{
    var application = await _context.EquivalencyApplications
        .FirstOrDefaultAsync(a =>
            a.Id == applicationId &&
            a.ApplicantId == applicantId);

    if (application == null)
    {
        throw new KeyNotFoundException(
            "Application not found.");
    }

    if (application.CurrentStep < 3)
    {
        throw new InvalidOperationException(
            "Step 3 must be completed first.");
    }

if (application.QualificationType != QualificationType.Secondary &&
    application.QualificationType != QualificationType.Bachelor)
{
    throw new InvalidOperationException(
        "Document requirements for this qualification are not configured yet.");
}    var requiredDocumentTypes = new[]
    {
        DocumentType.PersonalPhoto,
        DocumentType.PassportOrApprovedId,
        DocumentType.BachelorCertificate,
        DocumentType.BachelorTranscript
    };

    var uploadedDocumentTypes = await _context.ApplicationDocuments
        .Where(d =>
            d.EquivalencyApplicationId == applicationId &&
            requiredDocumentTypes.Contains(d.DocumentType))
        .Select(d => d.DocumentType)
        .ToListAsync();

    var missingDocuments = requiredDocumentTypes
        .Except(uploadedDocumentTypes)
        .ToList();

    if (missingDocuments.Count > 0)
    {
        throw new InvalidOperationException(
            "All required documents must be uploaded before completing Step 4.");
    }

    if (application.CurrentStep == 3)
    {
        application.CurrentStep = 4;

        await _context.SaveChangesAsync();
    }
}

// =========================================================
// STEP 5 - APPLICATION REVIEW
// =========================================================


public async Task<EquivalencyApplicationReviewDto> GetReviewAsync(
    string applicantId,
    int applicationId)
{
    var application = await _context.EquivalencyApplications
        .Include(a => a.Applicant)
            .ThenInclude(a => a.User)
        .Include(a => a.Country)
        .Include(a => a.Institution)
        .Include(a => a.Major)
        .Include(a => a.Documents)
        .FirstOrDefaultAsync(a =>
            a.Id == applicationId &&
            a.ApplicantId == applicantId);

    if (application == null)
    {
        throw new KeyNotFoundException(
            "Equivalency application not found.");
    }

    if (application.CurrentStep < 4)
    {
        throw new InvalidOperationException(
            "The application must complete Step 4 before review.");
    }

    if (application.Applicant == null)
    {
        throw new InvalidOperationException(
            "Applicant information is not available.");
    }

    if (application.Applicant.User == null)
    {
        throw new InvalidOperationException(
            "Applicant login information is not available.");
    }

    if (application.Country == null ||
        application.Institution == null ||
        application.Major == null ||
        application.GraduationYear == null)
    {
        throw new InvalidOperationException(
            "Certificate information is incomplete.");
    }

    var review = new EquivalencyApplicationReviewDto
    {
        ApplicationId = application.Id,

        QualificationType = application.QualificationType,

        Status = application.Status,

        CurrentStep = application.CurrentStep,

        Applicant = new ApplicantReviewDto
        {
            FullName = application.Applicant.DisplayName,

            NationalId = application.Applicant.NationalId ?? string.Empty,

            Email = application.Applicant.User.Email ?? string.Empty,

            Address = application.Applicant.Address ?? string.Empty,

            PhoneNumber = application.Applicant.PhoneNumber ?? string.Empty
        },

        Certificate = new CertificateReviewDto
        {
            CountryId = application.Country.Id,

            CountryName = application.Country.Name,

            InstitutionId = application.Institution.Id,

            InstitutionName = application.Institution.Name,

            MajorId = application.Major.Id,

            MajorName = application.Major.Name,

            GraduationYear = application.GraduationYear.Value,

            AdditionalNotes = application.AdditionalNotes
        },

        Documents = application.Documents
            .OrderBy(d => d.DocumentType)
            .Select(d => new DocumentReviewDto
            {
                DocumentId = d.Id,

                DocumentType = d.DocumentType,

                FileName = d.FileName ?? string.Empty,

                Url = d.Url,

                UploadedAt = d.UploadedAt
            })
            .ToList()
    };

    return review;
}


// =========================================================
// STEP 6 - APPLICATION SUBMISSION
// =========================================================

public async Task<SubmitApplicationResponseDto> SubmitAsync(
    string applicantId,
    int applicationId)
{
    var application = await _context.EquivalencyApplications
        .Include(a => a.Applicant)
            .ThenInclude(a => a.User)
        .Include(a => a.Country)
        .Include(a => a.Institution)
        .Include(a => a.Major)
        .Include(a => a.Documents)
        .FirstOrDefaultAsync(a =>
            a.Id == applicationId &&
            a.ApplicantId == applicantId);

    if (application == null)
    {
        throw new KeyNotFoundException(
            "Equivalency application not found.");
    }

    if (application.Status != "Draft")
    {
        throw new InvalidOperationException(
            "Only draft applications can be submitted.");
    }

    if (application.CurrentStep < 4)
    {
        throw new InvalidOperationException(
            "The application must complete all previous steps before submission.");
    }

    // =========================================================
    // Validate Applicant Information
    // =========================================================

    if (application.Applicant == null)
    {
        throw new InvalidOperationException(
            "Applicant information is missing.");
    }

    if (string.IsNullOrWhiteSpace(application.Applicant.DisplayName))
    {
        throw new InvalidOperationException(
            "Full name is required.");
    }

    if (string.IsNullOrWhiteSpace(application.Applicant.NationalId))
    {
        throw new InvalidOperationException(
            "National ID is required.");
    }

    if (application.Applicant.User == null ||
        string.IsNullOrWhiteSpace(application.Applicant.User.Email))
    {
        throw new InvalidOperationException(
            "Email is required.");
    }

    if (string.IsNullOrWhiteSpace(application.Applicant.Address))
    {
        throw new InvalidOperationException(
            "Proof of residence is required.");
    }

    if (string.IsNullOrWhiteSpace(application.Applicant.PhoneNumber))
    {
        throw new InvalidOperationException(
            "Phone number is required.");
    }

    // =========================================================
    // Validate Certificate Information
    // =========================================================

    if (application.Country == null)
    {
        throw new InvalidOperationException(
            "Country is required.");
    }

    if (application.Institution == null)
    {
        throw new InvalidOperationException(
            "Institution is required.");
    }

    if (application.Major == null)
    {
        throw new InvalidOperationException(
            "Major is required.");
    }

    if (!application.GraduationYear.HasValue)
    {
        throw new InvalidOperationException(
            "Graduation year is required.");
    }

    // =========================================================
    // Validate Required Documents
    // =========================================================

    var requiredDocumentTypes = new[]
    {
        DocumentType.PersonalPhoto,
        DocumentType.PassportOrApprovedId,
        DocumentType.BachelorCertificate,
        DocumentType.BachelorTranscript
    };

    var uploadedDocumentTypes = application.Documents
        .Select(d => d.DocumentType)
        .ToHashSet();

    var missingDocuments = requiredDocumentTypes
        .Where(type => !uploadedDocumentTypes.Contains(type))
        .ToList();

    if (missingDocuments.Count > 0)
    {
        throw new InvalidOperationException(
            "All required documents must be uploaded before submitting the application.");
    }

    // =========================================================
    // Submit Application
    // =========================================================

    application.Status = "Submitted";

    application.SubmittedAt = DateTime.UtcNow;

    application.CurrentStep = 6;

    await _context.SaveChangesAsync();

    return new SubmitApplicationResponseDto
    {
        ApplicationId = application.Id,

        Status = application.Status,

        CurrentStep = application.CurrentStep,

        SubmittedAt = application.SubmittedAt.Value
    };
}
}