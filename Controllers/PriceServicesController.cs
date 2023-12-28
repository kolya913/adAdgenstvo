using adAdgenstvo.Models.DataBaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace adAdgenstvo.Controllers
{
    [Authorize(Roles ="Agent")]
    public class PriceServicesController : Controller
    {
        private readonly MDBContext _context;

        public PriceServicesController(MDBContext context)
        {
            _context = context;
        }


        public IActionResult Index(int? id)
        {
            ViewData["CurrentId"] = id;

            
            return View();
        }



        public IActionResult SearchResults(string? priceServiceName, string? serviceName, int? priceServiceId, int? id)
        {
            var query = _context.PriceService.Include(p => p.Service).AsQueryable();

            if (!string.IsNullOrEmpty(priceServiceName))
            {
                query = query.Where(p => p.Name.ToLower().Contains(priceServiceName.ToLower()));
            }

            if (!string.IsNullOrEmpty(serviceName))
            {
                query = query.Where(p => p.Service.Name.ToLower().Contains(serviceName.ToLower()));
            }

            if (priceServiceId.HasValue)
            {
                query = query.Where(p => p.Id == priceServiceId.Value);
            }

            if (id.HasValue)
            {
                query = query.Where(p => p.ServiceId == id.Value);
            }

            var results = query.ToList();

            return PartialView("_PriceTable", results);
        }





        public IActionResult Create()
        {
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Count,TimeToProduce,ServiceId")] PriceService priceService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(priceService);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Успешно создано";
                ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
                return View(null);
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", priceService.ServiceId);
            TempData["ErrorMessage"] = "Ошибка ввода данных";
            return View(priceService);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Некорректный id";
                return View();
            }

            var priceService = await _context.PriceService.FindAsync(id);
            if (priceService == null)
            {
                TempData["ErrorMessage"] = "Цена не найдена";
                return View();
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", priceService.ServiceId);
            return View(priceService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Count,TimeToProduce,ServiceId")] PriceService priceService)
        {
            if (id != priceService.Id)
            {
                TempData["ErrorMessage"] = "Некорректный id";
                return View();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priceService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceServiceExists(priceService.Id))
                    {
                        TempData["ErrorMessage"] = "Ошибка ввода";
                        return View();
                    }
                }
                TempData["SuccessMessage"] = "Данные успешно изменены";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка ввода";
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", priceService.ServiceId);
            return View(priceService);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (_context.PriceService == null)
                {
                    return Problem("Entity set 'MDBContext.PriceService' is null.");
                }

                var priceService = await _context.PriceService.FindAsync(id);
                if (priceService != null)
                {
                    _context.PriceService.Remove(priceService);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Успешно удалено" });
                }

                return Json(new { success = false, message = "Цена не найдена" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении цены: {ex.Message}");
                return Json(new { success = false, message = $"Ошибка при удалении цены: {ex.Message}" });
            }
        }

        private bool PriceServiceExists(int id)
        {
            return (_context.PriceService?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
