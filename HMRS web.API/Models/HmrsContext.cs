using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HMRS_web.API.Models;

public partial class HmrsContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{ 
        
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

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ✅ Required for Identity to work

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.CvfilePath).HasMaxLength(200).HasColumnName("CVFilePath");
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Employees_Department");

                entity.HasOne(d => d.JobRole).WithMany(p => p.Employees)
                    .HasForeignKey(d => d.JobRoleId)
                    .HasConstraintName("FK_Employees_JobRole");
            });

            modelBuilder.Entity<Evaluation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.HasOne(d => d.Employee).WithMany(p => p.EvaluationEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Evaluations_Employee");

                entity.HasOne(d => d.Reviewer).WithMany(p => p.EvaluationReviewers)
                    .HasForeignKey(d => d.ReviewerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Evaluations_Reviewer");
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.FileName).HasMaxLength(100);
                entity.Property(e => e.FilePath).HasMaxLength(255);
                entity.Property(e => e.FileType).HasMaxLength(20);
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.UploadedBy).HasMaxLength(450);
            });

            modelBuilder.Entity<JobRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Reason).HasMaxLength(255);
                entity.Property(e => e.RequestedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");

                entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.LeaveRequestApprovedByNavigations)
                    .HasForeignKey(d => d.ApprovedBy)
                    .HasConstraintName("FK_LeaveRequests_ApprovedBy");

                entity.HasOne(d => d.Employee).WithMany(p => p.LeaveRequestEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LeaveRequests_Employee");
            });

        OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
