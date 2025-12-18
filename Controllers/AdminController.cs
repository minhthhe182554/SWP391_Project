using System;
using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Helpers;
using SWP391_Project.Models;
using SWP391_Project.Services;
using SWP391_Project.ViewModels;
using SWP391_Project.ViewModels.Admin;

namespace SWP391_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [RoleAuthorize(Role.ADMIN)]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(ManageUsers));
        }

        [RoleAuthorize(Role.ADMIN)]
        public async Task<IActionResult> ManageUsers(
            int candidateActivePage = 1,
            int candidateBannedPage = 1,
            int companyActivePage = 1,
            int companyBannedPage = 1,
            int pageSize = 10,
            int? focusUserId = null)
        {
            candidateActivePage = Math.Max(1, candidateActivePage);
            candidateBannedPage = Math.Max(1, candidateBannedPage);
            companyActivePage = Math.Max(1, companyActivePage);
            companyBannedPage = Math.Max(1, companyBannedPage);
            pageSize = Math.Max(1, pageSize);

            var vm = await _adminService.GetManageUsersAsync(
                candidateActivePage,
                candidateBannedPage,
                companyActivePage,
                companyBannedPage,
                pageSize,
                focusUserId);
            return View("ManageUsers", vm);
        }

        [RoleAuthorize(Role.ADMIN)]
        public async Task<IActionResult> ManageReports(int page = 1, int pageSize = 10, int? focusReportId = null)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var vm = await _adminService.GetManageReportsAsync(page, pageSize, focusReportId);
            return View("ManageReports", vm);
            }

        [RoleAuthorize(Role.ADMIN)]
        public async Task<IActionResult> ManageJobs(int page = 1, int pageSize = 10, string statusFilter = "all", int? focusJobId = null)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var vm = await _adminService.GetManageJobsAsync(page, pageSize, statusFilter, focusJobId);
            return View("ManageJobs", vm);
        }

        [HttpGet]
        [RoleAuthorize(Role.ADMIN)]
        public async Task<IActionResult> JobDetail(int id)
        {
            var vm = await _adminService.GetJobDetailForAdminAsync(id);
            if (vm == null) return NotFound();
            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Role.ADMIN)]
        public async Task<IActionResult> UpdateReportStatus(int id, ReportStatus status, string? adminNote)
        {
            await _adminService.UpdateReportStatusAsync(id, status, adminNote);
            return Json(new { success = true });
        }

        [HttpGet]
        [RoleAuthorize(Role.ADMIN)]
        public async Task<IActionResult> UserDetail(int id)
        {
            var detail = await _adminService.GetUserDetailAsync(id);
            if (detail == null) return NotFound();
            return Json(detail);
        }

        [HttpPost]
        [RoleAuthorize(Role.ADMIN)]
        public async Task<IActionResult> ToggleUserStatus(int id, bool active)
        {
            await _adminService.ToggleUserActiveAsync(id, active);
            return Json(new { success = true, active });
        }
    }
}
