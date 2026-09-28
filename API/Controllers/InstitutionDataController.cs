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
    private readonly AppDbContext _context;

    public InstitutionDataController(AppDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET: api/InstitutionData/countries
    // =========================================================

    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var countries = await _context.EquivalencyCountries
            .AsNoTracking()
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
    // GET: api/InstitutionData/institutions?countryId=1
    // =========================================================

    [HttpGet("institutions")]
    public async Task<IActionResult> GetInstitutions(
        [FromQuery] short countryId)
    {
        var institutions = await _context.EquivalencyInstitutions
            .AsNoTracking()
            .Where(i => i.CountryId == countryId)
            .Select(i => new
            {
                id = i.Id,
                name = i.Name,
                nameEn = i.NameEn,
                countryId = i.CountryId,
                isActive = i.IsActive
            })
            .ToListAsync();

        return Ok(institutions);
    }


    // =========================================================
    // GET: api/InstitutionData/majors?institutionId=1
    // =========================================================

    [HttpGet("majors")]
    public async Task<IActionResult> GetMajors(
        [FromQuery] int institutionId)
    {
        var majors = await _context.EquivalencyMajors
            .AsNoTracking()
            .Where(m => m.InstitutionId == institutionId)
            .Select(m => new
            {
                id = m.Id,
                name = m.Name,
                nameEn = m.NameEn,
                institutionId = m.InstitutionId,
                isActive = m.IsActive
            })
            .ToListAsync();

        return Ok(majors);
    }
}