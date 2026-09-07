using System.Buffers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using API.DTOs;
using System.Security.Claims;
using API.Extensions;


namespace API.Controllers
{

    [Authorize]
   
    public class ApplicantsController(IApplicantRepository applicantRepository) : BaseApiController // هون لما اغير الي عند البروجرام.سي اس بغير الIApplicantReositort لَ IUnitOfWork uow وبسويلها فنكشناتها الي تحت
    {

        [HttpGet("ApplicantsList")]
        public async Task<ActionResult<IReadOnlyList<Applicant>>> GetApplicants()
        {
            return Ok(await applicantRepository.GetApplicantsAsync());
        }

        [Authorize]

        [HttpGet("{id}")]  //we use=> loclalhast:5001/api/Applicants/bob-id
        public async Task<ActionResult<Applicant>> GetApplicant(string id)
        {
            var Applicant = await applicantRepository.GetApplicantByIdAsync(id);

            if (Applicant == null) return NotFound();

            return Applicant;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetApplicantPhotos(string id)
        {
            return Ok(await applicantRepository.GetPhotoForApplicantAsync(id));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateApplicant(ApplicantUpdateDto applicantUpdateDto)
        {
            var ApplicantId = User.GetApplicantId();

            var Applicant = await applicantRepository.GetApplicantForUpdate(ApplicantId);

            if (Applicant == null) return BadRequest("Could not get Applicant");

            Applicant.DisplayName = applicantUpdateDto.DisplayName ?? Applicant.DisplayName;
            // Applicant.Discription = ApplicantUpdateDto.Discription ?? Applicant.Discription;
            // Applicant.City = ApplicantUpdateDto.City ?? Applicant.City;
            // Applicant.Country = ApplicantUpdateDto.Country ?? Applicant.Country;
            if (Applicant.User != null)
            {
                Applicant.User.DisplayName = applicantUpdateDto.DisplayName ?? Applicant.User.DisplayName;
            }
            applicantRepository.Update(Applicant); //optional

            if (await applicantRepository.SaveAllAsync()) return NoContent();//the {save allchanges} here رح تلتغى ويصير بدالها uow.Complete();

            return BadRequest("Faild to update Applicant");
        }
    }
}
