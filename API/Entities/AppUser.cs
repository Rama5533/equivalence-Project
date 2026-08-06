using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Entities;

public class AppUser
{
    //the ID -->represent the rows
    public  string Id { get; set; }=Guid.NewGuid().ToString();

//these are represent the columns
    public required string DisplayName { get; set; } 
    public required string Email { get; set; }

    public string? ImageUrl { get; set; }

    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }

    internal ActionResult<UserDto> ToDto()
    {
        throw new NotImplementedException();
    }
//Nav property
public Member Member { get; set; }=null!;
}