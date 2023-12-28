using adAdgenstvo.Models.DataBaseModels;
using adAdgenstvo.Models.EditModels;
using adAdgenstvo.Models.LoginModels;
using adAdgenstvo.Models.RegisterModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace adAdgenstvo.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly MDBContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(MDBContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            var positions = await _context.Positions.ToListAsync();
            ViewBag.Positions = new SelectList(positions, "Id", "PositionName");

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(ClientRM client)
        {
            var positions = await _context.Positions.ToListAsync();
            ViewBag.Positions = new SelectList(positions, "Id", "PositionName");
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == client.Email))
                {
                    ModelState.AddModelError("Email", "Адрес почты уже занят");
                    return View(client);
                }
                if (!string.IsNullOrEmpty(client.Inn) && client.Inn != null && _context.Users.Any(u => u.Inn!=null && u.Inn == client.Inn))
                {
                    ModelState.AddModelError("Inn", "Инн не верный");
                    return View(client);
                }
                User user = new User(client);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
                if (userRole == null)
                {
                    user.Role = userRole;
                }
                    if (User.IsInRole("Agent") || User.IsInRole("Admin"))
                {
                    TempData["SuccessMessage"] = "Пользователь создан";
                    return View();
                }
                await Authenticate(user);
                return RedirectToAction("Index", "Home");
            }
            return View(client);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            TempData["ReturnUrl"] = Request.Headers["Referer"].ToString();
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserLM userLogin)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Email == userLogin.Email && u.Password == userLogin.Password);

                if (user != null)
                {
                    await Authenticate(user);
                    if (TempData.TryGetValue("ReturnUrl", out var returnUrl) && !string.IsNullOrEmpty(returnUrl.ToString()))
                    {
                        string returnUrlStr = returnUrl.ToString();
                        TempData.Remove("ReturnUrl");
                        if (!returnUrlStr.Contains("Login") && !returnUrlStr.Contains("Register"))
                        {
                            return Redirect(returnUrlStr);
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                TempData["ErrorMessage"] = "Ошибка ввода почты либо пароля";
            }
            return View(userLogin);
        }

        private async Task Authenticate(User user)
        {
            var idClaim = new Claim(ClaimTypes.NameIdentifier, $"{user.Id}");
            var nameClaim = new Claim(ClaimTypes.Name, user.Email);
            var roleClaim = new Claim(ClaimTypes.Role, user.Role.RoleName);
            var claimsIdentity = new ClaimsIdentity(new[] { idClaim, nameClaim, roleClaim }, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(5),
                IsPersistent = true,
                AllowRefresh = true
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (User.IsInRole("Client"))
            {
                return View(new UserEM(await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userIdClaim)));
            }
            var user = await _context.Users.Include(u => u.Role).Include(u => u.Position).FirstOrDefaultAsync(u => u.Id == id);
            var positions = await _context.Positions.ToListAsync();
            ViewBag.Positions = new SelectList(positions, "Id", "PositionName");

            if (user == null)
            {
                TempData["Error"] = "Пользователь не найден";
                return View();
            }

            if ((User.IsInRole("Agent") && (user.Position == null || user.Id == userIdClaim)) || User.IsInRole("Admin"))
            {
                UserEM editUser = new UserEM(user);
                return View(editUser);
            }

            TempData["Error"] = "Нету доступа";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEM editModel)
        {
            if (ModelState.IsValid)
            {
                User? user = await _context.Users.FindAsync(editModel.Id);
                user.UpdateUser(editModel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Запись успешно изменена";
                return RedirectToAction("Index");
            }
            return View(editModel);
        }

        [Authorize(Roles = "Admin, Agent")]
        public IActionResult Users()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Agent")]
        public async Task<IActionResult> UsersPartial(string? searchName = null, string? searchSurname = null,
    string? searchEmail = null, string? searchPhoneNumber = null, string? searchCompanyName = null, string? searchInn = null)
        {
            IQueryable<User> usersQuery = _context.Users.Include(u => u.Role);
            Console.WriteLine($"Is Admin: {User.IsInRole("Admin")}, Is Agent: {User.IsInRole("Agent")}");

            usersQuery = User.IsInRole("Admin")
                ? usersQuery.Where(u => u.PositionId != null)
                : usersQuery.Where(u => u.PositionId == null);

            if (!string.IsNullOrEmpty(searchName))
            {
                var lowerCaseSearchName = searchName.ToLower();
                usersQuery = usersQuery.Where(u => u.Name.ToLower().Contains(lowerCaseSearchName));
            }

            if (!string.IsNullOrEmpty(searchSurname))
            {
                var lowerCaseSearchSurname = searchSurname.ToLower();
                usersQuery = usersQuery.Where(u => u.Patronymic.ToLower().Contains(lowerCaseSearchSurname));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                var lowerCaseSearchEmail = searchEmail.ToLower();
                usersQuery = usersQuery.Where(u => u.Email.ToLower().Contains(lowerCaseSearchEmail));
            }

            if (!string.IsNullOrEmpty(searchPhoneNumber))
            {
                usersQuery = usersQuery.Where(u => u.PhoneNumber.Contains(searchPhoneNumber));
            }

            if (!string.IsNullOrEmpty(searchCompanyName))
            {
                var lowerCaseSearchCompanyName = searchCompanyName.ToLower();
                usersQuery = usersQuery.Where(u => u.NameCompany.ToLower().Contains(lowerCaseSearchCompanyName));
            }

            if (!string.IsNullOrEmpty(searchInn))
            {
                usersQuery = usersQuery.Where(u => u.Inn.Contains(searchInn));
            }

            var users = await usersQuery.ToListAsync();

            return PartialView("UsersPartial", users);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var userToDelete = await _context.Users.FindAsync(userId);

                if (userToDelete == null)
                {
                    return Json(new { success = false, message = "Пользователь не найден." });
                }

                _context.Users.Remove(userToDelete);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Пользователь успешно удален." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Ошибка при удалении пользователя: {ex.Message}" });
            }
        }
    }
}
