using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP391_Project.Models;

namespace SWP391_Project.Controllers;

public class HomeController : Controller
{
    private readonly EzJobDbContext _context;

    public HomeController(EzJobDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Check if user is logged in
        var email = HttpContext.Session.GetString("Email");
        var role = HttpContext.Session.GetString("Role");
        
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(role))
        {
            // Redirect to role-specific home page
            if (role == "ADMIN")
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (role == "COMPANY")
            {
                return RedirectToAction("Index", "Company");
            }
            else if (role == "CANDIDATE")
            {
                return RedirectToAction("Index", "Candidate");
            }
        }
        
        // Guest user - show public job browsing page
        var jobs = _context.Jobs
            .Include(j => j.Company)
            .Include(j => j.Location)
            .Include(j => j.RequiredSkills)
            .Where(j => j.EndDate >= DateTime.Now && !j.IsDelete)
            .OrderByDescending(j => j.StartDate)
            .Take(20)
            .ToList();
            
        return View(jobs);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
