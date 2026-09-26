using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class InstitutionDbContext : DbContext
{
    public InstitutionDbContext(
        DbContextOptions<InstitutionDbContext> options)
        : base(options)
    {
    }

public DbSet<InstitutionCountry> Countries { get; set; }
public DbSet<Institution> Institutions { get; set; }
public DbSet<InstitutionFaculty> Faculties { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InstitutionFaculty>(entity =>
{
    entity.ToTable("Faculies", "inst");

    entity.HasKey(f => f.FacultyID);

    entity.Property(f => f.FacultyID)
        .HasColumnName("FacultyID");

    entity.Property(f => f.InstituteID)
        .HasColumnName("InstituteID");

    entity.Property(f => f.CampusID)
        .HasColumnName("CampusID");

    entity.Property(f => f.FacultyName)
        .HasColumnName("FacultyName");

    entity.Property(f => f.FacultyNameEn)
        .HasColumnName("FacultyNameEn");

    entity.Property(f => f.IsActive)
        .HasColumnName("IsActive");
});
        modelBuilder.Entity<InstitutionCountry>(entity =>
        {
            entity.ToTable("Country", "shrd");

            entity.HasKey(c => c.ID);

            entity.Property(c => c.ID)
                .HasColumnName("ID");

            entity.Property(c => c.Name)
                .HasColumnName("Name");

            entity.Property(c => c.NameEn)
                .HasColumnName("NameEn");
        });

        modelBuilder.Entity<Institution>(entity =>
        {
            entity.ToTable("Institutions", "inst");

            entity.HasKey(i => i.InstituteID);

            entity.Property(i => i.InstituteID)
                .HasColumnName("InstituteID");

            entity.Property(i => i.InstituteName)
                .HasColumnName("InstituteName");

            entity.Property(i => i.InstituteNameEn)
                .HasColumnName("InstituteNameEn");

            entity.Property(i => i.CountryID)
                .HasColumnName("CountryID");

            entity.Property(i => i.IsActive)
                .HasColumnName("IsActive");
        });
    }
}
