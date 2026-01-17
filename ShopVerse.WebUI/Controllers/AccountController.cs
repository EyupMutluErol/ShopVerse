using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Models;
using ShopVerse.WebUI.Utils;


namespace ShopVerse.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    City = "Belirtilmedi"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Member"))
                    {
                        await _roleManager.CreateAsync(new AppRole { Name = "Member" });
                    }
                    await _userManager.AddToRoleAsync(user, "Member");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    var emailHelper = new EmailHelper();
                    string subject = "ShopVerse Hesap Onayı";
                    string body = $"<h3>Aramıza Hoş Geldin {user.Name}!</h3>" +
                                  $"<p>Kaydını tamamlamak ve giriş yapabilmek için lütfen aşağıdaki linke tıkla:</p>" +
                                  $"<a href='{confirmationLink}' style='background-color:#fd7e14; color:white; padding:10px 20px; text-decoration:none; border-radius:5px;'>Hesabımı Doğrula</a>";

                    emailHelper.SendEmail(user.Email, subject, body);

                    return View("RegisterConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                TempData["Success"] = "Hesabınız başarıyla doğrulandı. Artık giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            return View("Error");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                 
                    bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                    if (isAdmin)
                    {
                        var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                        if (passwordCheck.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, model.RememberMe);
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                        }
                    }
                    else
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

                        if (result.Succeeded)
                        {
                            return !string.IsNullOrEmpty(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction("Index", "Home");
                        }
                        else if (result.IsNotAllowed)
                        {
                            ModelState.AddModelError("", "Giriş yapabilmek için e-posta adresinizi doğrulamanız gerekmektedir.");
                            return View(model);
                        }
                    }
                }
                ModelState.AddModelError("", "E-posta veya şifre hatalı.");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return View("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { token = token, email = user.Email }, protocol: Request.Scheme);

            var emailHelper = new EmailHelper();
            string subject = "Şifre Sıfırlama Talebi";
            string body = $"<h3>Merhaba {user.Name},</h3>" +
                          $"<p>Şifrenizi sıfırlamak için lütfen aşağıdaki linke tıklayın:</p>" +
                          $"<a href='{callbackUrl}'>Şifremi Sıfırla</a>";

            emailHelper.SendEmail(user.Email, subject, body);

            return View("ForgotPasswordConfirmation");
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                TempData["Error"] = "Geçersiz şifre sıfırlama linki.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["Success"] = "Şifreniz başarıyla sıfırlandı. Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> FixAdmin()
        {
            var adminUser = await _userManager.FindByEmailAsync("admin@shopverse.com");

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = "admin@shopverse.com",
                    Email = "admin@shopverse.com",
                    Name = "ShopVerse",
                    Surname = "Admin",
                    City = "İstanbul",
                    EmailConfirmed = true 
                };

                var createResult = await _userManager.CreateAsync(adminUser, "Admin123.");
                if (!createResult.Succeeded)
                {
                    return Content("Admin oluşturulamadı: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(adminUser);
                await _userManager.ResetPasswordAsync(adminUser, token, "Admin123.");

                adminUser.EmailConfirmed = true;
                await _userManager.UpdateAsync(adminUser);
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new AppRole { Name = "Admin" });
            }

            if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            return Content("Admin kullanıcısı başarıyla onarıldı! \nEmail: admin@shopverse.com \nŞifre: Admin123. \nRol: Admin \n\nArtık giriş yapabilirsiniz.");
        }
    }
}