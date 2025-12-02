using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Models;

namespace val_builder_api.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyPlan> CompanyPlans { get; set; }

    public virtual DbSet<Valannotation> Valannotations { get; set; }

    public virtual DbSet<Valdetail> Valdetails { get; set; }

    public virtual DbSet<Valheader> Valheaders { get; set; }

    public virtual DbSet<Valsection> Valsections { get; set; }

    public virtual DbSet<ValsubContent> ValsubContents { get; set; }

    public virtual DbSet<ValsubContentItem> ValsubContentItems { get; set; }

    public virtual DbSet<ValsubItem> ValsubItems { get; set; }

    public virtual DbSet<Valtask> Valtasks { get; set; }

    public virtual DbSet<ValtemplateItem> ValtemplateItems { get; set; }
    public virtual DbSet<ValPdfAttachment> ValPdfAttachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Valannotation>(entity =>
        {
            entity.Property(e => e.AnnotationId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Valdetail>(entity =>
        {
            entity.Property(e => e.ValDetailsId).HasDefaultValueSql("(newid())", "DF_VALDetails_ValDetailsID");
        });

        modelBuilder.Entity<ValsubContent>(entity =>
        {
            entity.Property(e => e.SubContentId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ValsubContentItem>(entity =>
        {
            entity.Property(e => e.ItemId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Valtask>(entity =>
        {
            entity.Property(e => e.TaskId).ValueGeneratedOnAdd();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
