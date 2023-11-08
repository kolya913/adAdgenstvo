using adAdgenstvo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace adAdgenstvo.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        private readonly MDBContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(MDBContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        private async Task<object> GetUserProfileAsync(int userId, string role)
        {
#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
            return role switch
            {
                "client" => await _context.Clients.Include(c => c.Role).FirstOrDefaultAsync(c => c.Id == userId),
                "agent" => await _context.Worker.Include(w => w.Role).FirstOrDefaultAsync(w => w.Id == userId),
                "admin" => await _context.Worker.Include(w => w.Role).FirstOrDefaultAsync(w => w.Id == userId),
                _ => null
            };
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
        }

        public async Task<IActionResult> ViewProfile(int id, string role)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userIdAndRole = userIdClaim.Value.Split('_');
            var userId = int.Parse(userIdAndRole[0]);
            var userRole = userIdAndRole[1];

            var userOutput = userRole switch
            {
                "client" => await GetUserProfileAsync(userId, userRole),
                "agent" => await (role == "client" ? GetUserProfileAsync(id, role) : GetUserProfileAsync(userId, userRole)),
                "admin" => await GetUserProfileAsync(userId, userRole)
            };


            if (userOutput == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь с указанным Id не найден.");
                return View();
            }

            return userRole switch
            {
                "client" => View("ClientProfile", userOutput),
                "agent" => (role == "client" ? View("ClientProfile", userOutput) : View("WorkerProfile", userOutput)),
                "admin" => (role == "client" ? View("ClientProfile", userOutput) : View("WorkerProfile", userOutput)),
                _ => View()
            };
        }

        public async Task<IActionResult> EditProfile(int id, string role)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userIdAndRole = userIdClaim.Value.Split('_');
            var userId = int.Parse(userIdAndRole[0]);
            var userRole = userIdAndRole[1];

            var userOutput = userRole switch
            {
                "client" => await GetUserProfileAsync(userId, userRole),
                "agent" => await (role == "client" ? GetUserProfileAsync(id, role) : GetUserProfileAsync(userId, userRole)),
                "admin" => await GetUserProfileAsync(userId, userRole)
            };


            if (userOutput == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь с указанным Id не найден.");
                return View("ClientProfile");
            }

            return userRole switch
            {
                "client" => View("EditClientProfile", userOutput),
                "agent" => (role == "client" ? View("EditClientProfile", userOutput) : View("EditWorkerProfile", userOutput)),
                "admin" => (role == "client" ? View("EditClientProfile", userOutput) : View("EditWorkerProfile", userOutput)),
                _ => View("ClientProfile")
            };
        }

        [HttpPost]
        public async Task<IActionResult> EditClientProfile(Client editedClient)
        {
            var existingClient = await _context.Clients.FindAsync(editedClient.Id);
            if (existingClient == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(editedClient.Password))
            {
                existingClient.Password = editedClient.Password;
            }

            existingClient.Name = editedClient.Name;
            existingClient.Patronymic = editedClient.Patronymic;
            existingClient.Surname = editedClient.Surname;
            existingClient.PhoneNumber = editedClient.PhoneNumber;
            existingClient.NameCompany = editedClient.NameCompany;
            existingClient.Login = editedClient.Login;
            existingClient.Inn = editedClient.Inn;
            existingClient.Email = editedClient.Email;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditWorkerProfile(Worker model)
        {
            var existingWorker = await _context.Worker.FirstOrDefaultAsync(w => w.Id == model.Id);
            if (existingWorker != null)
            {
                existingWorker.Name = model.Name;
                existingWorker.Patronymic = model.Patronymic;
                existingWorker.Surname = model.Surname;
                existingWorker.PhoneNumber = model.PhoneNumber;
                await _context.SaveChangesAsync();
            }

            //return RedirectToAction("Index");
            return View("EditWorkerProfile", model);
        }

        public async Task<IActionResult> ViewOrders(int id, string role)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userIdAndRole = userIdClaim.Value.Split('_');
            var userId = int.Parse(userIdAndRole[0]);
            var userRole = userIdAndRole[1];

            if (userRole == "client")
            {
                var orders = await _context.Orders
                    .Include(o => o.Client)
                    .Include(o => o.Agent)
                    .Where(o => o.ClientId == userId && _context.Contracts.Any(c => c.OrderId == o.Id))
                    .ToListAsync();

                return View(orders);
            }

            return View();
        }

        public async Task<IActionResult> Requisition(int? id, string role)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userIdAndRole = userIdClaim.Value.Split('_');
            var userId = int.Parse(userIdAndRole[0]);
            var userRole = userIdAndRole[1];

            if (userRole == "client")
            {
                var orders = await _context.Orders
                    .Include(o => o.Client)
                    .Include(o => o.Agent)
                    .Where(o => o.ClientId == userId && !_context.Contracts.Any(c => c.OrderId == o.Id))
                    .ToListAsync();

                return View(orders);
            }

            return View();
        }


        public IActionResult CreateRequisition()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequisition(CreateRequisitionModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userIdAndRole = userIdClaim.Value.Split('_');
            var userId = int.Parse(userIdAndRole[0]);
            var userRole = userIdAndRole[1];

            if (ModelState.IsValid)
            {
                var utcDeadline = DateTime.UtcNow.AddDays(8);

                var order = new Order
                {
                    Deadline = utcDeadline,
                    ClientId = userId,
                    Status = "Pending",
                    Text = model.Text,
                    AgentId = null
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    var orderService = new OrderService
                    {
                        ServiceId = serviceId,
                        OrderId = order.Id,
                        IdentifierAd = null,
                        Status = "Pending"
                    };

                    _context.OrderServices.Add(orderService);
                }

                await _context.SaveChangesAsync();
            }
            var services = await _context.Services.ToListAsync();
            return View("CreateRequisition", services);
        }

        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userIdAndRole = userIdClaim.Value.Split('_');
            var userId = int.Parse(userIdAndRole[0]);
            var userRole = userIdAndRole[1];
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                if (order.ClientId == userId)
                {
                    order.Status = "canceled";
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction("ViewOrders");
        }


        public async Task<IActionResult> DebateOrder(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> DebateOrder(int orderId, string debateText)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                order.DebateText = debateText;
                order.Status = "debate";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewOrders");
        }



    }
}