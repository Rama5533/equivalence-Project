using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using API.Services;
using API.Models;

namespace API.Controllers;


public class AccountController(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IMailService mailService
) : BaseApiController
{
    [HttpPost("register")]// api/accounts/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Email,
            Applicant = new Applicant
            {
                DisplayName = registerDto.DisplayName,
                // DateOfBirth = registerDto.DateOfBirth,
                // Country = registerDto.Country,
                // City = registerDto.City,
                // Gender = registerDto.Gender
            }
        };



        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }
            return ValidationProblem();
        }

        await userManager.AddToRoleAsync(user, "APPLICANT");

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService);
    }

    [HttpPost("login")] //  api/accounts/login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized("Invalid email address");

        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result) return Unauthorized("Invalid password");

        await SetRefreshTokenCookie(user);


        return await user.ToDto(tokenService);

    }

    [HttpPost("refresh-to-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken == null) return NoContent();

        var user = await userManager.Users
        .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken
        && x.RefreshTokenExpirey > DateTime.UtcNow);

        if(user==null) return Unauthorized();

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService);
    }



// هذا الـ endpoint يبدأ عملية نسيان كلمة المرور
// يستقبل إيميل المستخدم، يولّد Password Reset Token
// وبعدها يبعث رابط إعادة تعيين كلمة المرور على الإيميل
[HttpPost("forgot-password")]
public async Task<ActionResult> ForgotPassword(
    ForgotPasswordDto forgotPasswordDto
)
{
    // نبحث عن المستخدم حسب الإيميل
    var user =
        await userManager.FindByEmailAsync(
            forgotPasswordDto.Email
        );

    // لأسباب أمنية، ما بنحكي للمستخدم إذا الإيميل موجود أو لا
    // حتى ما نكشف الحسابات المسجلة بالنظام
    if (user == null)
    {
        return Ok(new
        {
            message =
                "If the email exists, a password reset link has been sent."
        });
    }

    // توليد توكن خاص بإعادة تعيين كلمة المرور
    // ASP.NET Identity هو المسؤول عن إنشاء التوكن والتحقق منه لاحقًا
    var resetToken =
        await userManager
            .GeneratePasswordResetTokenAsync(user);

    // بناء رابط إعادة تعيين كلمة المرور
    // الفرونت لاحقًا رح يعمل صفحة reset-password
    // وتقرأ منها email و token من الرابط
    var resetLink =
        $"http://localhost:3000/reset-password" +
        $"?email={Uri.EscapeDataString(user.Email!)}" +
        $"&token={Uri.EscapeDataString(resetToken)}";

    // تجهيز رسالة الإيميل
    var mailData =
        new MailData(
            new List<string>
            {
                user.Email!
            },
            "Reset your password",
            $"""
            <h2>Password Reset</h2>

            <p>
                We received a request to reset your password.
            </p>

            <p>
                Click the link below to choose a new password:
            </p>

            <p>
                <a href="{resetLink}">
                    Reset Password
                </a>
            </p>

            <p>
                If you did not request a password reset,
                you can ignore this email.
            </p>
            """
        );

    // إرسال الإيميل باستخدام MailService الموجود عندنا
    var emailSent =
        await mailService.SendAsync(
            mailData,
            HttpContext.RequestAborted
        );

    // إذا صار خطأ أثناء إرسال الإيميل
    if (!emailSent)
    {
        return StatusCode(
            StatusCodes.Status500InternalServerError,
            new
            {
                message =
                    "Failed to send password reset email."
            }
        );
    }

    // إذا تم الإرسال بنجاح
    // ما بنرجع resetToken بالـ response لأسباب أمنية
    return Ok(new
    {
        message =
            "If the email exists, a password reset link has been sent."
    });
}



    private async Task SetRefreshTokenCookie(AppUser user)
    {
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirey = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    //forget password
    //update user
    //delet user
}

