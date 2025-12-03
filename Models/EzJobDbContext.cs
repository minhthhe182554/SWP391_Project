using Microsoft.EntityFrameworkCore;

namespace SWP391_Project.Models;

public class EzJobDbContext : DbContext
{
    public EzJobDbContext(DbContextOptions<EzJobDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Domain> Domains => Set<Domain>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<WorkExperience> WorkExperiences => Set<WorkExperience>();
    public DbSet<EducationRecord> EducationRecords => Set<EducationRecord>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<SavedJob> SavedJobs => Set<SavedJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Quan hệ 1-1 giữa User và Candidate.
        modelBuilder.Entity<User>()
            .HasOne(u => u.Candidate)
            .WithOne(c => c.User)
            .HasForeignKey<Candidate>(c => c.UserId);

        // Quan hệ 1-1 giữa User và Company.
        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithOne(c => c.User)
            .HasForeignKey<Company>(c => c.UserId);

        // Bảng trung gian SavedJob (many-to-many giữa Candidate và Job).
        modelBuilder.Entity<SavedJob>()
            .HasKey(sj => new { sj.CandidateId, sj.JobId });

        modelBuilder.Entity<SavedJob>()
            .HasOne(sj => sj.Candidate)
            .WithMany(c => c.SavedJobs)
            .HasForeignKey(sj => sj.CandidateId);

        modelBuilder.Entity<SavedJob>()
            .HasOne(sj => sj.Job)
            .WithMany() // không có navigation ngược
            .HasForeignKey(sj => sj.JobId);

        // =========================
        // Cấu hình DeleteBehavior
        // =========================

        // Location trong nghiệp vụ KHÔNG bị xóa hard delete,
        // nên ta đặt DeleteBehavior.Restrict để tránh multiple cascade path
        // mà vẫn an toàn vì code không gọi Remove(Location).
        modelBuilder.Entity<Company>()
            .HasOne(c => c.Location)
            .WithMany(l => l.Companies)
            .HasForeignKey(c => c.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Job>()
            .HasOne(j => j.Location)
            .WithMany(l => l.Jobs)
            .HasForeignKey(j => j.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Company cũng soft delete, nên không cascade Company -> Jobs.
        // Khi cần xóa Job, ta chủ động Remove(Job) trong code.
        modelBuilder.Entity<Job>()
            .HasOne(j => j.Company)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Global query filters & ràng buộc dữ liệu
        // =========================

        // Soft delete cho Job: mặc định mọi query DbSet<Job> sẽ ẩn Job đã bị xoá (IsDelete = true).
        modelBuilder.Entity<Job>()
            .HasQueryFilter(j => !j.IsDelete);

        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique(); // Email phải là duy nhất

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(255);

        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .HasMaxLength(255); // đủ cho hash password

        // Candidate
        modelBuilder.Entity<Candidate>()
            .Property(c => c.FullName)
            .HasMaxLength(255);

        modelBuilder.Entity<Candidate>()
            .Property(c => c.PhoneNumber)
            .HasMaxLength(20);

        modelBuilder.Entity<Candidate>()
            .Property(c => c.ImageUrl)
            .HasMaxLength(500);

        // Company
        modelBuilder.Entity<Company>()
            .Property(c => c.Name)
            .HasMaxLength(255);

        modelBuilder.Entity<Company>()
            .Property(c => c.PhoneNumber)
            .HasMaxLength(20);

        modelBuilder.Entity<Company>()
            .Property(c => c.Address)
            .HasMaxLength(500);

        modelBuilder.Entity<Company>()
            .Property(c => c.ImageUrl)
            .HasMaxLength(500);

        modelBuilder.Entity<Company>()
            .Property(c => c.Website)
            .HasMaxLength(255);

        // Job
        modelBuilder.Entity<Job>() 
            .Property(j => j.Title)
            .HasMaxLength(255);

        modelBuilder.Entity<Job>()
            .Property(j => j.WorkTime)
            .HasMaxLength(100);

        // Resume
        modelBuilder.Entity<Resume>()
            .Property(r => r.Name)
            .HasMaxLength(255);

        modelBuilder.Entity<Resume>()
            .Property(r => r.Url)
            .HasMaxLength(500);

        // Skill
        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.Name)
            .IsUnique(); // tránh trùng tên kỹ năng

        modelBuilder.Entity<Skill>()
            .Property(s => s.Name)
            .HasMaxLength(100);

        // Domain
        modelBuilder.Entity<Domain>()
            .HasIndex(d => d.Name)
            .IsUnique(); // tránh trùng tên domain

        modelBuilder.Entity<Domain>()
            .Property(d => d.Name)
            .HasMaxLength(100);

        // Location
        modelBuilder.Entity<Location>()
            .Property(l => l.City)
            .HasMaxLength(100);

        modelBuilder.Entity<Location>()
            .Property(l => l.Ward)
            .HasMaxLength(100);

        // Notification
        modelBuilder.Entity<Notification>()
            .Property(n => n.Title)
            .HasMaxLength(255);

        // Report
        modelBuilder.Entity<Report>()
            .Property(r => r.Content)
            .HasMaxLength(2000);

        // Certificate
        modelBuilder.Entity<Certificate>()
            .Property(c => c.Name)
            .HasMaxLength(255);
    }
}
