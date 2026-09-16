using System;
using API.Entities;
using API.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace API.Data;


public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{

    public DbSet<Applicant> Applicants { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    public DbSet<QualificationRequirement> QualificationRequirements { get; set; }

    public DbSet<ApplicantQualification> ApplicantQualifications { get; set; }

    public DbSet<EquivalencyApplication> EquivalencyApplications { get; set; }

    public DbSet<Photo> Photos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicantQualification>()
    .HasOne(q => q.Applicant)
    .WithMany(a => a.qualifications)
    .HasForeignKey(q => q.ApplicantId)
    .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<EquivalencyApplication>()
    .HasOne(a => a.Applicant)
    .WithMany(a => a.EquivalencyApplications)
    .HasForeignKey(a => a.ApplicantId)
    .OnDelete(DeleteBehavior.Cascade);

        //للادوار والصلاحيات الي لازم تكون عندي بالداتا بيس
        modelBuilder.Entity<IdentityRole>()
        .HasData(
new IdentityRole
{
    Id = "admin-id",
    Name = "Admin",
    NormalizedName = "ADMIN",
    ConcurrencyStamp = "admin-concurrency-stamp"
},

new IdentityRole
{
    Id = "manager-id",
    Name = "Manager",
    NormalizedName = "MANAGER",
    ConcurrencyStamp = "manager-concurrency-stamp"
},
new IdentityRole
{
    Id = "equivalency-id",
    Name = "Equivalency",
    NormalizedName = "EQUIVALENCY",
    ConcurrencyStamp = "equivalency-concurrency-stamp"
},
new IdentityRole
{
    Id = "receiving-id",
    Name = "Receiving",
    NormalizedName = "RECEIVING",
    ConcurrencyStamp = "receiving-concurrency-stamp"
},
new IdentityRole
{
    Id = "inquiry-id",
    Name = "Inquiry",
    NormalizedName = "INQUIRY",
    ConcurrencyStamp = "inquiry-concurrency-stamp"
},
new IdentityRole
{
    Id = "archive-id",
    Name = "Archive",
    NormalizedName = "ARCHIVE",
    ConcurrencyStamp = "archive-concurrency-stamp"
},
new IdentityRole
{
    Id = "office-id",
    Name = "Office",
    NormalizedName = "OFFICE",
    ConcurrencyStamp = "office-concurrency-stamp"
},
new IdentityRole
{
    Id = "printing-id",
    Name = "Printing",
    NormalizedName = "PRINTING",
    ConcurrencyStamp = "printing-concurrency-stamp"
},
new IdentityRole
{
    Id = "committee_coordinator-id",
    Name = "Committee_Coordinator",
    NormalizedName = "COMMITTEE_COORDINATOR",
    ConcurrencyStamp = "committee_coordinator-concurrency-stamp"
},
new IdentityRole
{
    Id = "committee_member-id",
    Name = "Committee_Member",
    NormalizedName = "COMMITTEE_MEMBER",
    ConcurrencyStamp = "committee_member-concurrency-stamp"
},
new IdentityRole
{
    Id = "applicant-id",
    Name = "Applicant",
    NormalizedName = "APPLICANT",
    ConcurrencyStamp = "applicant-concurrency-stamp"
}
        );
        //للمتطلبات الشايقة تاعت الشهادات
        modelBuilder.Entity<QualificationRequirement>()
        .HasData(
            new QualificationRequirement
            {
                Id = 1,
                QualificationType = QualificationType.Diploma,
                RequiredQualificationType = QualificationType.Secondary
            },
            new QualificationRequirement
            {
                Id = 2,
                QualificationType = QualificationType.Bachelor,
                RequiredQualificationType = QualificationType.Secondary
            },
            new QualificationRequirement
            {
                Id = 3,
                QualificationType = QualificationType.Master,
                RequiredQualificationType = QualificationType.Bachelor
            },
            new QualificationRequirement
            {
                Id = 4,
                QualificationType = QualificationType.Master,
                RequiredQualificationType = QualificationType.Secondary

            },
            new QualificationRequirement
            {
                Id = 5,
                QualificationType = QualificationType.PhD,
                RequiredQualificationType = QualificationType.Secondary
            },
            new QualificationRequirement
            {
                Id = 6,
                QualificationType = QualificationType.PhD,
                RequiredQualificationType = QualificationType.Bachelor

            },
            new QualificationRequirement
            {
                Id = 7,
                QualificationType = QualificationType.PhD,
                RequiredQualificationType = QualificationType.Master

            }
        );

    }
}