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
   
    public class MembersController(IMemberRepository memberRepository) : BaseApiController // هون لما اغير الي عند البروجرام.سي اس بغير الImemberReositort لَ IUnitOfWork uow وبسويلها فنكشناتها الي تحت
    {

        [HttpGet("MembersList")]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            return Ok(await memberRepository.GetMembersAsync());
        }

        [Authorize]

        [HttpGet("{id}")]  //we use=> loclalhast:5001/api/members/bob-id
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await memberRepository.GetMemberByIdAsync(id);

            if (member == null) return NotFound();

            return member;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            return Ok(await memberRepository.GetPhotoForMemberAsync(id));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var memberId = User.GetMemberId();

            var member = await memberRepository.GetMemberForUpdate(memberId);

            if (member == null) return BadRequest("Could not get member");

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            // member.Discription = memberUpdateDto.Discription ?? member.Discription;
            // member.City = memberUpdateDto.City ?? member.City;
            // member.Country = memberUpdateDto.Country ?? member.Country;
            if (member.User != null)
            {
                member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;
            }
            memberRepository.Update(member); //optional

            if (await memberRepository.SaveAllAsync()) return NoContent();//the save allchanges here رح تلتغى ويصير بدالها uow.Complete();

            return BadRequest("Faild to update member");
        }
    }
}
