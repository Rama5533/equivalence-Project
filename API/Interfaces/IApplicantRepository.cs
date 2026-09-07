using System;
using API.Entities;

namespace API.Interfaces;
    public interface IApplicantRepository
    {
        void Update(Applicant applicant);
        Task<bool> SaveAllAsync();// حاليا بتضل لما اغير الprogram.cs بتلتغي

        Task<IReadOnlyList<Applicant>> GetApplicantsAsync();
        Task<Applicant?> GetApplicantByIdAsync(string id);
        Task<IReadOnlyList<Photo>> GetPhotoForApplicantAsync(string ApplicantId);

        Task<Applicant?>GetApplicantForUpdate(string id);
    }
