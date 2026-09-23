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
    IMailService mailService,
    AppDbContext context
) : BaseApiController
{
    [HttpPost("register")]// api/accounts/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

            var verifiedOtp =
            await context.OtpVerifications
                .Where(x =>
                    x.Email == registerDto.Email &&
                    x.Purpose == "Register" &&
                    x.IsVerified
                )
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

        if (verifiedOtp == null)
        {
            return BadRequest(new
            {
                message = "Please verify your email before registration."
            });
        }



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


            // بعد نجاح إنشاء الحساب، نحذف OTP الخاص بالتسجيل
            // حتى ما يضل قابل لإعادة الاستخدام
            context.OtpVerifications.Remove(verifiedOtp);

            await context.SaveChangesAsync();


        return await user.ToDto(tokenService);
    }


        
    [HttpPost("login")] // api/accounts/login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        // نبحث عن المستخدم حسب الإيميل
        var user =
            await userManager.FindByEmailAsync(
                loginDto.Email
            );

        if (user == null)
        {
            return Unauthorized("Invalid email address");
        }

        // نتأكد من كلمة المرور
        var passwordIsCorrect =
            await userManager.CheckPasswordAsync(
                user,
                loginDto.Password
            );

        if (!passwordIsCorrect)
        {
            return Unauthorized("Invalid password");
        }

        // إذا المستخدم مفعّل المصادقة الثنائية
        if (user.TwoFactorEnabled)
        {
            // نحذف أي OTP قديم خاص بتسجيل الدخول
            var oldLoginOtps =
                await context.OtpVerifications
                    .Where(x =>
                        x.Email == user.Email &&
                        x.Purpose == "Login2FA"
                    )
                    .ToListAsync();

            if (oldLoginOtps.Count > 0)
            {
                context.OtpVerifications.RemoveRange(oldLoginOtps);
            }

            // توليد OTP جديد
            var otp = GenerateOtp();

            // تخزين OTP بجدول OtpVerifications
            var otpVerification = new OtpVerification
            {
                Email = user.Email!,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Purpose = "Login2FA",
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            context.OtpVerifications.Add(otpVerification);

            await context.SaveChangesAsync();

            // تجهيز رسالة الإيميل
            var mailData = new MailData(
                new List<string>
                {
                    user.Email!
                },
                "Your Login Verification Code",
                $"""
                <h2>Two-Factor Authentication</h2>

                <p>Your login verification code is:</p>

                <h1>{otp}</h1>

                <p>This code will expire in 5 minutes.</p>
                """
            );

            // إرسال OTP على الإيميل
            var emailSent =
                await mailService.SendAsync(
                    mailData,
                    HttpContext.RequestAborted
                );

            if (!emailSent)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "Failed to send login verification code."
                    }
                );
            }

            // مهم:
            // هون ما بنرجع JWT ولا Refresh Token
            // لأنه المستخدم لسا لازم يتحقق من OTP
            return Ok(new
            {
                requiresTwoFactor = true,
                email = user.Email,
                message = "A verification code has been sent to your email."
            });
        }

        // إذا 2FA مش مفعّل، تسجيل الدخول يكمل طبيعي
        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService);
    }



        // هذا الـ endpoint يتحقق من OTP الخاص بالمصادقة الثنائية وقت تسجيل الدخول
    [HttpPost("verify-login-otp")]
    public async Task<ActionResult<UserDto>> VerifyLoginOtp(
        VerifyOtpDto verifyOtpDto
    )
    {
        // نبحث عن المستخدم حسب الإيميل
        var user =
            await userManager.FindByEmailAsync(
                verifyOtpDto.Email
            );

        if (user == null)
        {
            return Unauthorized("Invalid verification request.");
        }

        // نجيب آخر OTP خاص بتسجيل الدخول لهذا الإيميل
        var otpRecord =
            await context.OtpVerifications
                .Where(x =>
                    x.Email == verifyOtpDto.Email &&
                    x.Purpose == "Login2FA"
                )
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

        // إذا ما في OTP محفوظ
        if (otpRecord == null)
        {
            return BadRequest(new
            {
                message = "No login OTP request was found."
            });
        }

        // إذا انتهت صلاحية الكود
        if (otpRecord.ExpiresAt < DateTime.UtcNow)
        {
            return BadRequest(new
            {
                message = "OTP has expired."
            });
        }

        // إذا الكود المدخل غلط
        if (otpRecord.OtpCode != verifyOtpDto.Otp)
        {
            return BadRequest(new
            {
                message = "Invalid OTP."
            });
        }

        // بعد نجاح التحقق نحذف الـ OTP
        // لأنه لازم يُستخدم مرة واحدة فقط
        context.OtpVerifications.Remove(otpRecord);

        await context.SaveChangesAsync();

        // هسا فقط نعتبر تسجيل الدخول ناجح
        // وننشئ Refresh Token
        await SetRefreshTokenCookie(user);

        // ونرجع بيانات المستخدم مع JWT
        return await user.ToDto(tokenService);
    }




    // هذه الدالة تولّد OTP مكوّن من 6 أرقام
    private static string GenerateOtp()
    {
        // يولّد رقم عشوائي آمن بين 100000 و 999999
        var otp = RandomNumberGenerator.GetInt32(100000, 1000000);

        return otp.ToString();
    }





    // هذا الـ endpoint يولّد OTP ويرسله على الإيميل
    // ويستخدم جدول OtpVerifications بدل AppUser
    [HttpPost("send-otp")]
    public async Task<ActionResult> SendOtp(SendOtpDto sendOtpDto)
    {
        // نتأكد إن الإيميل مش فاضي
        if (string.IsNullOrWhiteSpace(sendOtpDto.Email))
        {
            return BadRequest(new
            {
                message = "Email is required."
            });
        }

        // توليد OTP مكوّن من 6 أرقام
        var otp = GenerateOtp();

        // نحذف أي OTP قديم لنفس الإيميل
        // حتى ما يصير عنده أكثر من كود فعال بنفس الوقت
        var oldOtps = await context.OtpVerifications
            .Where(x => x.Email == sendOtpDto.Email)
            .ToListAsync();

        if (oldOtps.Count > 0)
        {
            context.OtpVerifications.RemoveRange(oldOtps);
        }

        // إنشاء سجل OTP جديد
        var otpVerification = new OtpVerification
        {
            Email = sendOtpDto.Email,
            OtpCode = otp,

            // صلاحية الكود 5 دقائق
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),

            // حاليًا نعتبره للتسجيل
            Purpose = "Register",

            IsVerified = false,

            CreatedAt = DateTime.UtcNow
        };

        // تخزين OTP في قاعدة البيانات
        context.OtpVerifications.Add(otpVerification);

        await context.SaveChangesAsync();

        // تجهيز الإيميل
        var mailData = new MailData(
            new List<string>
            {
                sendOtpDto.Email
            },
            "Your OTP Code",
            $"""
            <h2>Email Verification</h2>

            <p>Your OTP code is:</p>

            <h1>{otp}</h1>

            <p>This code will expire in 5 minutes.</p>
            """
        );

        // إرسال OTP على الإيميل
        var emailSent = await mailService.SendAsync(
            mailData,
            HttpContext.RequestAborted
        );

        // إذا فشل إرسال الإيميل
        if (!emailSent)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "Failed to send OTP."
                }
            );
        }

        return Ok(new
        {
            message = "OTP has been sent successfully."
        });
    }






// هذا الـ endpoint يتحقق من OTP الخاص بالإيميل
[HttpPost("verify-otp")]
public async Task<ActionResult> VerifyOtp(
    VerifyOtpDto verifyOtpDto
)
{
    // نبحث عن آخر OTP لنفس الإيميل
    // والمستخدم للتسجيل Purpose = Register
    var otpRecord =
        await context.OtpVerifications
            .Where(x =>
                x.Email == verifyOtpDto.Email &&
                x.Purpose == "Register"
            )
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    // إذا ما في OTP محفوظ
    if (otpRecord == null)
    {
        return BadRequest(new
        {
            message = "No OTP request was found."
        });
    }

    // نتأكد إن الكود لسا ما انتهت صلاحيته
    if (otpRecord.ExpiresAt < DateTime.UtcNow)
    {
        return BadRequest(new
        {
            message = "OTP has expired."
        });
    }

    // نقارن الكود المدخل مع الكود المخزن
    if (otpRecord.OtpCode != verifyOtpDto.Otp)
    {
        return BadRequest(new
        {
            message = "Invalid OTP."
        });
    }

    // إذا الكود صحيح، نعتبر الإيميل متحقق منه
    otpRecord.IsVerified = true;

    await context.SaveChangesAsync();

    return Ok(new
    {
        message = "Email verified successfully."
    });
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




    // هذا الـ endpoint ينفذ إعادة تعيين كلمة المرور فعليًا
    // يستقبل الإيميل + التوكن + كلمة المرور الجديدة
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(
        ResetPasswordDto resetPasswordDto
    )
    {
        // نبحث عن المستخدم حسب الإيميل
        var user =
            await userManager.FindByEmailAsync(
                resetPasswordDto.Email
            );

        // إذا المستخدم غير موجود
        if (user == null)
        {
            return BadRequest(new
            {
                message = "Invalid password reset request."
            });
        }

        // ASP.NET Identity يتحقق من التوكن
        // وإذا كان صحيح يغيّر كلمة المرور
        var result =
            await userManager.ResetPasswordAsync(
                user,
                resetPasswordDto.Token,
                resetPasswordDto.NewPassword
            );

        // إذا فشلت العملية، نرجع أخطاء Identity
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "identity",
                    error.Description
                );
            }

            return ValidationProblem(ModelState);
        }

        // نمسح refresh token القديم
        // حتى الجلسات القديمة ما تضل فعالة بعد تغيير الباسورد
        user.RefreshToken = null;
        user.RefreshTokenExpirey = null;

        await userManager.UpdateAsync(user);

        return Ok(new
        {
            message = "Password has been reset successfully."
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

