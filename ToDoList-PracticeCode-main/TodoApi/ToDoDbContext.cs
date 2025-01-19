using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TodoApi;

public partial class ToDoDbContext : DbContext
{
    public ToDoDbContext()
    {
    }

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tabl> Tabls { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Tabl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tabl");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.IsComplete).HasColumnType("TINYINT(1)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
