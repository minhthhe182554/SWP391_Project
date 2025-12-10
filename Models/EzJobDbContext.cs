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

        // Configure Identity Columns for all entities
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Candidate>()
            .Property(c => c.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Company>()
            .Property(c => c.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Job>()
            .Property(j => j.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Application>()
            .Property(a => a.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Resume>()
            .Property(r => r.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Skill>()
            .Property(s => s.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Domain>()
            .Property(d => d.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Location>()
            .Property(l => l.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Certificate>()
            .Property(c => c.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<WorkExperience>()
            .Property(w => w.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<EducationRecord>()
            .Property(e => e.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Notification>()
            .Property(n => n.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Report>()
            .Property(r => r.Id)
            .UseIdentityColumn();
            
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
            .HasMaxLength(500)
            .HasDefaultValue("default_yvl9oh");
        
        modelBuilder.Entity<Company>()
            .Property(c => c.Website)
            .HasMaxLength(255);

        // Job
        modelBuilder.Entity<Job>()
            .Property(j => j.Title)
            .HasMaxLength(255);

        modelBuilder.Entity<Job>()
            .Property(j => j.Address)
            .HasMaxLength(500);

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
            .Property(r => r.Reason)
            .HasMaxLength(2000);

        // Certificate
        modelBuilder.Entity<Certificate>()
            .Property(c => c.Name)
            .HasMaxLength(255);
    }
}
