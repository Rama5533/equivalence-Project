using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs.EquivalencyApplication;
using API.Enums;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EquivalencyApplicationsController : ControllerBase
{
    private readonly IEquivalencyApplicationService _service;

    public EquivalencyApplicationsController(
        IEquivalencyApplicationService service)
    {
        _service = service;
    }

    // =========================================================
    // STEP 1
    // POST: /api/EquivalencyApplications
    // =========================================================

    [HttpPost]
    public async Task<ActionResult<EquivalencyApplicationDto>> CreateDraft(
        CreateEquivalencyApplicationDto dto)
    {
        var applicantId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(applicantId))
        {
            return Unauthorized();
        }

        if (!Enum.IsDefined(dto.QualificationType))
        {
            return BadRequest("Invalid qualification type.");
        }

        try
        {
            var result = await _service.CreateDraftAsync(
                applicantId,
                dto);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }


    // =========================================================
    // STEP 2
    // PUT: /api/EquivalencyApplications/{applicationId}/step2
    // =========================================================

    [HttpPut("{applicationId}/step2")]
    public async Task<ActionResult<Step2ApplicantDto>> CompleteStep2(
        int applicationId)
    {
        var applicantId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(applicantId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _service.CompleteStep2Async(
                applicantId,
                applicationId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // =========================================================
    // STEP 3
    // PUT: /api/EquivalencyApplications/{applicationId}/step3
    // =========================================================

    [HttpPut("{applicationId}/step3")]
    public async Task<ActionResult<Step3CertificateResponseDto>> CompleteStep3(
        int applicationId,
        Step3CertificateDto dto)
    {
        var applicantId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(applicantId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _service.CompleteStep3Async(
                applicantId,
                applicationId,
                dto);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // =========================================================
    // STEP 4 - GET REQUIRED DOCUMENTS
    // GET: /api/EquivalencyApplications/{applicationId}/step4/documents
    // =========================================================

    [HttpGet("{applicationId}/step4/documents")]
    public async Task<ActionResult<List<RequiredDocumentDto>>> GetRequiredDocuments(
        int applicationId)
    {
        var applicantId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(applicantId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _service.GetRequiredDocumentsAsync(
                applicantId,
                applicationId);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // =========================================================
    // STEP 4 - UPLOAD DOCUMENT
    // POST: /api/EquivalencyApplications/{applicationId}/step4/documents
    // =========================================================

    [HttpPost("{applicationId}/step4/documents")]
    public async Task<ActionResult<UploadDocumentResponseDto>> UploadDocument(
        int applicationId,
        [FromForm] DocumentType documentType,
        [FromForm] IFormFile file)
    {
        var applicantId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(applicantId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _service.UploadDocumentAsync(
                applicantId,
                applicationId,
                documentType,
                file);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // =========================================================
    // STEP 4 - COMPLETE
    // PUT: /api/EquivalencyApplications/{applicationId}/step4
    // =========================================================

    [HttpPut("{applicationId}/step4")]
    public async Task<IActionResult> CompleteStep4(
        int applicationId)
    {
        var applicantId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(applicantId))
        {
            return Unauthorized();
        }

        try
        {
            await _service.CompleteStep4Async(
                applicantId,
                applicationId);

            return Ok(new
            {
                applicationId,
                currentStep = 4
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


// =========================================================
// STEP 5 - REVIEW APPLICATION
// GET: /api/EquivalencyApplications/{applicationId}/review
// =========================================================

    [HttpGet("{applicationId}/review")]
public async Task<ActionResult<EquivalencyApplicationReviewDto>> GetReview(
    int applicationId)
{
    var applicantId = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(applicantId))
    {
        return Unauthorized();
    }

    try
    {
        var result = await _service.GetReviewAsync(
            applicantId,
            applicationId);

        return Ok(result);
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ex.Message);
    }
}


// =========================================================
// STEP 6 - SUBMIT APPLICATION
// PUT: /api/EquivalencyApplications/{applicationId}/submit
// =========================================================

[HttpPut("{applicationId}/submit")]
public async Task<ActionResult<SubmitApplicationResponseDto>> SubmitApplication(
    int applicationId)
{
    var applicantId = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(applicantId))
    {
        return Unauthorized();
    }

    try
    {
        var result = await _service.SubmitAsync(
            applicantId,
            applicationId);

        return Ok(result);
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ex.Message);
    }
}
}