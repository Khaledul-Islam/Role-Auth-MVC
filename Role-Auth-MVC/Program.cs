using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Role_Auth_MVC.Data;
using Role_Auth_MVC.Mappings;
using Role_Auth_MVC.Repositories;
using Role_Auth_MVC.Repositories.Interfaces;
using Role_Auth_MVC.Services;
using Role_Auth_MVC.Services.Interfaces;
using Role_Auth_MVC.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Role-Auth-MVC")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services (Dependency Inversion Principle)
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(LoginDtoValidator).Assembly);

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
    options.AddPolicy("EditorOnly", policy => policy.RequireRole("Editor"));
    options.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Moderator"));
    options.AddPolicy("SystemUser", policy => policy.RequireRole("Editor", "Moderator"));
});

var app = builder.Build();

// Configure the HTTP request pipeline
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
