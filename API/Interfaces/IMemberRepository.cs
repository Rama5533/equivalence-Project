using System;
using API.Entities;

namespace API.Interfaces;
    public interface IMemberRepository
    {
        void Update(Member member);
        Task<bool> SaveAllAsync();// حاليا بتضل لما اغير الprogram.cs بتلتغي

        Task<IReadOnlyList<Member>> GetMembersAsync();
        Task<Member?> GetMemberByIdAsync(string id);
        Task<IReadOnlyList<Photo>> GetPhotoForMemberAsync(string memberId);

        Task<Member?>GetMemberForUpdate(string id);
    }
