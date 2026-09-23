using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.EquivalencyApplication;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace API.Services;
    public class EquivalencyApplicationService:IEquivalencyApplicationService
    {
        private readonly AppDbContext _context;
        public EquivalencyApplicationService (AppDbContext context)
    {
        _context=context;
    }

public async Task<EquivalencyApplicationDto> CreateDraftAsync(
    string applicantId,
    CreateEquivalencyApplicationDto dto
)
    {
        var applicantExists=await _context.Applicants
        .AnyAsync(a=>a.Id==applicantId);

        if(!applicantExists){
            throw new KeyNotFoundException("Applicant not found.");
        }

                var application = new EquivalencyApplication
        {
            ApplicantId = applicantId,
            QualificationType = dto.QualificationType,
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        };

        _context.EquivalencyApplications.Add(application);

        await _context.SaveChangesAsync();

        return new EquivalencyApplicationDto
        {
            Id = application.Id,
            QualificationType = application.QualificationType,
            Status = application.Status,
            CreatedAt = application.CreatedAt,
            SubmittedAt = application.SubmittedAt
        };
    }


    }
