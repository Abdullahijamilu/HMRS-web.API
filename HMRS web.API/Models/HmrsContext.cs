using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HMRS_web.API.Models;

public partial class HmrsContext : DbContext
{
    public HmrsContext()
    {
    }

    public HmrsContext(DbContextOptions<HmrsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Evaluation> Evaluations { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<JobRole> JobRoles { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-MIU8T4N;Database=HMRS;User Id=Abdul Jamiu;Password=Abdul131;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC07D43A80C7");

            entity.ToTable("Department");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC073561C314");

            entity.ToTable("Employee");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CvfilePath).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Employee_Department");

            entity.HasOne(d => d.JobRole).WithMany(p => p.Employees)
                .HasForeignKey(d => d.JobRoleId)
                .HasConstraintName("FK_Employee_JobRole");

            entity.HasOne(d => d.User).WithMany(p => p.Employees)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_User");
        });

        modelBuilder.Entity<Evaluation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Evaluati__3214EC071B703AE2");

            entity.ToTable("Evaluation");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Employee).WithMany(p => p.EvaluationEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evaluation_Employee");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.EvaluationReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evaluation_Reviewer");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__File__3214EC07F2A337E1");

            entity.ToTable("File");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(100);
            entity.Property(e => e.UploadedAt).HasColumnType("datetime");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Files).HasForeignKey(d => d.UploadedBy);
        });

        modelBuilder.Entity<JobRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JobRole__3214EC07F0AA1A5E");

            entity.ToTable("JobRole");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveReq__3214EC071B0E37C2");

            entity.ToTable("LeaveRequest");

            entity.Property(e => e.Reason).IsUnicode(false);
            entity.Property(e => e.RequestedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.LeaveRequestApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK_LeaveRequest_ApprovedBy");

            entity.HasOne(d => d.Employee).WithMany(p => p.LeaveRequestEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRequest_Employee");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07AB8E7219");

            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.JobRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.JobRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_JobRole");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
