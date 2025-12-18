using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;
using SWP391_Project.Services;
using SWP391_Project.Repositories;
using SWP391_Project.Helpers;
using SWP391_Project.Repositories.Storage;
using SWP391_Project.Services.Storage;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);



// Regist EzJobDbContext 
builder.Services.AddDbContext<EzJobDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Register Repositories
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();         
builder.Services.AddScoped<ILocationRepository, LocationRepository>(); 
builder.Services.AddScoped<ISkillRepository, SkillRepository>();       
builder.Services.AddScoped<IDomainRepository, DomainRepository>();
builder.Services.AddScoped<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


// Register Services
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ISavedJobRepository, SavedJobRepository>();
builder.Services.AddScoped<ISavedJobService, SavedJobService>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IDomainService, DomainService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register Storage Service (no Infrastructure layer)
builder.Services.AddScoped<IStorageService, StorageService>();

// Use repository-based storage abstraction only
builder.Services.AddScoped<IStorageRepository, CloudinaryStorageRepository>();

// Register HttpClient and LocationService
builder.Services.AddHttpClient<ILocationService, LocationService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache(); //dung ram luu token tam

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
