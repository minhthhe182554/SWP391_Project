using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_Project.Helpers;
using SWP391_Project.Models;

namespace SWP391_Project.Controllers
{
    public class DataController : Controller
    {
        private readonly EzJobDbContext _context;
        private readonly ILogger<DataController> _logger;

        public DataController(EzJobDbContext context, ILogger<DataController> logger)
        {
            _context = context;
            _logger = logger;
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
    }
}
