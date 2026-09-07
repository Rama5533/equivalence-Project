using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicantRepository(AppDbContext context) : IApplicantRepository
{
    public async Task<Applicant?> GetApplicantByIdAsync(string id)
    {
        return await context.Applicants.FindAsync(id);
    }

    public async Task<Applicant?> GetApplicantForUpdate(string id)
    {
        return await context.Applicants
        .Include(x => x.User)
        .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<Applicant>> GetApplicantsAsync()
    {
        return await context.Applicants.ToListAsync();
    }

    public async Task<IReadOnlyList<Photo>> GetPhotoForApplicantAsync(string ApplicantId)
    {
        return await context.Applicants.Where(x => x.Id == ApplicantId)
        .SelectMany(x => x.Photos)
        .ToListAsync();
    }


    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    } //وهاد نفس الاشي بتلغى لما اغير الprogram.cs

    public void Update(Applicant Applicant)
    {
        context.Entry(Applicant).State = EntityState.Modified;
    }

}
