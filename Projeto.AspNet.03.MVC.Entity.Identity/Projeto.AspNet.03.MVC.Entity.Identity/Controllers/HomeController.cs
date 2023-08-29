using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Projeto.AspNet._03.MVC.Entity.Identity.Models;
using System.Diagnostics;
using Projeto.AspNet._03.MVC.Entity.Identity.Controllers;

namespace Projeto.AspNet._03.MVC.Entity.Identity.Controllers
{
    // Este controle será responsável por exercer controle e influência sobre a área restrita da aplicação
    public class HomeController : Controller
    {
        // Objeto referencial definido a partir da interface ILogger para que - se necessário for - seja possível obter informações de log
        // a partir do comportamento do controller
        private readonly ILogger<HomeController> _logger;

        // 1 passo: definir o "auxiliador" - objeto referencial - referenciado a partir da classe embarcada UserManager
        UserManager<AppUser> _userManager;


        // Este é o construtor da classe onde são definidas as DPs(Dependences Injection)
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            // 2 passo: definir a di
            _userManager = userManager;
        }

        // Definição das actions relacionas com este controller
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // 3 passo: criar uma nova action para "controlar" as operações em relação a view que será estabelecida - Private.cshtml; esta action é uma tarefa assíncrona
        [Authorize] // O uso do atributo [Authorize] define que a action precisa ser acessada por um contexto de autenticação e autorização dadas à um determinado conjunto de dados de acesso

        public async Task<IActionResult> Private()
        {
            // Criar uma prop para exercer uma consulta que receberá como valor o nome do usuário que foi devidamente autenticado e autorizado acessar está área
            AppUser consultaUser = await _userManager.GetUserAsync(HttpContext.User); // HttpContext = método get. User = método set {get; set;}

            // Criar uma nova prop para receber o valor de uma mensagem de boas-vindas associado ao nome do usuario.
            string mensagem = "Ola " + consultaUser.UserName + " você está na área restrita da aplicação";

            return View((object)mensagem);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // É um atributo que define a action Error()
        // O atributo determina que nenhuma informação sobre qualquer erro que possa ocorrer fique armazenado na aplicação por tempo nenhum, em nenhum local...
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}