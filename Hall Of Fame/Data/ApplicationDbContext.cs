using EntityConfigurations;
using Hall_Of_Fame.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Skills> Skills { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Hall_Of_Fame");

        modelBuilder.ApplyConfiguration(new PersonEntityTypeConfiguration()); 

        modelBuilder.Entity<Person>()
            .HasMany(p => p.Skills)
            .WithOne()
            .HasForeignKey(s => s.PersonId);
    }


}

