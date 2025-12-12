using Final_Grp6_PROG3340.Data;
using Final_Grp6_PROG3340.Hubs;
using Final_Grp6_PROG3340.Models;
using Final_Grp6_PROG3340.Repositories;
using Final_Grp6_PROG3340.Services;
using Final_Grp6_PROG3340.UOfW;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//used for caching
builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("DefaultDb"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddSingleton<PasswordService>();


builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
}); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7005")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
builder.Services.AddSignalR();
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]);

builder.Services
	.AddAuthentication("Bearer")
	.AddJwtBearer("Bearer", o =>
	{
		o.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwt["Issuer"],
			ValidAudience = jwt["Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(key),
		};
		o.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				// SignalR expects access_token in query string
				var accessToken = context.Request.Query["access_token"];
				var path = context.HttpContext.Request.Path;
				if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/taskHub"))
				{
					context.Token = accessToken;
				}
				return Task.CompletedTask;
			}
		};
	});

builder.Services.AddAuthorization();
var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowFrontend");
app.MapControllers();
app.MapHub<TaskHub>("/taskHub");
void SeedDatabase(WebApplication app)
{
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	var hasher = new PasswordHasher<User>();

	db.Database.EnsureCreated();

	if (!db.Users.Any())
	{
		var admin = new User
		{
			Id = 1,
			Name = "Admin User",
			Email = "admin@example.com",
			Username = "admin",
			Role = "Admin",
			CreatedAt = DateTime.UtcNow
		};
		admin.PasswordHash = hasher.HashPassword(admin, "admin123");

		var john = new User
		{
			Id = 2,
			Name = "John Doe",
			Email = "john@example.com",
			Username = "john",
			Role = "User",
			CreatedAt = DateTime.UtcNow
		};
		john.PasswordHash = hasher.HashPassword(john, "pass123");

		var sarah = new User
		{
			Id = 3,
			Name = "Sarah Lee",
			Email = "sarah@example.com",
			Username = "sarah",
			Role = "User",
			CreatedAt = DateTime.UtcNow
		};
		sarah.PasswordHash = hasher.HashPassword(sarah, "pass123");

		var michael = new User
		{
			Id = 4,
			Name = "Michael Chen",
			Email = "michael@example.com",
			Username = "michael",
			Role = "User",
			CreatedAt = DateTime.UtcNow
		};
		michael.PasswordHash = hasher.HashPassword(michael, "pass123");

		var emily = new User
		{
			Id = 5,
			Name = "Emily Stone",
			Email = "emily@example.com",
			Username = "emily",
			Role = "User",
			CreatedAt = DateTime.UtcNow
		};
		emily.PasswordHash = hasher.HashPassword(emily, "pass123");

		db.Users.AddRange(admin, john, sarah, michael, emily);
		db.SaveChanges();
	}

	if (!db.Tasks.Any())
	{
		db.Tasks.AddRange(
			new TaskItem
			{
				Id = 1,
				Title = "Initial Project Setup",
				Description = "Create solution + basic API structure",
				Status = Final_Grp6_PROG3340.Models.TaskStatus.ToDo,
				CreatedById = 1, // admin
				AssignedToId = 2, // john
				IsArchived = false,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			},
			new TaskItem
			{
				Id = 2,
				Title = "Design Database Models",
				Description = "User + Task models with relationships",
				Status = Final_Grp6_PROG3340.Models.TaskStatus.Development,
				CreatedById = 2,
				AssignedToId = 2,
				IsArchived = false,
				CreatedAt = DateTime.UtcNow.AddMinutes(-20),
				UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
			},
			new TaskItem
			{
				Id = 3,
				Title = "Implement Task CRUD",
				Description = "Basic CRUD endpoints",
				Status = Final_Grp6_PROG3340.Models.TaskStatus.Review,
				CreatedById = 3,
				AssignedToId = 3,
				IsArchived = false,
				CreatedAt = DateTime.UtcNow.AddMinutes(-40),
				UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
			},
			new TaskItem
			{
				Id = 4,
				Title = "Setup Swagger",
				Description = "Enable Swagger UI + documentation",
				Status = Final_Grp6_PROG3340.Models.TaskStatus.Merge,
				CreatedById = 1,
				AssignedToId = 4,
				IsArchived = false,
				CreatedAt = DateTime.UtcNow.AddHours(-1),
				UpdatedAt = DateTime.UtcNow.AddMinutes(-20)
			},
			new TaskItem
			{
				Id = 5,
				Title = "Frontend Wireframes",
				Description = "High-level UI mockups",
				Status = Final_Grp6_PROG3340.Models.TaskStatus.Done,
				CreatedById = 5,
				AssignedToId = 5,
				IsArchived = false,
				CreatedAt = DateTime.UtcNow.AddHours(-2),
				UpdatedAt = DateTime.UtcNow.AddMinutes(-30)
			}
		);

		db.SaveChanges();
	}
}
SeedDatabase(app);

app.Run();
