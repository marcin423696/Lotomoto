using Microsoft.EntityFrameworkCore;
using Lotomoto.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LotomotoConnectionString")));

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; 
    });

var app = builder.Build();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Lotomoto.Data.ApplicationDbContext>();

        
        context.Database.Migrate();

        
        if (!context.CarListings.Any())
        {
            context.CarListings.AddRange(
                new Lotomoto.Models.CarListing
                {
                    Title = "BMW ",
                    Price = 45000,
                    Mileage = 180000,
                    Year = 2016,
                    Category = "Osobowe",
                    Power = 150,
                    Description = "BMKA smiga ram pam pam ",
                    ImageUrl = "/uploads/bmw.jpg" 
                },
                new Lotomoto.Models.CarListing
                {
                    Title = "Skoda",
                    Price = 38000,
                    Mileage = 210000,
                    Year = 2014,
                    Category = "Osobowe",
                    Power = 170,
                    Description = "Super skoda smiga",
                    ImageUrl = "/uploads/szkoda.jpg"
                }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Wyst¹pi³ b³¹d podczas migracji lub seedowania bazy danych.");
    }
}

app.Run();