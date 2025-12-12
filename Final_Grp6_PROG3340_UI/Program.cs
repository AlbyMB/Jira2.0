using Final_Grp6_PROG3340_UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<ApiClient>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
	options.Authority = "https://accounts.google.com";
	options.ClientId = builder.Configuration["Google:ClientId"];
	options.ClientSecret = builder.Configuration["Google:ClientSecret"];
	options.CallbackPath = "/auth/google-callback";
	options.ResponseType = "code";
	options.SaveTokens = true;
	
	options.Scope.Clear();
	options.Scope.Add("openid");
	options.Scope.Add("profile");
	options.Scope.Add("email");
	
	options.GetClaimsFromUserInfoEndpoint = true;
	
	options.Events = new OpenIdConnectEvents
	{
		OnTokenValidated = context =>
		{
			var tokens = context.Properties.GetTokens();
			foreach (var token in tokens)
			{
				Console.WriteLine($"OIDC Token: {token.Name} = {token.Value}");
			}
			return Task.CompletedTask;
		}
	};
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
		)
	};

	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			var token = context.HttpContext.Request.Cookies["AuthToken"];
			if (!string.IsNullOrEmpty(token))
				context.Token = token;

			return Task.CompletedTask;
		}
	};
});

// App services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ApiClient>();

// API Services
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<TaskApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Board}/{id?}");

app.Run();
