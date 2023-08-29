using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projeto.AspNet._03.MVC.Entity.Identity.Models;

var builder = WebApplication.CreateBuilder(args);

// 1º passo: adicionar o serive que "aciona" a string de conexão da aplicação com o servidor e o db - devidamente configurados no arquivo appsettings.json
// AddDbContext() - este é um método
// UseSqlServer() - este é um método
builder.Services.AddDbContext<AppEntityIdentityDbContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

// 2º passo: indicar o contexto de autenticação/autorização de acesso à área restrita - a aprtir do service de inclusão de Identity para a aplicação
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

// 3º passo: adicionar o método que auxilia a aplicação nos processos de autenticação de usuarios para qualquer área restrita  
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
