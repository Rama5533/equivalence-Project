using API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Entities;

public class AppUser : IdentityUser
{
    //the ID -->represent the rows

    //these are represent the columns
    public required string DisplayName { get; set; }

    public string? ImageUrl { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpirey { get; set; }


    /*  internal ActionResult<UserDto> ToDto()
      {
          throw new NotImplementedException();
      }*/
    //Nav property
    public Member Member { get; set; } = null!;
}