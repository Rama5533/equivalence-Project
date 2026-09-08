using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ApplicantsController(
        IApplicantRepository applicantRepository
    ) : BaseApiController
    {
        // GET: api/Applicants/ApplicantsList
        [HttpGet("ApplicantsList")]
        public async Task<ActionResult<IReadOnlyList<Applicant>>> GetApplicants()
        {
            return Ok(
                await applicantRepository.GetApplicantsAsync()
            );
        }


        // GET: api/Applicants/profile
        // يرجع بروفايل المستخدم المسجل دخوله حالياً
        [HttpGet("profile")]
        public async Task<ActionResult> GetProfile()
        {
            var applicantId = User.GetApplicantId();

            var applicant =
                await applicantRepository
                    .GetApplicantForUpdate(applicantId);

            if (applicant == null)
            {
                return NotFound("Applicant not found");
            }

            return Ok(new
            {
                applicant.Id,

                // الاسم الأساسي يأتي من بيانات التسجيل
                DisplayName =
                    applicant.User?.DisplayName
                    ?? applicant.DisplayName,

                // البريد الأساسي يأتي من AppUser
                Email =
                    applicant.User?.Email,

                applicant.NationalId,

                applicant.IdentityType,

                applicant.Gender,

                applicant.DateOfBirth,

                applicant.Nationality,

                applicant.PhoneNumber,

                applicant.WhatsAppNumber,

                applicant.AlternativeEmail,

                applicant.Country,

                applicant.City,

                applicant.Address,

                // الصورة الشخصية
                applicant.ImageUrl,

                // صورة الهوية
                applicant.IdImageUrl
            });
        }


        // PUT: api/Applicants/profile
        // تعديل بيانات الملف الشخصي
        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile(
            ApplicantProfileUpdateDto dto
        )
        {
            var applicantId = User.GetApplicantId();

            var applicant =
                await applicantRepository
                    .GetApplicantForUpdate(applicantId);

            if (applicant == null)
            {
                return NotFound("Applicant not found");
            }


            // تعديل الاسم الكامل
            if (!string.IsNullOrWhiteSpace(dto.DisplayName))
            {
                applicant.DisplayName =
                    dto.DisplayName.Trim();

                // الاسم موجود أيضاً في AppUser
                if (applicant.User != null)
                {
                    applicant.User.DisplayName =
                        dto.DisplayName.Trim();
                }
            }


            // المعلومات الشخصية
            applicant.NationalId =
                dto.NationalId;

            applicant.IdentityType =
                dto.IdentityType;

            applicant.Gender =
                dto.Gender;

            applicant.DateOfBirth =
                dto.DateOfBirth;

            applicant.Nationality =
                dto.Nationality;


            // معلومات التواصل
            applicant.PhoneNumber =
                dto.PhoneNumber;

            applicant.WhatsAppNumber =
                dto.WhatsAppNumber;

            applicant.AlternativeEmail =
                dto.AlternativeEmail;


            // العنوان
            applicant.Country =
                dto.Country;

            applicant.City =
                dto.City;

            applicant.Address =
                dto.Address;


            applicantRepository.Update(applicant);

            var saved =
                await applicantRepository
                    .SaveAllAsync();

            if (!saved)
            {
                return BadRequest(
                    "Failed to update profile"
                );
            }

            return Ok(new
            {
                message =
                    "Profile updated successfully"
            });
        }




        [HttpPost("profile/photo")]
        public async Task<ActionResult> UploadProfilePhoto(
            IFormFile file
        )
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var applicantId = User.GetApplicantId();

            var applicant =
                await applicantRepository
                    .GetApplicantForUpdate(applicantId);

            if (applicant == null)
            {
                return NotFound("Applicant not found");
            }

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "profiles"
            );

            Directory.CreateDirectory(uploadsFolder);

            var extension =
                Path.GetExtension(file.FileName);

            var fileName =
                $"{applicantId}_{Guid.NewGuid()}{extension}";

            var filePath =
                Path.Combine(
                    uploadsFolder,
                    fileName
                );

            using (var stream =
                new FileStream(
                    filePath,
                    FileMode.Create
                ))
            {
                await file.CopyToAsync(stream);
            }

            applicant.ImageUrl =
                $"/uploads/profiles/{fileName}";

            applicantRepository.Update(applicant);

            var saved =
                await applicantRepository
                    .SaveAllAsync();

            if (!saved)
            {
                return BadRequest(
                    "Failed to save profile photo"
                );
            }

            return Ok(new
            {
                imageUrl = applicant.ImageUrl
            });
        }



        [HttpPost("profile/id-image")]
        public async Task<ActionResult> UploadIdImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var applicantId = User.GetApplicantId();

            var applicant =
                await applicantRepository.GetApplicantForUpdate(applicantId);

            if (applicant == null)
            {
                return NotFound("Applicant not found");
            }

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "id-images"
            );

            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(file.FileName);

            var fileName =
                $"{applicantId}_{Guid.NewGuid()}{extension}";

            var filePath =
                Path.Combine(uploadsFolder, fileName);

            using (var stream =
                new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            applicant.IdImageUrl =
                $"/uploads/id-images/{fileName}";

            applicantRepository.Update(applicant);

            var saved =
                await applicantRepository.SaveAllAsync();

            if (!saved)
            {
                return BadRequest("Failed to save ID image");
            }

            return Ok(new
            {
                idImageUrl = applicant.IdImageUrl
            });
        }



        // GET: api/Applicants/{id}/photos
        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetApplicantPhotos(
            string id
        )
        {
            return Ok(
                await applicantRepository
                    .GetPhotoForApplicantAsync(id)
            );
        }


        // GET: api/Applicants/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Applicant>> GetApplicant(
            string id
        )
        {
            var applicant =
                await applicantRepository
                    .GetApplicantByIdAsync(id);

            if (applicant == null)
            {
                return NotFound();
            }

            return applicant;
        }


        // PUT: api/Applicants
        // Endpoint القديم الموجود في المشروع
        [HttpPut]
        public async Task<ActionResult> UpdateApplicant(
            ApplicantUpdateDto applicantUpdateDto
        )
        {
            var applicantId =
                User.GetApplicantId();

            var applicant =
                await applicantRepository
                    .GetApplicantForUpdate(applicantId);

            if (applicant == null)
            {
                return BadRequest(
                    "Could not get Applicant"
                );
            }

            applicant.DisplayName =
                applicantUpdateDto.DisplayName
                ?? applicant.DisplayName;

            if (applicant.User != null)
            {
                applicant.User.DisplayName =
                    applicantUpdateDto.DisplayName
                    ?? applicant.User.DisplayName;
            }

            applicantRepository.Update(applicant);

            if (
                await applicantRepository
                    .SaveAllAsync()
            )
            {
                return NoContent();
            }

            return BadRequest(
                "Failed to update Applicant"
            );
        }
    }
}