using Final_Grp6_PROG3340_UI.Models.ViewModels;
using Final_Grp6_PROG3340_UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
		public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
		{
			if (!ModelState.IsValid)
				return View(model);

			var (success, token, error) = await _authService.LoginAsync(model.Username, model.Password);

			if (!success || string.IsNullOrEmpty(token))
			{
				ModelState.AddModelError(string.Empty, error ?? "Invalid login");
				return View(model);
			}

			// Store JWT in HttpOnly cookie
			Response.Cookies.Append("AuthToken", token, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTimeOffset.UtcNow.AddDays(7) // match backend
			});

			if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);

			return RedirectToAction("Board", "Tasks");
		}

		[HttpGet]
		public IActionResult Register() => View();

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var (success, error) = await _authService.RegisterAsync(model);

			if (!success)
			{
				ModelState.AddModelError(string.Empty, error ?? "Registration failed");
				return View(model);
			}

			TempData["SuccessMessage"] = "Account created. Login now.";
			return RedirectToAction(nameof(Login));
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
			var redirectUrl = Url.Action("GoogleCallback", "Auth");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
		}

		[HttpGet]
		public async Task<IActionResult> GoogleCallback()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			
			if (!result.Succeeded)
			{
				TempData["ErrorMessage"] = "Google authentication failed.";
				return RedirectToAction("Login");
			}

			var tokens = result.Properties?.GetTokens();
			if (tokens != null)
			{
				foreach (var t in tokens)
				{
					_logger.LogInformation($"{t.Name} => {t.Value}");
				}
			}

			var idToken = result.Properties?.GetTokenValue("id_token");

			if (idToken == null)
			{
				TempData["ErrorMessage"] = "Could not retrieve Google ID token.";
				return RedirectToAction("Login");
			}

			// SEND GOOGLE ID TOKEN → API BACKEND
			var (success, jwt, errorMessage) = await _authService.GoogleLoginAsync(idToken);

			if (!success || string.IsNullOrEmpty(jwt))
			{
				TempData["ErrorMessage"] = errorMessage ?? "Google login failed.";
				return RedirectToAction("Login");
			}

			// Store backend JWT
			Response.Cookies.Append("AuthToken", jwt, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTimeOffset.UtcNow.AddHours(1)
			});

			return RedirectToAction("Board", "Tasks");
		}
	}
}
