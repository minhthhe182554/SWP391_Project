using Microsoft.EntityFrameworkCore;

namespace SWP391_Project.Models;

public class EzJobDbContext : DbContext
{
    public EzJobDbContext(DbContextOptions<EzJobDbContext> options) : base(options) {}

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

        // 1-1 Relationship between User & Candidate.
        modelBuilder.Entity<User>()
            .HasOne(u => u.Candidate)
            .WithOne(c => c.User)
            .HasForeignKey<Candidate>(c => c.UserId);

        // 1-1 Relationship between User & Company.
        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithOne(c => c.User)
            .HasForeignKey<Company>(c => c.UserId);

        // SavedJob tables (many-to-many Relationship between Candidate & Job).
        modelBuilder.Entity<SavedJob>()
            .HasKey(sj => new { sj.CandidateId, sj.JobId });

        modelBuilder.Entity<SavedJob>()
            .HasOne(sj => sj.Candidate)
            .WithMany(c => c.SavedJobs)
            .HasForeignKey(sj => sj.CandidateId);

        modelBuilder.Entity<SavedJob>()
            .HasOne(sj => sj.Job)
            .WithMany() 
            .HasForeignKey(sj => sj.JobId);

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

        modelBuilder.Entity<Job>()
            .HasOne(j => j.Company)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global query filters
        modelBuilder.Entity<Job>()
            .HasQueryFilter(j => !j.IsDelete);

        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique(); // Email must be unique

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(255); // email length must be <= 255 chars

        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .HasMaxLength(255); // password length must be <= 255 chars

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
            .IsUnique(); // Skill name is unique

        modelBuilder.Entity<Skill>()
            .Property(s => s.Name)
            .HasMaxLength(100);

        // Domain
        modelBuilder.Entity<Domain>()
            .HasIndex(d => d.Name)
            .IsUnique(); // Domain name is unique

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

        // SEED DATA (Dữ liệu mẫu khởi tạo)

        // 1. Locations data
        modelBuilder.Entity<Location>().HasData(
            new Location { Id = 1, City = "Hà Nội", Ward = "Cầu Giấy" },
            new Location { Id = 2, City = "Hồ Chí Minh", Ward = "Quận 1" },
            new Location { Id = 3, City = "Đà Nẵng", Ward = "Hải Châu" }
        );

        // 2. Skill & Domain data
        modelBuilder.Entity<Skill>().HasData(
            new Skill { Id = 1, Name = "Java" },
            new Skill { Id = 2, Name = "C#" },
            new Skill { Id = 3, Name = ".NET Core" },
            new Skill { Id = 4, Name = "ReactJS" },
            new Skill { Id = 5, Name = "SQL Server" }
        );

        modelBuilder.Entity<Domain>().HasData(
            new Domain { Id = 1, Name = "IT Phần mềm" },
            new Domain { Id = 2, Name = "Marketing" },
            new Domain { Id = 3, Name = "Sales" }
        );

        // 3. 3 User accounts with 3 different roles
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Email = "admin@ezjob.com", Password = "123456", Role = Role.ADMIN, Active = true },
            new User { Id = 2, Email = "recruiter@fpt.com", Password = "123456", Role = Role.COMPANY, Active = true },
            new User { Id = 3, Email = "ungvien@gmail.com", Password = "123456", Role = Role.CANDIDATE, Active = true }
        );

        // 4. Companies
        modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = 1,
                UserId = 2,
                Name = "FPT Software",
                Description = "Công ty công nghệ hàng đầu Việt Nam",
                Address = "Số 17 Duy Tân",
                PhoneNumber = "0988888888",
                LocationId = 1, // Hà Nội
                Website = "https://fpt-software.com"
            }
        );

        // 5. Candidate record (userId = 3)
        modelBuilder.Entity<Candidate>().HasData(
            new Candidate
            {
                Id = 1,
                UserId = 3,
                FullName = "Nguyễn Văn A",
                PhoneNumber = "0912345678",
                Jobless = true,
                RemainingReport = 2
            }
        );

        // 6. Sample Job
        modelBuilder.Entity<Job>().HasData(
            new Job
            {
                Id = 1,
                Title = "Tuyển dụng Senior .NET Developer",
                Description = "Tham gia phát triển dự án Banking...",
                WorkTime = "Full-time (T2-T6)",
                Type = JobType.FULLTIME,
                YearsOfExperience = 2,
                VacancyCount = 5,
                LowerSalaryRange = 15000000, 
                HigherSalaryRange = 30000000, 
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30), 
                CompanyId = 1, // FPT Company
                LocationId = 1, 
                IsDelete = false
            }
        );
    }
}
