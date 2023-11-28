//signalr için aþaðýdaki satýrý ekle
using StajyerTakipSistemi.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StajyerTakipSistemi.Web.Models;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddHostedService<BackgroundWorkerService>();
// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

//signalr için aþaðýdaki satýrý ekle
builder.Services.AddSignalR();



builder.Services.AddDbContext<StajyerTakipSistemiDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("StajyerTakipSistemiDB")));
builder.Services.AddSession();


builder.Services.AddHostedService<BackgroundWorkerService>();
builder.Services.AddDbContext<StajyerTakipSistemiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StajyerTakipSistemiDB")));

var app = builder.Build();

app.UseSession();
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
    pattern: "{controller=Home}/{action=HomePage}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "absenceInformationRoute",
        pattern: "AbsenceInformation/{action=Index}/{id?}",
        defaults: new { controller = "AbsenceInformation" }
    );

    // Other route mappings...
});



//signalr için aþaðýdaki satýrý ekle
app.MapHub<ConnectedHub>("/ConnectedHub");


app.Run();
