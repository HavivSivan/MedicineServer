using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MedicineServer.Models;

public partial class MedicineDbContext : DbContext
{
    public MedicineDbContext()
    {
    }

    public MedicineDbContext(DbContextOptions<MedicineDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<MedicineStatus> MedicineStatuses { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Pharmacy> Pharmacies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB;Initial Catalog=medicineDB;User ID=MedicineAdminLogin;Password=qwerty;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.MedicineId).HasName("PK__Medicine__4F21289064940FB9");

            entity.HasOne(d => d.Pharmacy).WithMany(p => p.Medicines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Medicines__Pharm__2D27B809");

            entity.HasOne(d => d.Status).WithMany(p => p.Medicines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Medicines__Statu__2E1BDC42");

            entity.HasOne(d => d.User).WithMany(p => p.Medicines)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Medicines__UserI__2F10007B");
        });

        modelBuilder.Entity<MedicineStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Medicine__C8EE20638431E0BD");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF2340D910");

            entity.HasOne(d => d.Medicine).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Medicine__31EC6D26");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserId__32E0915F");
        });

        modelBuilder.Entity<Pharmacy>(entity =>
        {
            entity.HasKey(e => e.PharmacyId).HasName("PK__Pharmaci__BD9D2AAE9429D4CB");

            entity.HasOne(d => d.User).WithMany(p => p.Pharmacies)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Pharmacie__UserI__276EDEB3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CE125A466");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
