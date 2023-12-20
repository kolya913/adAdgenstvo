using adAdgenstvo.Models.CreateModels;
using adAdgenstvo.Models.DataBaseModels;
using adAdgenstvo.Models.EditModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace adAdgenstvo.Controllers
{
    [Authorize(Roles = "Agent")]
    public class ServiceController : Controller
    {
        private readonly MDBContext _context;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(MDBContext context, ILogger<ServiceController> logger)
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

        public IActionResult CreateServiceType()
        {
            ViewBag.Types = new List<string> { "FIZ", "WEB" };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateServiceType(ServiceTypeCM model)
        {
            ViewBag.Types = new List<string> { "FIZ", "WEB" };
            if (ModelState.IsValid)
            {
                ServiceType newServiceType = new ServiceType(model);
                await _context.ServiceTypes.AddAsync(newServiceType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Успешно создано";
                return View();
            }
            TempData["ErrorMessage"] = "Ошибка создания";
            return View(model);
        }

        public async Task<IActionResult> EditTypeServices(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Не найдено";
                return View();
            }

            ServiceType typeService = await _context.ServiceTypes.FindAsync(id);
            if (typeService == null)
            {
                TempData["ErrorMessage"] = "Не найдено";
                return NotFound();
            }
            ServiceTypeEM serviceTypeEM = new ServiceTypeEM(typeService);
            var check = await _context.Services.Where(s => s.ServiceTypeId == id).ToListAsync();
            if (check != null && check.Any())
            {
                ViewBag.Check = serviceTypeEM.Type;
            }
            else
            {
                ViewBag.Check = null;
            }

            return View(serviceTypeEM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTypeServices(ServiceTypeEM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServiceType typeService = await _context.ServiceTypes.FindAsync(model.Id);

                    if (typeService == null)
                    {
                        TempData["ErrorMessage"] = "Категория не найдена";
                        return RedirectToAction(nameof(Index));
                    }

                    typeService.Update(model);
                    _context.Update(typeService);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Успешно изменено";
                    return View(model);
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["ErrorMessage"] = "Ошибка при сохранении изменений. Попробуйте еще раз.";
                    return View(model);
                }
            }
            TempData["ErrorMessage"] = "Не правильный ввод данных";
            return View(model);
        }


        private bool TypeServiceExists(int id)
        {
            return _context.ServiceTypes.Any(e => e.Id == id);
        }

        public async Task<IActionResult> CreateService(int typeId)
        {
            if (await _context.ServiceTypes.FindAsync(typeId) == null)
            {
                TempData["ErrorMessage"] = "Не правильная категория услуги";
                return View();
            }
            ViewBag.TypeId = typeId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService([Bind("Name,ShortDescription,ServiceTypeId")] ServiceCM serviceCM)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Успешно изменено";
                ViewBag.TypeId = serviceCM.ServiceTypeId;
                await _context.Services.AddAsync(new(serviceCM));
                await _context.SaveChangesAsync();
                return View();
            }
            ViewBag.TypeId = serviceCM.ServiceTypeId;
            TempData["ErrorMessage"] = "Не правильный ввод данных";
            return View(serviceCM);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTypeServices(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Категория не найдена" });
            }

            Service typeService = await _context.Services.Where(t => t.ServiceTypeId == id).FirstOrDefaultAsync(m => m.Id == id);

            if (typeService != null)
            {
                return Json(new { success = false, message = "Невозможно удалить категорию, которая содержит услуги." });
            }

            ServiceType removeTypeService = await _context.ServiceTypes.FirstOrDefaultAsync(m => m.Id == id);
            _context.ServiceTypes.Remove(removeTypeService);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Успешно удалено" });
        }

        public async Task<IActionResult> EditService(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Услуга не найдена";
                return View();
            }
            ServiceEM serviceEM = new(await _context.Services.Include(ser => ser.ServiceType).FirstOrDefaultAsync(ser => ser.Id == id)); 
            if (serviceEM == null)
            {
                TempData["ErrorMessage"] = "Услуга не найдена";
                return View();
            }
            return View(serviceEM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, [Bind("Id,Name,ShortDescription,ServiceTypeId,ServiceType")] ServiceEM service)
        {
            if (id != service.Id)
            {
                TempData["ErrorMessage"] = "Ошибка";
            }
            if (ModelState.IsValid)
            {
                Service serviceToUpdate = await _context.Services.Include(ser => ser.ServiceType).FirstOrDefaultAsync(ser => ser.Id == id);
                serviceToUpdate.Update(service);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Успешно изменено";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка ввода";
            }
            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                Service serviceToDelete = await _context.Services.FindAsync(id);

                if (serviceToDelete == null)
                {
                    return Json(new { success = false, message = "Услуга не найдена" });
                }

                _context.Services.Remove(serviceToDelete);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Успешно удалено" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ошибка при удалении услуги" });
            }
        }

       
    }
}
