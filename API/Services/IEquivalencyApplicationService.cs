using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.EquivalencyApplication;

namespace API.Services
{
    public interface IEquivalencyApplicationService
    {
        Task<EquivalencyApplicationDto> CreateDraftAsync(
            string applicantId,
            CreateEquivalencyApplicationDto dto
        );
    }
}