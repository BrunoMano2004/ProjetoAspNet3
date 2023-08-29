using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projeto.AspNet._03.MVC.Entity.Identity.Models;

var builder = WebApplication.CreateBuilder(args);

// 1� passo: adicionar o serive que "aciona" a string de conex�o da aplica��o com o servidor e o db - devidamente configurados no arquivo appsettings.json
// AddDbContext() - este � um m�todo
// UseSqlServer() - este � um m�todo
builder.Services.AddDbContext<AppEntityIdentityDbContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

// 2� passo: indicar o contexto de autentica��o/autoriza��o de acesso � �rea restrita - a aprtir do service de inclus�o de Identity para a aplica��o
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppEntityIdentityDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

// 3� passo: adicionar o m�todo que auxilia a aplica��o nos processos de autentica��o de usuarios para qualquer �rea restrita  
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
