using Business;
using Business.Services;
using DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Movies479_Version2.Settings;
using System.ComponentModel.Design;

var builder = WebApplication.CreateBuilder(args);

#region AppSettings
// Application configuration settings can be read from sections such as AppSettings in the appsettings.json file.
// Way 1:
//var section = builder.Configuration.GetSection("AppSettings");
// Way 2:
var section = builder.Configuration.GetSection(nameof(Movies479_Version2.Settings.AppSettings)); // only AppSettings class can also be used by
                                                                                  // adding "using MVC.Settings;" directive
section.Bind(new Movies479_Version2.Settings.AppSettings()); // this method will fill the only one instance of type AppSettings with data in the
                                              // AppSettings section of the appsettings.json file
#endregion

// Add services to the container.
builder.Services.AddDbContext<Db>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IDirectorService, DirectorService>();


#region Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    // We are adding authentication to the project using default Cookie authentication.

    .AddCookie(config =>
    // We configure the cookie to be created through the config action delegate; unlike func delegates, 
    // action delegates do not return a result and they are generally used for configuration operations as seen here.

    {
        config.LoginPath = "/Account/Login";
        // If an operation is attempted without logging into the system, redirect the user to the 
        // Users controller -> Login action.

        config.AccessDeniedPath = "/Account/AccessDenied";
        // If an unauthorized operation is attempted after logging into the system, redirect the user to the 
        // Users controller -> AccessDenied action.

        // Way 1:
        //config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        // Way 2: getting minute value from appsettings.json
        config.ExpireTimeSpan = TimeSpan.FromMinutes(AppSettings.CookieExpirationInMinutes);
        // Allow the cookie created after logging into the system to be valid for 30 minutes.

        config.SlidingExpiration = true;
        // When SlidingExpiration is set to true, the user's cookie expiration is extended by a specific duration 
        // every time they perform an action in the system. If set to false, the user's cookie lifespan ends after 
        // a specific duration after the initial login, requiring them to log in again.
    });
#endregion

#region Session
builder.Services.AddSession(config => // we add session to the services through the config action delegate
{
    config.IdleTimeout = TimeSpan.FromMinutes(30); // the session data will be kept during this value as long as the application user interacts,
                                                   // default is 20 minutes
    config.IOTimeout = Timeout.InfiniteTimeSpan; // we set this value for using the only time out value of IdleTimeout
});
#endregion

builder.Services.AddControllersWithViews();
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

#region Authentication
app.UseAuthentication();
#endregion

app.UseAuthorization();

#region Session
app.UseSession();
#endregion

// custom conventional routes can be added before the default route so that here for example instead of calling "Movies/Create" route,
// "Register" route can be called when necessary
app.MapControllerRoute(name: "register",
    pattern: "register",
    defaults: new { controller = "Movies", action = "Create" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
