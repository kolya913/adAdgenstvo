using adAdgenstvo.Models;
using adAdgenstvo.Models.DataBaseModels;
using adAdgenstvo.Models.DataBaseModels;
using adAdgenstvo.Models.EditModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace adAdgenstvo.Controllers
{
    public class OrderController : Controller
    {
        private readonly MDBContext _context;

        public OrderController(MDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = _context.Order.Include(o => o.Agent).Include(o => o.Client);
            if (orders == null)
            {
                TempData["ErrorMessage"] = "Ничего не найдено";
                return View(null);
            }
            else 
            { 
                return View(await orders.ToListAsync());
            }
            
        }


        [HttpGet]
        public IActionResult LoadOrders(int? searchId, int? searchClientId, int? searchAgentId, string searchStatus, bool searchNullAgent)
        {
            
            if (User.IsInRole("Client"))
            {
                searchClientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            }
            else if (User.IsInRole("Agent"))
            {
                searchAgentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            }
            var orders = _context.Order.Include(o => o.Agent).Include(o => o.Client).AsQueryable();

            if (searchClientId.HasValue)
            {
                orders = orders.Where(o => o.ClientId == searchClientId);
            }


            if (searchId.HasValue)
            {
                orders = orders.Where(o => o.Id == searchId);
            }


            if (searchAgentId.HasValue)
            {
                if (searchNullAgent)
                {
                    orders = orders.Where(o => o.AgentId == null);
                }
                else
                {
                    orders = orders.Where(o => o.AgentId == searchAgentId);
                }
            }

            if (!string.IsNullOrEmpty(searchStatus))
            {
                orders = orders.Where(o => o.Status == searchStatus);
            }
            return PartialView("_OrderTablePartial", orders.ToList());
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Agent)
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (User.IsInRole("Client"))
            {
                id  = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            }
            else
            {
                if (!id.HasValue)
                {
                    TempData["ErrorMessages"] = "Id ошибка";
                    return View(null);
                }
            }
            ViewBag.ClientId = 1;
            List<PriceService> services = await _context.PriceService.ToListAsync();
            var model = new CreateRequisitionModel
            {
                TextField = "значение по умолчанию",
                Id = (int)id,
                Services = services
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateRequisitionModel model)
        {
            if (User.IsInRole("Client"))
            {
                int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if(id != model.Id)
                {
                    model.Id = id;
                }

            }

            if (ModelState.IsValid)
            {
                Order order = new Order();
                order.DateCreate = DateOnly.FromDateTime(DateTime.UtcNow);
                order.ClientId = (int)model.Id;
                order.Text = model.TextField;
                order.Status = "new";
                await _context.AddAsync(order);
                await _context.SaveChangesAsync();
                foreach (var selectedServiceId in model.SelectedServiceIds)
                {
                    Console.WriteLine(order.Id);
                    Console.WriteLine(selectedServiceId+"\n");
                    OrderServicePrice requisition = new OrderServicePrice();
                    requisition.OrderId = order.Id;
                    requisition.PriceServiceId = selectedServiceId;
                    await _context.OrderServicePrice.AddAsync(requisition);
                    await _context.SaveChangesAsync();
                }
            }
            model.Services = await _context.PriceService.ToListAsync();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                TempData["ErrorMessages"] = "Id ошибка";
                return View(null);
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                TempData["ErrorMessages"] = "Заказ не найден";
                return View(null);
            }
            ViewData["AgentId"] = new SelectList(_context.Users.Where(u => u.RoleId == 2).Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.Id} | {u.Name} {u.Lastname}"
            }), "Id", "FullName");
            OrderEM orderEM = new OrderEM();
            orderEM.Id = order.Id;
            orderEM.AgentId = order.AgentId;
            orderEM.Text = order.Text;
            return View(orderEM);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AgentId,Text")] OrderEM orderem)
        {
            if (id != orderem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var order = _context.Order.Find(orderem.Id);
                    order.AgentId = orderem.AgentId;
                    order.Text = orderem.Text;
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    ChangeStatusOrderVnyrt(order.Id, "workProject");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(orderem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AgentId"] = new SelectList(_context.Users.Where(u => u.RoleId == 2).Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.Id} | {u.Name} {u.Lastname}"
            }), "Id", "FullName");
            return View(orderem);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Agent)
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'MDBContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {

            var order = _context.Order.Find(orderId);
            int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (order == null || order.ClientId != id)
            {
                return BadRequest();
            }

            order.Status = "canceled";
            _context.SaveChanges();

            return Ok(new { message = "Заказ успешно отменен" });
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
                    var orderStatus = _context.Order.Find(updatedProject.OrderId);
                    if(orderStatus.Status == "payment")
                    {
                        ChangeStatusOrderVnyrt(updatedProject.OrderId, "run");
                    }
                    else
                    {
                        ChangeStatusOrderVnyrt(updatedProject.OrderId, "needAproved");
                    }
                    
                    return RedirectToAction("Index");
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
            ChangeStatusOrderVnyrt(updatedProject.OrderId, "needAproved");
            return RedirectToAction("Index");
        }
        public void ChangeStatusOrderVnyrt(int orderId, string status)
        {
            var order = _context.Order.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.SaveChanges();
            }
        }
        public IActionResult ShowLayout(int orderId, string? doing)
        {
            var layoutProject = _context.Projects
                .Include(lp => lp.Photos)
                .FirstOrDefault(lp => lp.OrderId == orderId);

            if (layoutProject == null)
            {
                return View(null);
            }
            if (doing != null)
                TempData["doing"] = doing;
            return View(layoutProject);
        }

        public IActionResult ChangeStatusOrder(int orderId, string status)
        {

            var order = _context.Order.Find(orderId);
            var contract = _context.Contract.FirstOrDefault(lp => lp.OrderId == orderId && status != "canceled" && status != "complete");

            if (order != null && contract != null)
            {
               
                contract.Status = status;
                order.Status = status;
                _context.SaveChanges();
            }
            else if (order != null && contract == null)
            {
                order.Status = status;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult CreateContract(int orderId)
        {
            var model = new Contract();
            model.OrderId = orderId;
            List<OrderServicePrice> paym =  _context.OrderServicePrice.Where(o => o.OrderId == orderId).Include(o => o.PriceService).ToList();
            foreach (var item in paym) 
            {
                model.Price += item.PriceService.Price;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateContract(Contract contract)
        {
            contract.DateStartCompany = ToUtcDateTime(contract.DateStartCompany);
            contract.DateEndCompany = ToUtcDateTime(contract.DateEndCompany);
            if (ModelState.IsValid)
            {
                contract.Status = "needContractSign";
                _context.Contract.Add(contract);
                _context.SaveChanges();

            }
            ChangeStatusOrderVnyrt(contract.OrderId, "needContractSign");
            return View(contract);
        }
        private DateTime ToUtcDateTime(DateTime dateTime)
        {
            DateTime utcDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return utcDateTime.ToUniversalTime();
        }
        public IActionResult ViewContract(int orderId)
        {
            var contract = _context.Contract.First(c => c.OrderId == orderId);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }


        public IActionResult PayContract(int orderId)
        {
            var contract = _context.Contract.FirstOrDefault(c => c.OrderId == orderId);
            if (contract != null)
            {
                ChangeStatusOrder(orderId, "payment");
                contract.Status = "payment";
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
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

        [HttpPost]
        public IActionResult Stop(int orderId)
        {
            var order = _context.Order.FirstOrDefault(o => o.Id == orderId);
            int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (order == null && (order.ClientId != id || order.AgentId != id ))
            {
                return BadRequest();
            }
            var contract = _context.Contract.FirstOrDefault(c => c.OrderId == orderId);
            if (contract != null)
            {
                contract.Status = "complete";
                order.Status = "complete";
            }
            else
            {
                return BadRequest();
            }
            _context.SaveChanges();
            return Ok(new { message = "Успешно завершено" });
        }

    }
}
