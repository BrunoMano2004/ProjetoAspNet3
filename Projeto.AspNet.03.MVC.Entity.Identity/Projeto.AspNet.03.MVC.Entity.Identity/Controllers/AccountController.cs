using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Projeto.AspNet._03.MVC.Entity.Identity.Models;
using System.Drawing.Text;
using System.Reflection.Metadata.Ecma335;

namespace Projeto.AspNet._03.MVC.Entity.Identity.Controllers
{
    // este controller será responsável pela modalidade de autenticação/autorização de usuários para acessar a área restrita da aplicação
    [Authorize] // este atributo faz com que todas as estruturas de instruções relacionas à esta classe se tornem inacessíveis por instruções sem autorização de acesso.
                // Significa que qualquer estrutura lógica que não faz parte desta classe não pode fazer acesso a nada que, aqui, está descrito.
    public class AccountController : Controller
    {
        /*
         ======================================================================================================================================================
             1 MOVIMENTO: CONFIGURAÇÃO DO CONTROLLER DA ESTRUTURA DE LOGIN
         ======================================================================================================================================================
         */

        // 1 passo: definir dois "auxiliadores" - objetos referenciais - para a DI (Dependence Injection)

        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signinManager; // este 2 objeto referencial nada mais é do que um "gerenciador de recursos de acesso" à áreas restritas de uma aplicação

        // 2 passo: estabelecer o construtor da classe - de forma customizada; definindo as DIs(Dependence Injections)

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signinManager = signInManager;
        }

        /*
        =======================================================================================================================================================
            2 MOVIMENTO: DEFINIÇÃO DAS ACTIONS PARA A ESTRUTURA DE LOGIN
        =======================================================================================================================================================
         */

        // 12 passo: estabelece a action Login para gerar um objeto e fazer acesso ao "endereço" no qual será indicado a entrada das credenciais
        //IP estagio da estrutura de loginl de acesso - 1 estagio da estrutura de login
        [AllowAnonymous] // este atributo//annotation permite o acesso à estas funcionalidades - descritas na action abaixo - sem a necessidade de autenticação ou autorização prévia.
        public IActionResult Login(string returnUrl)
        {
            // Praticar a instancia direto do model login para gerar um objeto do qual se faça uso e acessar a suas props
            Login login = new Login();

            // Fazer uso do objeto para acessar a prop ReturnUrl
            login.ReturnUrl = returnUrl;
            return View(login);
        }

        // 2 passo: definir, de forma explícita, a tarefa assíncrona para o envio de dados e mais  um estágio da estrutura de login
        [HttpPost]
        [AllowAnonymous] // estabelece a possibilidade do processo de autenticação ocorrer com o acesso anônimo do usuario
        [ValidateAntiForgeryToken] // este atributo impede a autenticação desta funcionalidade entre elementos autonomos
        public async Task<IActionResult> Login(Login logar)
        {
            // 3 passo: observar as validações do model Login.cs aplicados a view Login.cshtml
            if (ModelState.IsValid)
            {
                // 3A passo: definir a consulta - à base de dados sempre a partir de sua representação - será verificado se o valor obtido da prop email é válido
                AppUser consulta = await _userManager.FindByEmailAsync(logar.Email);

                // 4 passo: validar a consulta
                if(consulta != null)
                {
                    // Se o email encontrado na consulta está - no momento da consulta - logado no sistema. Se sim, o método abaixo providencia sua saida desta autenticação
                    // "desloga" o usuario - então o método encerra a sessão de login do usuario no sistema para que seja possível continuar o processo de autenticação necessário
                    // para o acesso à área restrita.
                    await _signinManager.SignOutAsync();

                    // Fazer uso de uma classe embarcada SignInResult para operar com um resultado de processo de autenticação de usuario
                    Microsoft.AspNetCore.Identity.SignInResult resultado = await _signinManager.PasswordSignInAsync(consulta, logar.Password, false, false);
                    // Aqui, temos os dados referentes ao Email e Senha inseridas pelo usuario
                    // O 1 false é para indicar que não é necessário persistir a sessão de acesso
                    // O 2 false impede qualquer bloqueio de autenticação/acesso - caso ocorra falha ao autenticar o usuario

                    // Fazer acesso a variável resultado e verificar se o valor atribuído resulta e sucesso
                    if (resultado.Succeeded)
                    {
                        // Abaixo, está indicada a área rstrita da aplicação - será uma action descrita no HomeController com uma view respectiva.
                        return Redirect(logar.ReturnUrl ?? "/Home/Private");
                        // ?? este é o operador de coalescencia nula: pop ?? "-----------" - se o valor de retorno da operação for diferente de nulo, o operador coalescencia
                        // retorna o valor do lado direito do operador. Caso contrário, retorna do lado direito do operador
                    }
                }
                ModelState.AddModelError(nameof(logar.Email), "sua autenticação falhou.");
            }
            return View(logar);
        }

        //4 passo: definir uma nova action para que o usuário, uma vez logado, possa sair da área restrita e ser redirecionado para uma área pública
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync(); // Caso o login/autorização já esteja e funcionamento pleno - ou seja - o usuario está usando a área restrita - o método 
            // SignOutAsync() é chamado para encerrar a "sessão" 

            // É importante indicar a "rota" pela qual o usuário será redirecionado quando escolher "sair" da área restrita.
            // Abaixo, estão indicadas a action e o controller para o qual o usuário será redirecionado.
            return RedirectToAction("Index", "Home");
        }
    }
}
