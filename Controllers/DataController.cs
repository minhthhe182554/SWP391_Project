using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using Role = SWP391_Project.Models.Role;

namespace SWP391_Project.Controllers
{
    public class DataController : Controller
    {
        private readonly EzJobDbContext _context;
        private readonly ILogger<DataController> _logger;
        private readonly ICloudinaryHelper _cloudinaryHelper;

        public DataController(EzJobDbContext context, ILogger<DataController> logger, ICloudinaryHelper cloudinaryHelper)
        {
            _context = context;
            _logger = logger;
            _cloudinaryHelper = cloudinaryHelper;
        }

        [HttpGet]
        public IActionResult Seed()
        {
            try
            {
                _logger.LogInformation("Bắt đầu seed data...");

                if (!_context.Locations.Any())
                {
                    _context.Locations.AddRange(
                        new Location { City = "Hà Nội", Ward = "Cầu Giấy" },
                        new Location { City = "Hồ Chí Minh", Ward = "Quận 1" },
                        new Location { City = "Đà Nẵng", Ward = "Hải Châu" }
                    );
                    _context.SaveChanges();
                    _logger.LogInformation("Đã seed Locations: 3 records");
                }
                else
                {
                    _logger.LogInformation("Locations đã tồn tại, bỏ qua");
                }

                if (!_context.Skills.Any())
                {
                    _context.Skills.AddRange(
                        new Skill { Name = "Java" },
                        new Skill { Name = "C#" },
                        new Skill { Name = ".NET Core" },
                        new Skill { Name = "ReactJS" },
                        new Skill { Name = "SQL Server" }
                    );
                    _context.SaveChanges();
                    _logger.LogInformation("Đã seed Skills: 5 records");
                }
                else
                {
                    _logger.LogInformation("Skills đã tồn tại, bỏ qua");
                }

                if (!_context.Domains.Any())
                {
                    _context.Domains.AddRange(
                        new Domain { Name = "IT Phần mềm" },
                        new Domain { Name = "Marketing" },
                        new Domain { Name = "Sales" }
                    );
                    _context.SaveChanges();
                    _logger.LogInformation("Đã seed Domains: 3 records");
                }
                else
                {
                    _logger.LogInformation("Domains đã tồn tại, bỏ qua");
                }

                if (!_context.Users.Any())
                {
                    var adminUser = new User
                    {
                        Email = "admin@ezjob.com",
                        Password = HashHelper.Hash("123456"),
                        Role = Role.ADMIN,
                        Active = true
                    };
                    var companyUser = new User
                    {
                        Email = "recruiter@fpt.com",
                        Password = HashHelper.Hash("123456"),
                        Role = Role.COMPANY,
                        Active = true
                    };
                    var candidateUser = new User
                    {
                        Email = "ungvien@gmail.com",
                        Password = HashHelper.Hash("123456"),
                        Role = Role.CANDIDATE,
                        Active = true
                    };

                    _context.Users.AddRange(adminUser, companyUser, candidateUser);
                    _context.SaveChanges();
                    _logger.LogInformation("Đã seed Users: 3 records");

                    var location = _context.Locations.FirstOrDefault(l => l.City == "Hà Nội" && l.Ward == "Cầu Giấy");
                    if (location != null)
                    {
                        var company = new Company
                        {
                            UserId = companyUser.Id,
                            Name = "FPT Software",
                            Description = "Công ty công nghệ hàng đầu Việt Nam",
                            Address = "Số 17 Duy Tân",
                            PhoneNumber = "0988888888",
                            LocationId = location.Id,
                            Website = "https://fpt-software.com"
                        };
                        _context.Companies.Add(company);
                        _context.SaveChanges();
                        _logger.LogInformation("Đã seed Company: 1 record");

                        var candidate = new Candidate
                        {
                            UserId = candidateUser.Id,
                            FullName = "Nguyễn Văn A",
                            PhoneNumber = "0912345678",
                            Jobless = true,
                            RemainingReport = 2
                        };
                        _context.Candidates.Add(candidate);
                        _context.SaveChanges();
                        _logger.LogInformation("Đã seed Candidate: 1 record");

                        var job = new Job
                        {
                            Title = "Tuyển dụng Senior .NET Developer",
                            Description = "Tham gia phát triển dự án Banking...",
                            Type = JobType.FULLTIME,
                            YearsOfExperience = 2,
                            VacancyCount = 5,
                            LowerSalaryRange = 15000000,
                            HigherSalaryRange = 30000000,
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now.AddDays(30),
                            CompanyId = company.Id,
                            LocationId = location.Id,
                            IsDelete = false
                        };
                        _context.Jobs.Add(job);
                        _context.SaveChanges();
                        _logger.LogInformation("Đã seed Job: 1 record");
                    }
                }
                else
                {
                    _logger.LogInformation("Users đã tồn tại, bỏ qua");
                }

                _logger.LogInformation("Seed data hoàn tất!");
                return Content("Seed data thành công! Kiểm tra console/log để xem chi tiết.", "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi seed data");
                return Content($"Lỗi khi seed data: {ex.Message}", "text/html");
            }
        }

        [HttpGet]
        public IActionResult TestCloudinaryImage()
        {
            try
            {
                _logger.LogInformation("TestCloudinaryImage: Starting...");

                // Get UserID from session
                var userId = HttpContext.Session.GetString("UserID");
                _logger.LogInformation("TestCloudinaryImage: Retrieved UserID from session: {UserId}", userId);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("TestCloudinaryImage: User not logged in");
                    return Content("<h1>Error: You are not logged in</h1>", "text/html");
                }

                if (!int.TryParse(userId, out int userIdInt))
                {
                    _logger.LogWarning("TestCloudinaryImage: Invalid UserID format: {UserId}", userId);
                    return Content("<h1>Error: Invalid UserID format</h1>", "text/html");
                }

                // Find user with navigation properties
                var user = _context.Users
                    .Include(u => u.Candidate)
                    .Include(u => u.Company)
                    .FirstOrDefault(u => u.Id == userIdInt);
                _logger.LogInformation("TestCloudinaryImage: User lookup result: {UserFound}", user != null);

                if (user == null)
                {
                    _logger.LogWarning("TestCloudinaryImage: User not found with ID: {UserId}", userIdInt);
                    return Content("<h1>Error: User not found</h1>", "text/html");
                }

                // Get ImageUrl from Candidate or Company
                string? imageUrl = null;
                string? userName = "Unknown";

                if (user.Candidate != null)
                {
                    imageUrl = user.Candidate.ImageUrl;
                    userName = user.Candidate.FullName;
                    _logger.LogInformation("TestCloudinaryImage: User is CANDIDATE. ImageUrl: {ImageUrl}", imageUrl);
                }
                else if (user.Company != null)
                {
                    imageUrl = user.Company.ImageUrl;
                    userName = user.Company.Name;
                    _logger.LogInformation("TestCloudinaryImage: User is COMPANY. ImageUrl: {ImageUrl}", imageUrl);
                }
                else
                {
                    _logger.LogWarning("TestCloudinaryImage: User has neither Candidate nor Company profile");
                }

                // If no ImageUrl, display info message
                if (string.IsNullOrEmpty(imageUrl))
                {
                    _logger.LogWarning("TestCloudinaryImage: No ImageUrl found for user {UserId}", userIdInt);
                    return Content(
                        $"<h1>ℹ️ Information</h1>" +
                        $"<p><strong>Name:</strong> {userName}</p>" +
                        $"<p><strong>Role:</strong> {user.Role}</p>" +
                        $"<p style='color: orange;'><strong>⚠️ No ImageUrl found</strong></p>" +
                        $"<p>Please upload a profile image first.</p>",
                        "text/html"
                    );
                }

                // Build image URL using CloudinaryHelper
                _logger.LogInformation("TestCloudinaryImage: Building image URL using CloudinaryHelper");
                var finalImageUrl = _cloudinaryHelper.BuildImageUrl(imageUrl);

                // Return HTML displaying image
                var html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Cloudinary Image Test</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            max-width: 600px;
            margin: 50px auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background: white;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }}
        h1 {{
            color: #28a745;
            margin-bottom: 20px;
        }}
        .info {{
            margin: 15px 0;
            padding: 10px;
            background: #f0f0f0;
            border-left: 4px solid #28a745;
            border-radius: 4px;
        }}
        .info strong {{
            display: block;
            color: #333;
            margin-bottom: 5px;
        }}
        .info-value {{
            color: #666;
            word-break: break-all;
            font-size: 0.9em;
        }}
        img {{
            max-width: 100%;
            height: auto;
            border-radius: 8px;
            margin-top: 20px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }}
        .error {{
            color: #dc3545;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>✅ Cloudinary Image Test</h1>
        
        <div class='info'>
            <strong>Account Name:</strong>
            <div class='info-value'>{userName}</div>
        </div>

        <div class='info'>
            <strong>Role:</strong>
            <div class='info-value'>{user.Role}</div>
        </div>

        <div class='info'>
            <strong>Public ID (ImageUrl):</strong>
            <div class='info-value'>{imageUrl}</div>
        </div>

        <div class='info'>
            <strong>Cloudinary URL:</strong>
            <div class='info-value'>{finalImageUrl}</div>
        </div>

        <h2 style='margin-top: 30px; color: #333;'>Image from Cloudinary:</h2>
        <img src='{finalImageUrl}' alt='Profile Image from Cloudinary' onerror=""this.src='https://via.placeholder.com/300?text=Image+Not+Found'; this.style.border='2px solid #dc3545';"">
    </div>
</body>
</html>
";

                _logger.LogInformation("TestCloudinaryImage: Returning success response with image URL");
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TestCloudinaryImage: Error occurred. Message: {ErrorMessage}", ex.Message);
                return Content($"<h1>❌ Error: {ex.Message}</h1><p><strong>Stack Trace:</strong></p><pre>{ex.StackTrace}</pre>", "text/html");
            }
        }
    }
}
