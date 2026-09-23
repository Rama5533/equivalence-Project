using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using API.DTOs.EquivalencyApplication;
// using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

//POST /api/EquivalencyApplications

public class EquivalencyApplicationsController : ControllerBase
{
    private readonly IEquivalencyApplicationService _service;

    public EquivalencyApplicationsController(
        IEquivalencyApplicationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<EquivalencyApplicationDto>> CreateDraft(
        CreateEquivalencyApplicationDto dto)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
}
