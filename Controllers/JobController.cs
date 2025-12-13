using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Services;
using SWP391_Project.ViewModels.Jobs;
using System.Threading.Tasks;

namespace SWP391_Project.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<IActionResult> Detail(int id)
        {
            var vm = await _jobService.GetJobDetailAsync(id);
            if (vm == null) return NotFound();

            return View(vm);
        }
    }
}

