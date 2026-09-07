using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UnitOfWorks(AppDbContext context) : IUnitOfWork
{
    private IApplicantRepository? _applicantRepository;
    public IApplicantRepository ApplicantRepository => _applicantRepository
        ??= new ApplicantRepository(context);

    public async Task<bool> Complete()
    {
        try
        {
            return await context.SaveChangesAsync() > 0;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occured while saving changes", ex);
        }
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
