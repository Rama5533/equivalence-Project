using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using Microsoft.AspNetCore.Identity;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var applicantData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var applicants = JsonSerializer.Deserialize<List<SeedUserDto>>(applicantData);

        if (applicants == null)
        {
            Console.WriteLine("No applicants in seed data");
            return;
        }


        foreach (var applicant in applicants)
        {

            var user = new AppUser
            {
                Id = applicant.Id,
                Email = applicant.Email,
                DisplayName = applicant.DisplayName,
                UserName = applicant.Email,
                ImageUrl = applicant.ImageUrl,
                Applicant = new Applicant
                {
                    Id = applicant.Id,
                    DisplayName = applicant.DisplayName,
                    // Discription = applicant.Discription,
                    // DateOfBirth = applicant.DateOfBirth,
                    ImageUrl = applicant.ImageUrl,
                    // Gender = applicant.Gender,
                    // City = applicant.City,
                    // Country = applicant.Country,
                    LastActive = applicant.LastActive,
                    Created = applicant.Created
                }
            };
            user.Applicant.Photos.Add(new Photo
            {
                Url = applicant.ImageUrl!,
                ApplicantId = applicant.Id
            });

            var result = await userManager.CreateAsync(user, "P$$w0rd");
            if (!result.Succeeded)
            {
                Console.WriteLine(result.Errors.First().Description);
            }
            await userManager.AddToRoleAsync(user, "applicant");
        }
        var admin = new AppUser
        {
            UserName = "admin@test.com",
            Email = "admin@test.com",
            DisplayName = "Admin"

        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, ["Admin", "Manager"]);
    }
}
