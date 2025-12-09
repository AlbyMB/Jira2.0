using Final_Grp6_PROG3340_UI.Models.ViewModels;
using Final_Grp6_PROG3340_UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Final_Grp6_PROG3340_UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthApiService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthApiService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, token, errorMessage) = await _authService.LoginAsync(model.Username, model.Password);

            if (success && !string.IsNullOrEmpty(token))
            {
                // Store JWT in cookie
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Board", "Tasks");
            }

            ModelState.AddModelError(string.Empty, errorMessage ?? "Invalid login attempt");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage) = await _authService.RegisterAsync(model);

            if (success)
            {
                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError(string.Empty, errorMessage ?? "Registration failed");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            // Redirect to Google OAuth using the configured OpenID Connect
            var redirectUrl = Url.Action("GoogleCallback", "Auth");
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> GoogleCallback()
        {
            // Authenticate the user from the Google OAuth response
            var authenticateResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Google authentication failed";
                return RedirectToAction(nameof(Login));
            }

            // Extract user information from the authenticated result
            var email = authenticateResult.Principal?.FindFirst("email")?.Value;
            var name = authenticateResult.Principal?.FindFirst("name")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Could not retrieve email from Google";
                return RedirectToAction(nameof(Login));
            }

            // TODO: Call backend /api/auth/google with the Google ID token
            // For now, we'll just sign in using the existing cookie authentication
            // You should implement a call to your backend API here:
            /*
            var idToken = authenticateResult.Properties?.GetTokenValue("id_token");
            var response = await _authService.GoogleLoginAsync(idToken);
            if (response.Success)
            {
                Response.Cookies.Append("AuthToken", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });
                return RedirectToAction("Board", "Tasks");
            }
            */

            _logger.LogInformation("Google login successful for user: {Email}", email);
            return RedirectToAction("Board", "Tasks");
        }
    }
}
