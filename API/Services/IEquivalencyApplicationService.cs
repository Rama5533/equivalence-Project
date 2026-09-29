using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.EquivalencyApplication;
using API.Enums;
using Microsoft.AspNetCore.Http;

namespace API.Services;

public interface IEquivalencyApplicationService
{
    Task<EquivalencyApplicationDto> CreateDraftAsync(
        string applicantId,
        CreateEquivalencyApplicationDto dto
    );

    Task<Step2ApplicantDto> CompleteStep2Async(
        string applicantId,
        int applicationId
    );

    Task<Step3CertificateResponseDto> CompleteStep3Async(
        string applicantId,
        int applicationId,
        Step3CertificateDto dto
    );

    Task<List<RequiredDocumentDto>> GetRequiredDocumentsAsync(
        string applicantId,
        int applicationId
    );

    Task<UploadDocumentResponseDto> UploadDocumentAsync(
        string applicantId,
        int applicationId,
        DocumentType documentType,
        IFormFile file
    );

    Task CompleteStep4Async(
        string applicantId,
        int applicationId
    );

    Task<EquivalencyApplicationReviewDto> GetReviewAsync(
    string applicantId,
    int applicationId
);
    Task<SubmitApplicationResponseDto> SubmitAsync(
        string applicantId,
        int applicationId
    );

}