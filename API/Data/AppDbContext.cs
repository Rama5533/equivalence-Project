using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;


public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{

    public DbSet<Member> Members { get; set; }

    public DbSet<Photo> Photos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>()
        .HasData(

            new IdentityRole
            {
                Id = "member-id",
                Name = "Member",
                NormalizedName = "MEMBER",
                ConcurrencyStamp = "member-concurrency-stamp"
            },
            new IdentityRole
            {
                Id = "moderator-id",
                Name = "Moderator",
                NormalizedName = "MODERATOR",
                ConcurrencyStamp = "moderator-concurrency-stamp"
            },
            new IdentityRole
            {
                Id = "admin-id",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "admin-concurrency-stamp"
            }

        );
    }
}