using adAdgenstvo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using adAdgenstvo.Models.DataBaseModels;

namespace adAdgenstvo.Controllers
{
    public class HomeController : Controller
    {

        private readonly MDBContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MDBContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<ServiceType> typeServices = await _context.ServiceTypes.ToListAsync();
            List<Service> services = await _context.Services.ToListAsync();
            var groupedServices = services.GroupBy(s => s.ServiceTypeId).ToDictionary(g => g.Key, g => g.ToList());
            var model = new Tuple<List<ServiceType>, List<Service>>(typeServices, services);
            return View(model);
        }

        public async Task<IActionResult> Price(int id)
        {
            var service = await _context.PriceService
                .Include(s => s.Service)
                .Where(s => s.ServiceId == id).ToListAsync();

            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}