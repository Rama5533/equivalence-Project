using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstitutionDataController : ControllerBase
{
    private readonly AppDbContext _context;

    public InstitutionDataController(
        AppDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET: api/InstitutionData/countries
    // =========================================================

    [HttpGet("countries")]
    public async Task<IActionResult>
        GetCountries()
    {
        var countries =
            await _context
                .EquivalencyCountries
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    nameEn = c.NameEn
                })
                .ToListAsync();

        return Ok(countries);
    }


    // =========================================================
    // GET:
    // api/InstitutionData/institutions
    // ?countryId=1
    // &qualificationType=secondary
    // =========================================================

    [HttpGet("institutions")]
    public async Task<IActionResult>
        GetInstitutions(
            [FromQuery] short countryId,
            [FromQuery] string qualificationType)
    {
        if (countryId <= 0)
        {
            return BadRequest(new
            {
                message =
                    "countryId is required."
            });
        }

        if (string.IsNullOrWhiteSpace(
                qualificationType))
        {
            return BadRequest(new
            {
                message =
                    "qualificationType is required."
            });
        }

        var isSecondary =
            qualificationType.Equals(
                "secondary",
                StringComparison.OrdinalIgnoreCase
            );

        var requiredKind =
            isSecondary
                ? InstitutionKind.SecondarySchool
                : InstitutionKind.HigherEducation;

        var institutions =
            await _context
                .EquivalencyInstitutions
                .AsNoTracking()
                .Where(i =>
                    i.CountryId == countryId &&
                    i.IsActive == true &&
                    i.Kind == requiredKind
                )
                .OrderBy(i => i.Name)
                .Select(i => new
                {
                    id = i.Id,
                    name = i.Name,
                    nameEn = i.NameEn,
                    countryId = i.CountryId,
                    isActive = i.IsActive,
                    kind = i.Kind.ToString()
                })
                .ToListAsync();

        return Ok(institutions);
    }


    // =========================================================
    // GET:
    // api/InstitutionData/majors
    // ?institutionId=1
    // &qualificationType=secondary
    //
    // secondary:
    //   Scientific / Literary / Industrial ...
    //
    // university:
    //   Computer Science / Engineering ...
    // =========================================================

    [HttpGet("majors")]
    public async Task<IActionResult>
        GetMajors(
            [FromQuery] int institutionId,
            [FromQuery] string qualificationType)
    {
        if (institutionId <= 0)
        {
            return BadRequest(new
            {
                message =
                    "institutionId is required."
            });
        }

        if (string.IsNullOrWhiteSpace(
                qualificationType))
        {
            return BadRequest(new
            {
                message =
                    "qualificationType is required."
            });
        }

        var isSecondary =
            qualificationType.Equals(
                "secondary",
                StringComparison.OrdinalIgnoreCase
            );

        var requiredInstitutionKind =
            isSecondary
                ? InstitutionKind.SecondarySchool
                : InstitutionKind.HigherEducation;

        var requiredMajorKind =
            isSecondary
                ? MajorKind.SecondaryBranch
                : MajorKind.UniversityMajor;


        // نتأكد أن المؤسسة موجودة ونوعها صحيح
        var institution =
            await _context
                .EquivalencyInstitutions
                .AsNoTracking()
                .FirstOrDefaultAsync(i =>
                    i.Id == institutionId &&
                    i.IsActive == true &&
                    i.Kind ==
                        requiredInstitutionKind
                );

        if (institution == null)
        {
            return NotFound(new
            {
                message =
                    "Institution was not found or does not match the selected qualification type."
            });
        }


        var majors =
            await _context
                .EquivalencyMajors
                .AsNoTracking()
                .Where(m =>
                    m.InstitutionId ==
                        institutionId &&
                    m.IsActive == true &&
                    m.Kind == requiredMajorKind
                )
                .OrderBy(m => m.Name)
                .Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    nameEn = m.NameEn,
                    institutionId =
                        m.InstitutionId,
                    isActive = m.IsActive,
                    kind = m.Kind.ToString()
                })
                .ToListAsync();

        return Ok(majors);
    }
}