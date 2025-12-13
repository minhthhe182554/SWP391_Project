using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;
using SWP391_Project.Services;
using SWP391_Project.Repositories;
using SWP391_Project.Helpers;
using SWP391_Project.Repositories.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Regist EzJobDbContext 
builder.Services.AddDbContext<EzJobDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Register Repositories
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Register Repositories
builder.Services.AddScoped<IJobRepository, JobRepository>();

// Register Services
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJobService, JobService>();

// Register Storage Service (no Infrastructure layer)
builder.Services.AddScoped<SWP391_Project.Services.Storage.IStorageService, SWP391_Project.Services.Storage.StorageService>();

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
