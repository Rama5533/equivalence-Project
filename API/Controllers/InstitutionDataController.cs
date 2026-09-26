using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstitutionDataController : ControllerBase
{
    private readonly InstitutionDbContext _context;

    public InstitutionDataController(InstitutionDbContext context)
    {
        _context = context;
    }

    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var countries = await _context.Countries
            .AsNoTracking()
            .Select(c => new
            {
                id = c.ID,
                name = c.Name,
                nameEn = c.NameEn
            })
            .ToListAsync();

        return Ok(countries);
    }

    [HttpGet("institutions")]
    public async Task<IActionResult> GetInstitutions(
        [FromQuery] short countryId)
    {
        var institutions = await _context.Institutions
            .AsNoTracking()
            .Where(i => i.CountryID == countryId)
            .Select(i => new
            {
                id = i.InstituteID,
                name = i.InstituteName,
                nameEn = i.InstituteNameEn,
                countryId = i.CountryID,
                isActive = i.IsActive
            })
            .ToListAsync();

        return Ok(institutions);
    }
        [HttpGet("faculties")]
    public async Task<IActionResult> GetFaculties(
        [FromQuery] short institutionId)
    {
        var faculties = await _context.Faculties
            .AsNoTracking()
            .Where(f => f.InstituteID == institutionId)
            .Select(f => new
            {
                id = f.FacultyID,
                name = f.FacultyName,
                nameEn = f.FacultyNameEn
            })
            .ToListAsync();

        return Ok(faculties);
    }
}