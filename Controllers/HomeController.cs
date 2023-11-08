using adAdgenstvo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            return View();
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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var worker = _context.Worker.Include(w => w.Role).FirstOrDefault(w => w.Login == model.Login && w.Password == model.Password);

                if (worker != null)
                {
                    await Authenticate(worker, worker.Role.Name);
                    return RedirectToAction("Index", "Home");
                }

                var client = _context.Clients.Include(c => c.Role).FirstOrDefault(c => c.Login == model.Login && c.Password == model.Password);
                if (client != null)
                {
                    await Authenticate(client, client.Role.Name);
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ErrorMessage = "Неверный логин или пароль";
            }
            else
            {
                ViewBag.ErrorMessage = "Пожалуйста, введите правильные данные.";
            }
            return View("Login", model);
        }


        private async Task Authenticate(IUser user, string role)
        {
            var idClaim = new Claim(ClaimTypes.NameIdentifier, $"{user.Id}_{role}");
            var nameClaim = new Claim(ClaimTypes.Name, user.Login);
            var roleClaim = new Claim(ClaimTypes.Role, role);

            var claimsIdentity = new ClaimsIdentity(new[] { idClaim, nameClaim, roleClaim }, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }



        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(ClientRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Clients.AnyAsync(c => c.Login == model.Login))
                {
                    ModelState.AddModelError(string.Empty, "Пользователь с таким логином уже существует");
                    return View(model);
                }

                var clientRole = await _context.Role.FirstOrDefaultAsync(r => r.Name == "client");

                if (clientRole == null)
                {
                    ModelState.AddModelError(string.Empty, "Роль 'client' не найдена в базе данных");
                    return View(model);
                }

                var client = new Client
                {
                    Name = model.Name,
                    Patronymic = model.Patronymic,
                    Surname = model.Surname,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    NameCompany = model.NameCompany,
                    Login = model.Login,
                    Inn = model.Inn,
                    Email = model.Email,
                    IdRole = clientRole.Id,
                    Role = clientRole
                };

                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();
                await Authenticate(client, client.Role.Name);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }

    }
}