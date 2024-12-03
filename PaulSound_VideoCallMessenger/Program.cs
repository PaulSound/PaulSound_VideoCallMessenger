using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PaulSound_VideoCallMessenger.Context;
using PaulSound_VideoCallMessenger.Hubs;
using PaulSound_VideoCallMessenger.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MessengerDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSignalR();


builder.Services.AddScoped<LoginService>(); // экземпляр службы создается один раз на каждый запрос 
builder.Services.AddScoped<DatabaseService>(); // экземпляр службы создается один раз на каждый запрос 


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Добавляем севис по аутификации
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";
        options.Cookie.Name = "LoginCookie";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

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
app.UseCookiePolicy();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.Run();
