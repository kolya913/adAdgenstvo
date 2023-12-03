using adAdgenstvo.Models.CreateModels;
using adAdgenstvo.Models.DataBaseModels;
using adAdgenstvo.Models.EditModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace adAdgenstvo.Controllers
{
    [Authorize]
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
            var typeServices = await _context.ServiceTypes.ToListAsync();
            var services =  await _context.Services.ToListAsync();
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
            ServiceTypeEM serviceTypeEM = new ServiceTypeEM(typeService);
            if (typeService == null)
            {
                TempData["ErrorMessage"] = "Не найдено";
                return NotFound();
            }
            var check = await _context.Services.Where(s => s.ServiceTypeId == id).ToListAsync();
            if (check != null && check.Any())
            {
                ViewBag.Check = typeService.ShortName;
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

        public async Task<IActionResult> CreateServiceAsync(int typeId)
        {
            if(await _context.ServiceTypes.FindAsync(typeId) == null)
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
            TempData["ErrorMessage"] = "Не правильный ввод данных";
            return View(serviceCM);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTypeServices(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Категория не найдена";
                return RedirectToAction("Index");
            }

            var typeService = await _context.Services
                .Where(t => t.ServiceTypeId == id)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (typeService != null)
            {
                TempData["ErrorMessage"] = "Невозможно удалить категорию, которая содержит услуги.";
                return RedirectToAction("Index");
            }
            var removeTypeService = await _context.ServiceTypes.FirstOrDefaultAsync(m => m.Id == id);
            _context.ServiceTypes.Remove(removeTypeService);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Успешно удалено";
            return RedirectToAction("Index");
        }

        /* public IActionResult ServicesType()
         {
             var typeServices = _context.TypeServices
                 .OrderBy(t => t.Category)
                 .ToList();

             var services = _context.Services
                 .Include(s => s.TypeService)
                 .Include(s => s.Price)
                 .ToList();

             var model = new Tuple<List<TypeService>, List<Service>>(typeServices, services);

             return View(model);
         }


         public IActionResult CreateTypeServices()
         {
             return View();
         }

         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> CreateTypeServices([Bind("Id,ShortDiscrp,Category,Type")] TypeService typeService)
         {
             if (ModelState.IsValid)
             {
                 _context.Add(typeService);
                 await _context.SaveChangesAsync();
                 return RedirectToAction("ServicesType");
             }
             return View(typeService);
         }



         public async Task<IActionResult> EditTypeServices(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var typeService = await _context.TypeServices.FindAsync(id);
             if (typeService == null)
             {
                 return NotFound();
             }
             return View(typeService);
         }

         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> EditTypeServices(int id, [Bind("Id,ShortDiscrp,Category,Type")] TypeService typeService)
         {
             if (id != typeService.Id)
             {
                 return NotFound();
             }

             if (ModelState.IsValid)
             {
                 try
                 {
                     _context.Update(typeService);
                     await _context.SaveChangesAsync();
                 }
                 catch (DbUpdateConcurrencyException)
                 {
                     if (!TypeServiceExists(typeService.Id))
                     {
                         return NotFound();
                     }
                     else
                     {
                         throw;
                     }
                 }
                 return RedirectToAction("ServicesType");
             }
             return View(typeService);
         }

         private bool TypeServiceExists(int id)
         {
             return _context.TypeServices.Any(e => e.Id == id);
         }


         public async Task<IActionResult> DetailsService(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var typeService = await _context.TypeServices
                 //.Include(t => t.Service)
                 .FirstOrDefaultAsync(m => m.Id == id);

             if (typeService == null)
             {
                 return NotFound();
             }

             return View(typeService); /////!!!!!!!!!!!!! реализогвать
         }

         public async Task<IActionResult> DeleteTypeServices(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var typeService = await _context.Services
                 .Where(t => t.TypeId == id)
                 .FirstOrDefaultAsync(m => m.Id == id);

             if (typeService != null)
             {
                 TempData["ErrorMessage"] = "Невозможно удалить категорию, которая содержит услуги.";
                 return RedirectToAction("ServicesType");
             }
             var removeTypeService = await _context.TypeServices.FirstOrDefaultAsync(m => m.Id == id);
             _context.TypeServices.Remove(removeTypeService);
             await _context.SaveChangesAsync();

             return RedirectToAction("ServicesType");
         }


         public async Task<IActionResult> EditService(int? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var ser = await _context.Services.Include(se => se.TypeService).FirstOrDefaultAsync(se => se.Id == id);

             var service = await _context.Services
                 .Include(s => s.TypeService)
                 .Include(s => s.Price)
                 .FirstOrDefaultAsync(m => m.Id == id);

             if (service == null)
             {
                 return NotFound();
             }

             ViewBag.Types = _context.TypeServices.Where(tp => tp.Type == ser.TypeService.Type).ToList();
             return View(service);
         }

         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> EditService(int id, [Bind("Id,TypeService,ShortDiscrp,Discription,TypeId,Price")] Service service)
         {
             //service.TypeService = _context.TypeServices.FirstOrDefault(m => m.Id == service.TypeId);
             if (id != service.Id)
             {
                 return NotFound();
             }

             // if (ModelState.IsValid)
             //{
             try
             {
                 _context.Update(service);
                 await _context.SaveChangesAsync();
             }
             catch (DbUpdateConcurrencyException)
             {
                 if (!ServiceExists(service.Id))
                 {
                     return NotFound();
                 }
                 else
                 {
                     throw;
                 }
             }

             ViewBag.Types = _context.TypeServices.ToList();
             return RedirectToAction("ServicesType");
             //}

             return View(service);
         }


         private bool ServiceExists(int id)
         {
             return _context.Services.Any(e => e.Id == id);
         }



         public IActionResult CreateService(int typeId, string categoryName)
         {
             ViewBag.Types = _context.TypeServices.SingleOrDefault(TS => TS.Category == categoryName);
             return View();
         }


         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> CreateService(ServiceCreationModel model)
         {
             if (ModelState.IsValid)
             {
                 var service = new Service
                 {
                     ShortDiscrp = model.ShortDiscrp,
                     Discription = model.Discription,
                     TypeId = model.TypeId,
                     Price = new Price
                     {
                         Cost = model.Cost,
                         Count = model.Count,
                         Size = model.Size
                     }
                 };

                 _context.Add(service);
                 await _context.SaveChangesAsync();
                 return RedirectToAction("ServicesType");
             }

             ViewBag.Types = _context.TypeServices.ToList();
             return View(model);
         }


         [HttpDelete]
         public IActionResult DeleteService(int id)
         {
             var service = _context.Services.Find(id);

             if (service == null)
             {
                 return NotFound();
             }

             _context.Services.Remove(service);
             _context.SaveChanges();

             return Ok();
         }


         public IActionResult EditLayout(int orderId)
         {
             var existingProject = _context.Projects.Include(lp => lp.Photos).FirstOrDefault(lp => lp.OrderId == orderId);
             if (existingProject != null)
             {
                 return View(existingProject);
             }
             LayoutProject newProject = new LayoutProject
             {
                 OrderId = orderId
             };
             return View(newProject);
         }

         [HttpPost]
         public IActionResult EditLayout(LayoutProject updatedProject, List<IFormFile> files)
         {
             if (ModelState.IsValid)
             {
                 var existingProject = _context.Projects.Include(lp => lp.Photos).FirstOrDefault(lp => lp.Id == updatedProject.Id);
                 if (existingProject != null)
                 {
                     existingProject.Text = updatedProject.Text;
                     foreach (var file in files)
                     {
                         var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/media/{existingProject.Id}/", file.FileName);
                         Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                         using (var stream = new FileStream(filePath, FileMode.Create))
                         {
                             file.CopyTo(stream);
                         }
                         var newPhoto = new LayoutPhoto
                         {
                             Title = file.FileName,
                             Path = $"media/{existingProject.Id}/{file.FileName}",
                             ProjectId = existingProject.Id
                         };
                         existingProject.Photos.Add(newPhoto);
                     }
                     _context.SaveChanges();
                     return RedirectToAction("ShowLayout", new { id = existingProject.OrderId });
                 }
                 else
                 {
                     _context.Projects.Add(updatedProject);
                     _context.SaveChanges();
                     foreach (var file in files)
                     {
                         var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/media/{updatedProject.Id}/", file.FileName);
                         Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                         using (var stream = new FileStream(filePath, FileMode.Create))
                         {
                             file.CopyTo(stream);
                         }
                         var newPhoto = new LayoutPhoto
                         {
                             Title = file.FileName,
                             Path = $"media/{updatedProject.Id}/{file.FileName}",
                             ProjectId = updatedProject.Id
                         };
                         _context.Photos.Add(newPhoto);
                     }
                     _context.SaveChanges();
                 }
             }

             return View(updatedProject);
         }

         [HttpPost]
         public IActionResult DeletePhoto(int photoId)
         {
             var photoToDelete = _context.Photos.Find(photoId);
             if (photoToDelete != null)
             {
                 var project = _context.Projects.Include(p => p.Photos).FirstOrDefault(p => p.Id == photoToDelete.ProjectId);
                 project?.Photos.Remove(photoToDelete);
                 _context.Photos.Remove(photoToDelete);
                 _context.SaveChanges();
                 var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{photoToDelete.Path}");
                 if (System.IO.File.Exists(filePath))
                 {
                     System.IO.File.Delete(filePath);
                 }
                 return Ok();
             }

             return NotFound();
         }

         public IActionResult AssignUrl(int serviceId)
         {
             var availableUrls = _context.Webs
                 .Where(w => (w.Status == true) && (w.ServiceId == null || w.ServiceId == serviceId))
                 .ToList();

             var viewModel = new Tuple<int, List<Web>>(serviceId, availableUrls);
             return View(viewModel);
         }

         [HttpPost]
         public IActionResult AssignUrlToService(int urlId, int serviceId)
         {
             var service = _context.Services.Find(serviceId);
             var url = _context.Webs.Find(urlId);
             if (service != null && url != null)
             {
                 url.Service = service;
                 _context.SaveChanges();
                 return Ok();
             }
             return BadRequest();
         }

         public IActionResult Unassign(int serviceId, int urlId)
         {
             try
             {
                 var service = _context.Services.Find(serviceId);
                 var url = _context.Webs.Find(urlId);

                 if (service == null || url == null)
                 {
                     TempData["ErrorMessage"] = "Invalid service or URL.";
                     return RedirectToAction("AssignUrl", new { serviceId = serviceId });
                 }
                 url.ServiceId = null;
                 _context.SaveChanges();

                 TempData["SuccessMessage"] = "URL unassigned successfully.";
             }
             catch (Exception ex)
             {
                 TempData["ErrorMessage"] = "Error unassigning URL: " + ex.Message;
             }
             return RedirectToAction("AssignUrl", new { serviceId = serviceId });
         }

         public IActionResult AssignAddress(int id)
         {
             try
             {
                 var service = _context.Services.Find(id);

                 if (service == null)
                 {
                     TempData["ErrorMessage"] = "Услуга не найдена";
                     return View(null);
                 }

                 var availableAddresses = _context.Addresses
                     .Where(a => a.Status == "Available" && (a.ServiceId == null || a.ServiceId == id))
                     .ToList();

                 if (availableAddresses == null || availableAddresses.Count == 0)
                 {
                     TempData["ErrorMessage"] = "Адреса не найдены";
                     return View(availableAddresses);
                 }

                 var viewModel = new Tuple<int, List<Address>>(id, availableAddresses);
                 return View(viewModel);
             }
             catch (Exception ex)
             {
                 TempData["ErrorMessage"] = "Адрес не добавлен. Ошибка на сервере";
                 return View(null);
             }
         }


         public IActionResult AssignAddressToService(int serviceId, int addressId)
         {
             try
             {
                 var service = _context.Services.Find(serviceId);

                 if (service == null)
                 {
                     TempData["ErrorMessage"] = "Услуга не найдена";
                     return View("AssignAddress");
                 }

                 var address = _context.Addresses.Find(addressId);

                 if (address == null)
                 {
                     TempData["ErrorMessage"] = "Адрес не найден";
                     return View("AssignAddress");
                 }

                 if (address.ServiceId != null)
                 {
                     TempData["ErrorMessage"] = "Адрес уже назначен другой услуге";
                     return View("AssignAddress");
                 }

                 address.ServiceId = serviceId;
                 _context.SaveChanges();

                 TempData["SuccessMessage"] = "Адрес успешно назначен услуге";
                 return View("AssignAddress");
             }
             catch (Exception ex)
             {
                 TempData["ErrorMessage"] = "Адрес не добавлен. Ошибка на сервере";
                 return View("AssignAddress");
             }
         }

         public IActionResult UnassignAddress(int serviceId, int addressId)
         {
             try
             {
                 TempData.Clear();

                 var service = _context.Services.Find(serviceId);
                 var address = _context.Addresses.Find(addressId);

                 if (service != null && address != null && address.Service == service)
                 {
                     address.ServiceId = null;

                     _context.SaveChanges();
                     TempData["SuccessMessage"] = "Адрес успешно добавлен";
                 }
                 else
                 {
                     TempData["ErrorMessage"] = "Адрес не добавлен";
                 }
             }
             catch (Exception ex)
             {
                 TempData["ErrorMessage"] = "Адрес не добавлен. Ошибка на сервере";
             }
             return RedirectToAction("AssignAddress", new { serviceId = serviceId });
         }*/


    }
}
