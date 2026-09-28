using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(
    UserManager<AppUser> userManager,
    AppDbContext context
) : BaseApiController
{
    
    // =========================================================
    // Users & Roles
    // =========================================================

    // جلب جميع المستخدمين مع أدوارهم
    [Authorize(Policy = "RequierAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
            .OrderBy(x => x.DisplayName)
            .ToListAsync();

        var userList = new List<object>();

        foreach (var user in users)
        {
            var roles =
                await userManager.GetRolesAsync(user);

            userList.Add(new
            {
                id = user.Id,

                displayName =
                    user.DisplayName,

                email =
                    user.Email,

                // شاشة الفرونت تتعامل حاليًا مع دور واحد رئيسي
                role =
                    roles.FirstOrDefault()
                    ?? "APPLICANT",

                // نخلي القائمة كاملة موجودة إذا احتجناها لاحقًا
                roles =
                    roles.ToList()
            });
        }

        return Ok(userList);
    }


    // تغيير دور مستخدم
    [Authorize(Policy = "RequierAdminRole")]
    [HttpPost("edit-roles/{userId}")]
    public async Task<ActionResult<List<string>>> EditRoles(
        string userId,
        [FromQuery] string roles
    )
    {
        if (string.IsNullOrWhiteSpace(roles))
        {
            return BadRequest(new
            {
                message =
                    "You must select at least one role."
            });
        }


        var selectedRoles =
            roles
                .Split(
                    ",",
                    StringSplitOptions.RemoveEmptyEntries
                )
                .Select(role =>
                    role.Trim().ToUpperInvariant()
                )
                .Distinct()
                .ToArray();


        var user =
            await userManager.FindByIdAsync(
                userId
            );


        if (user == null)
        {
            return NotFound(new
            {
                message =
                    "User was not found."
            });
        }


        // الأدوار المعتمدة في النظام
        var allowedRoles = new[]
        {
            "ADMIN",
            "MANAGER",
            "EQUIVALENCY",
            "RECEIVING",
            "INQUIRY",
            "ARCHIVE",
            "COMMITTEE_COORDINATOR",
            "COMMITTEE_MEMBER",
            "OFFICE",
            "PRINTING",
            "APPLICANT"
        };


        var invalidRoles =
            selectedRoles
                .Except(
                    allowedRoles,
                    StringComparer.OrdinalIgnoreCase
                )
                .ToArray();


        if (invalidRoles.Length > 0)
        {
            return BadRequest(new
            {
                message =
                    "One or more selected roles are invalid.",

                invalidRoles
            });
        }


        var currentRoles =
            await userManager.GetRolesAsync(
                user
            );


        // إضافة الدور الجديد
        var addResult =
            await userManager.AddToRolesAsync(
                user,
                selectedRoles.Except(
                    currentRoles,
                    StringComparer.OrdinalIgnoreCase
                )
            );


        if (!addResult.Succeeded)
        {
            return BadRequest(new
            {
                message =
                    "Failed to add the selected role.",

                errors =
                    addResult.Errors.Select(
                        x => x.Description
                    )
            });
        }


        // إزالة الأدوار القديمة
        var removeResult =
            await userManager.RemoveFromRolesAsync(
                user,
                currentRoles.Except(
                    selectedRoles,
                    StringComparer.OrdinalIgnoreCase
                )
            );


        if (!removeResult.Succeeded)
        {
            return BadRequest(new
            {
                message =
                    "Failed to remove the old role.",

                errors =
                    removeResult.Errors.Select(
                        x => x.Description
                    )
            });
        }


        var updatedRoles =
            await userManager.GetRolesAsync(
                user
            );


        return Ok(
            updatedRoles.ToList()
        );
    }


    [Authorize(Policy = "ManagePhotoRole")]
    [HttpGet("photos-to-manage")]
    public ActionResult GetPhotosForManaging()
    {
        return Ok(
            "Admins or Manager can see this"
        );
    }


    // =========================================================
    // Active Applications
    // =========================================================

    [Authorize(Policy = "RequierAdminRole")]
    [HttpGet("applications/active")]
    public async Task<ActionResult<List<AdminApplicationDto>>>
        GetActiveApplications(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] QualificationType? qualification,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to
        )
    {
        // الحالات التي تعتبر طلبات نشطة
        var activeStatuses = new[]
        {
            "Submitted",
            "UnderReview",
            "WaitingForInquiry",
            "Committee"
        };


        var query = context
            .Set<EquivalencyApplication>()
            .Include(x => x.Applicant)
            .Where(
                x => activeStatuses.Contains(x.Status)
            )
            .AsQueryable();


        // =====================================================
        // Search
        // =====================================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm =
                search.Trim();

            var hasApplicationId =
                int.TryParse(
                    searchTerm,
                    out var applicationId
                );


            // دعم البحث برقم الطلب مثل:
            // EQ-2026-00001
            if (
                !hasApplicationId
                &&
                searchTerm.StartsWith(
                    "EQ-",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                var parts =
                    searchTerm.Split(
                        '-',
                        StringSplitOptions.RemoveEmptyEntries
                    );

                if (parts.Length >= 3)
                {
                    hasApplicationId =
                        int.TryParse(
                            parts[^1],
                            out applicationId
                        );
                }
            }


            query = query.Where(x =>
                x.Applicant.DisplayName.Contains(searchTerm)
                ||
                x.Status.Contains(searchTerm)
                ||
                (
                    hasApplicationId
                    &&
                    x.Id == applicationId
                )
            );
        }


        // =====================================================
        // Status Filter
        // =====================================================

        if (!string.IsNullOrWhiteSpace(status))
        {
            var selectedStatus =
                status.Trim();

            var validStatus =
                activeStatuses.Any(
                    x =>
                        x.Equals(
                            selectedStatus,
                            StringComparison.OrdinalIgnoreCase
                        )
                );

            if (!validStatus)
            {
                return BadRequest(new
                {
                    message =
                        "The selected status is not an active application status."
                });
            }


            var normalizedStatus =
                activeStatuses.First(
                    x =>
                        x.Equals(
                            selectedStatus,
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            query = query.Where(
                x => x.Status == normalizedStatus
            );
        }


        // =====================================================
        // Qualification Filter
        // =====================================================

        if (qualification.HasValue)
        {
            query = query.Where(
                x =>
                    x.QualificationType
                    == qualification.Value
            );
        }


        // =====================================================
        // From Date
        // =====================================================

        if (from.HasValue)
        {
            var fromDate =
                from.Value.Date;

            query = query.Where(x =>
                (x.SubmittedAt ?? x.CreatedAt)
                >= fromDate
            );
        }


        // =====================================================
        // To Date
        // =====================================================

        if (to.HasValue)
        {
            var toExclusive =
                to.Value.Date.AddDays(1);

            query = query.Where(x =>
                (x.SubmittedAt ?? x.CreatedAt)
                < toExclusive
            );
        }


        // =====================================================
        // Execute
        // =====================================================

        var applications =
            await query
                .OrderByDescending(
                    x =>
                        x.SubmittedAt
                        ?? x.CreatedAt
                )
                .ToListAsync();


        var result =
            applications
                .Select(application =>
                    new AdminApplicationDto
                    {
                        Id =
                            application.Id,

                        RequestNumber =
                            $"EQ-{application.CreatedAt.Year}-{application.Id:D5}",

                        ApplicantName =
                            application
                                .Applicant
                                .DisplayName,

                        QualificationType =
                            application
                                .QualificationType
                                .ToString(),

                        Status =
                            application.Status,

                        Date =
                            application.SubmittedAt
                            ?? application.CreatedAt
                    }
                )
                .ToList();


        return Ok(result);
    }


    // =========================================================
    // Archived Applications
    // =========================================================

    [Authorize(Policy = "RequierAdminRole")]
    [HttpGet("applications/archive")]
    public async Task<ActionResult<List<AdminApplicationDto>>>
        GetArchivedApplications(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] QualificationType? qualification,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to
        )
    {
        // الحالات المؤرشفة
        var archivedStatuses = new[]
        {
            "Completed",
            "Rejected",
            "Cancelled"
        };


        var query = context
            .Set<EquivalencyApplication>()
            .Include(x => x.Applicant)
            .Where(
                x =>
                    archivedStatuses.Contains(x.Status)
            )
            .AsQueryable();


        // =====================================================
        // Search
        // =====================================================

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm =
                search.Trim();

            var hasApplicationId =
                int.TryParse(
                    searchTerm,
                    out var applicationId
                );


            // دعم البحث برقم الطلب كامل
            if (
                !hasApplicationId
                &&
                searchTerm.StartsWith(
                    "EQ-",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                var parts =
                    searchTerm.Split(
                        '-',
                        StringSplitOptions.RemoveEmptyEntries
                    );

                if (parts.Length >= 3)
                {
                    hasApplicationId =
                        int.TryParse(
                            parts[^1],
                            out applicationId
                        );
                }
            }


            query = query.Where(x =>
                x.Applicant.DisplayName.Contains(searchTerm)
                ||
                x.Status.Contains(searchTerm)
                ||
                (
                    hasApplicationId
                    &&
                    x.Id == applicationId
                )
            );
        }


        // =====================================================
        // Status Filter
        // =====================================================

        if (!string.IsNullOrWhiteSpace(status))
        {
            var selectedStatus =
                status.Trim();

            var validStatus =
                archivedStatuses.Any(
                    x =>
                        x.Equals(
                            selectedStatus,
                            StringComparison.OrdinalIgnoreCase
                        )
                );

            if (!validStatus)
            {
                return BadRequest(new
                {
                    message =
                        "The selected status is not an archived application status."
                });
            }


            var normalizedStatus =
                archivedStatuses.First(
                    x =>
                        x.Equals(
                            selectedStatus,
                            StringComparison.OrdinalIgnoreCase
                        )
                );


            query = query.Where(
                x => x.Status == normalizedStatus
            );
        }


        // =====================================================
        // Qualification Filter
        // =====================================================

        if (qualification.HasValue)
        {
            query = query.Where(
                x =>
                    x.QualificationType
                    == qualification.Value
            );
        }


        // =====================================================
        // From Date
        // =====================================================

        if (from.HasValue)
        {
            var fromDate =
                from.Value.Date;

            query = query.Where(x =>
                (x.SubmittedAt ?? x.CreatedAt)
                >= fromDate
            );
        }


        // =====================================================
        // To Date
        // =====================================================

        if (to.HasValue)
        {
            var toExclusive =
                to.Value.Date.AddDays(1);

            query = query.Where(x =>
                (x.SubmittedAt ?? x.CreatedAt)
                < toExclusive
            );
        }


        var applications =
            await query
                .OrderByDescending(
                    x =>
                        x.SubmittedAt
                        ?? x.CreatedAt
                )
                .ToListAsync();


        var result =
            applications
                .Select(application =>
                    new AdminApplicationDto
                    {
                        Id =
                            application.Id,

                        RequestNumber =
                            $"EQ-{application.CreatedAt.Year}-{application.Id:D5}",

                        ApplicantName =
                            application
                                .Applicant
                                .DisplayName,

                        QualificationType =
                            application
                                .QualificationType
                                .ToString(),

                        Status =
                            application.Status,

                        Date =
                            application.SubmittedAt
                            ?? application.CreatedAt
                    }
                )
                .ToList();


        return Ok(result);
    }


    // =========================================================
    // Dashboard Statistics
    // =========================================================

    [Authorize(Policy = "RequierAdminRole")]
    [HttpGet("dashboard-stats")]
    public async Task<ActionResult>
        GetDashboardStats()
    {
        var totalApplications =
            await context
                .Set<EquivalencyApplication>()
                .CountAsync();


        var underReview =
            await context
                .Set<EquivalencyApplication>()
                .CountAsync(
                    x =>
                        x.Status == "UnderReview"
                );


        var waitingForInquiry =
            await context
                .Set<EquivalencyApplication>()
                .CountAsync(
                    x =>
                        x.Status == "WaitingForInquiry"
                );


        var committee =
            await context
                .Set<EquivalencyApplication>()
                .CountAsync(
                    x =>
                        x.Status == "Committee"
                );


        var completed =
            await context
                .Set<EquivalencyApplication>()
                .CountAsync(
                    x =>
                        x.Status == "Completed"
                );


        var active =
            await context
                .Set<EquivalencyApplication>()
                .CountAsync(
                    x =>
                        x.Status == "Submitted"
                        ||
                        x.Status == "UnderReview"
                        ||
                        x.Status == "WaitingForInquiry"
                        ||
                        x.Status == "Committee"
                );


        var archived =
            await context
                .Set<EquivalencyApplication>()
                .CountAsync(
                    x =>
                        x.Status == "Completed"
                        ||
                        x.Status == "Rejected"
                        ||
                        x.Status == "Cancelled"
                );


        return Ok(new
        {
            totalApplications,
            active,
            archived,
            underReview,
            waitingForInquiry,
            committee,
            completed
        });
    }


    // =========================================================
    // Application Details
    // =========================================================

    [Authorize(Policy = "RequierAdminRole")]
    [HttpGet("applications/{id}")]
    public async Task<ActionResult<AdminApplicationDetailsDto>>
        GetApplicationDetails(int id)
    {
        var application =
            await context
                .Set<EquivalencyApplication>()
                .Include(x => x.Applicant)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(
                    x => x.Id == id
                );


        if (application == null)
        {
            return NotFound(new
            {
                message =
                    "Application not found."
            });
        }


        var result =
            new AdminApplicationDetailsDto
            {
                Id =
                    application.Id,

                RequestNumber =
                    $"EQ-{application.CreatedAt.Year}-{application.Id:D5}",

                ApplicantName =
                    application
                        .Applicant
                        .DisplayName,

                NationalId =
                    application
                        .Applicant
                        .NationalId,

                Email =
                    application
                        .Applicant
                        .User
                        .Email,

                PhoneNumber =
                    application
                        .Applicant
                        .PhoneNumber,

                QualificationType =
                    application
                        .QualificationType
                        .ToString(),

                Status =
                    application.Status,

                CreatedAt =
                    application.CreatedAt,

                SubmittedAt =
                    application.SubmittedAt
            };


        return Ok(result);
    }


        // =========================================================
    // Printing Applications
    // =========================================================

    [Authorize(Policy = "RequierAdminRole")]
    [HttpGet("printing")]
    public async Task<ActionResult> GetPrintingApplications()
    {
        var applications = await context
            .Set<EquivalencyApplication>()
            .Include(x => x.Applicant)
            .Where(x =>
                x.Status == "Completed"
            )
            .OrderByDescending(x =>
                x.SubmittedAt ?? x.CreatedAt
            )
            .ToListAsync();


        var rows = applications
            .Select(application => new
            {
                id = application.Id,

                requestNumber =
                    $"EQ-{application.CreatedAt.Year}-{application.Id:D5}",

                applicantName =
                    application.Applicant.DisplayName,

                qualificationType =
                    application.QualificationType.ToString(),

                status =
                    application.Status,

                date =
                    application.SubmittedAt ??
                    application.CreatedAt
            })
            .ToList();


        return Ok(new
        {
            total = rows.Count,

            drafts = 0,

            awaitingApplicant = 0,

            approved = rows.Count,

            final = 0,

            applications = rows
        });
    }




}