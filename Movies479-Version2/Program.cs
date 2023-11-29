using Business;
using Business.Services;
using DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
