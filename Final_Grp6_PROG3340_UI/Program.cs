using Final_Grp6_PROG3340_UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("API", client =>
{
	client.BaseAddress = new Uri("https://localhost:7111/api/"); // your API's base URL
});


builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
	})
	.AddCookie(options =>
	{
		options.Cookie.SameSite = SameSiteMode.None;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		options.LoginPath = "/Auth/Login";
		options.LogoutPath = "/Auth/Logout";
	})
	.AddOpenIdConnect(options =>
	{
		options.ClientId = "24183777669-dchkr9lco9rhk04v4pa78dvrcf00oa4s.apps.googleusercontent.com";
		options.ClientSecret = "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";
		options.Authority = "https://accounts.google.com";
		options.ResponseType = "code";
		options.SaveTokens = true;
		options.CallbackPath = "/signin-oidc"; // This is the critical missing piece
		options.SignedOutCallbackPath = "/signout-callback-oidc";
		options.Scope.Add("openid");
		options.Scope.Add("profile");
		options.Scope.Add("email");
		options.GetClaimsFromUserInfoEndpoint = true;
		options.ClaimActions.MapJsonKey("email", "email");
		options.TokenValidationParameters.NameClaimType = "name";
		options.TokenValidationParameters.RoleClaimType = "role";
		options.Events = new OpenIdConnectEvents
		{
			OnRemoteFailure = context =>
			{
				context.Response.Redirect("/Auth/Login");
				context.HandleResponse();
				return Task.CompletedTask;
			}
		};
	});

// App services
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<ApiClient>();

// API Services
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<TaskApiService>();
builder.Services.AddScoped<UserApiService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Board}/{id?}");

app.Run();
