using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Projeto.AspNet._03.MVC.Entity.Identity.Models;

namespace Projeto.AspNet._03.MVC.Entity.Identity.Controllers
{
    //DEFINIR O "PAPEL" DESTE CONTROLLER: será responsávelpelas operações CRUD do "cadastro" de dados do usuário
    //pratica do mecanismo de herança coma superclasse Controller
    public class AdminController : Controller
    {
        /* ===================================================================================================================
         1 MOVIMENTO: DEFINIÇÃO DE ELEMENTOS REFERENCIAIS E PRATICA DE INJEÇÃO DE DEPENDENCIA PARA AS OPERAÇÕES COM OS DADOS
         =====================================================================================================================
         */

        /* 1 passo: definir uma prop - private - para criar um elemento lógico referencial.
         * Neste momento, é importante criar este elemento para que seja usado no auxílio 
         * na manipulação de dados da base - com as quais o controller vai "lidar". Para
         * a definição deste elemento será usada uma classe embarcada UserManager<> - oferece
         * recursos para operações com dados de usuário (esta classe tem origem no AspNetCore)
        */
        private UserManager<AppUser> _userManager;

        /* 2 passo: definir uma nova prop - private - para criar um elemento lógico referencial.
         * Servirá como referência para a recuperação/leitura/alteração da senha/password em
         * estrutura Hash. esta prop será definida a partir do recurso de interface embarcada
         * IPasswordHasher
         */
        private IPasswordHasher<AppUser> _passwordHasher;

        /* 3 passo: será a definição da injeção de dependência. Para este contexto serão usadas
         * as props de referencia - a partir do construtor da classe
         */
        public AdminController(UserManager<AppUser> UserManager, IPasswordHasher<AppUser> PasswordHasher)
        {
            // aqui, as props private serão acessadas e, à elas, atribuido valores referentes ao parametros do construtor
            _userManager = UserManager;
            _passwordHasher = PasswordHasher;
        }

        /*======================================================================================================================
         * 2 MOVIMENTO: CRIAÇÃO DAS ACTIONS - definição das operações CRUD - C - Create (Criar), R - Read (Ler), U - Update (Atualizar)
         * D - Delete (Deletar)
         * 
         * AppUser: representação da table - aqui, na aplicação - do DB. Neste contexto - relação entre AppUser e Model User - a
         * representação da table será responsável por receber do model User - aplicados as regras/formatos - os dados necessários
         * para as manipulações e, posteriormente, os processos de autenticação/autorização de acesso à área restrita da aplicação
         * 
         * User: é o model que estabelece as regras/formato pelos quais os dados serão operados - com foco no procedimento de cadastro
         * do usuário
         *=====================================================================================================================
         */

        // 1 OP CRUD - Read - action que será responsável pela recuperação/acesso e exibição de todos os dados armazenados na base

        public IActionResult Index() // esta é uma action síncrona
        {
            // O que esta action vai retornar?
            return View(_userManager.Users);
            // R: o elemento lógico Users(método get) que foi, acima, referenciado por ser um método get- implícito.
            // Dessa forma é possível recuperar os dados da base. É um método exclusivo da classe UserManager.
        }

        // 2 OP CRUD - Create - action será responsável pela inserção de dados na base.
        /*
        public IActionResult Create()
        {
            return View();
        }
        */

        // esta é uma nova forma de definir uma action que trará como resultado a mesma situação indicada na action acima
        public ViewResult Create() => View();

        //... continuando 2 OP: sobrecarga da action para que os dados possam ser obtidos e, posteriormente, armazenados
        // definir o atributo de requisição [HttpPost]
        [HttpPost]
        // definir - de forma explícita - uma tarefa assíncrona para obter e enviar os dados para o armazenamento
        public async Task<IActionResult> Create(User registro)
        {
            // verificar se o ModelState é válido
            if (ModelState.IsValid)// se a avaliação for TRUE
            {
                // definir um objeto - a partir do model/representação da table AppUser - para, posteriormente, serem praticados
                // os processos de autenticação/autorização de acesso à área restrita. Além deste propósito acima é preciso
                // entender que nesta action está sendo disposto o processo de obtenção e armazenamento de dados

                AppUser appuser = new AppUser
                {
                    UserName = registro.Name,
                    Email = registro.Email
                    // agora, o objeto appUser possui valores para as duas props/atributo que o compõem
                };

                // neste passo, será utilizado - de forma assíncrona - um método de criação/inserção de registro na base
                IdentityResult resultadoInsert = await _userManager.CreateAsync(appuser, registro.Password); // aqui está o conjunto de dados com as 3 props definidas no model User

                // é necessário, agora, aninhar um novo if(){} - para que os recursos embarcados de sucesso possam indicar o resultado da operação
                if (resultadoInsert.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    // estabeler um loop para investigar/iterar sobre eventuais erros que possam ter ocorrido
                    foreach (IdentityError erro in resultadoInsert.Errors)
                    {
                        ModelState.AddModelError("", erro.Description);
                    }
                }
            }

            return View(registro);
        }

        // 3 OP CRUD - Update - será responsável pela REinserção de dados na base - desde que esteja devidamente armazenado e identificado na base

        // definição da action Update - é necessário que o registro atual seja disponibilizado para a operação de alteração/atualização.

        public async Task<IActionResult> Update(string id)
        {
            // definir uma consulta - à base - para a obtenção de um registro para atualização/alteração. Para este proposito será uma prop para receber
            // como valor a consulta que o registro

            AppUser buscaUser = await _userManager.FindByIdAsync(id);

            // avaliar o resultado da busca e verificar se o registro realmente existe

            if (buscaUser != null)
            {
                return View(buscaUser);
            }
            else
            {
                return View("Index");
            }
        }

        // ...continuando a 3 OP: sobrecarga de action/método Update para que seja, agora, possível alterar/atualizar e REenviar os dados para a base

        [HttpPost]
        public async Task<IActionResult> Update(string id, string username, string email, string password)
        {
            // repete a consulta à base - aqui, será observado se o registro está disponível é, realmente, ele que está senod alterado
            AppUser buscaUser = await _userManager.FindByIdAsync(id);

            // agora é necessário lidar com as props e seus valores para serem alterados e, posteriormente, REenviados à base

            if (buscaUser != null) // neste passo - com a avalição considerada TRUE - a variavel trouxe um resultado nada mais do que o conjunto de dados.
                                   // A partir deste momento, o conjunto será "particionado" e observado/manipulado "pedaço-a-pedaço"
            {
                // observar o 1 pedaço: o valor da prop email
                if (!string.IsNullOrEmpty(email))// aqui, a prop email é um parâmetro deo método IsNullOrEmpty
                {
                    buscaUser.Email = email;
                }
                else
                {
                    ModelState.AddModelError("", "O campo email não pode ser vazio!");

                    // observar o 2 ponto: o valor da prop password
                }

                if (!string.IsNullOrEmpty(username))
                {
                    buscaUser.UserName = username;
                }
                else
                {
                    ModelState.AddModelError("", "O campo nome não pode ser vazio");
                }

                if (!string.IsNullOrEmpty(password))
                {
                    buscaUser.PasswordHash = _passwordHasher.HashPassword(buscaUser, password);
                }
                else
                {
                    ModelState.AddModelError("", "O campo senha/password não pode ser vazio!");
                }

                // observar o 3 pedaço: consiste em observar e avaliar os dados de email e password, agora, em conjunto. Para que seja possível REenvia-los à
                // base e REarmazena-lo - de forma assincrona

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username))
                {
                    IdentityResult resultadoOP = await _userManager.UpdateAsync(buscaUser); // neste ponto, a atualização/alteração ocorre

                    // verificar o sucesso da operação

                    if (resultadoOP.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // chamada do método de observação de erros
                        Erros(resultadoOP);
                    }
                }

            }
            else
                ModelState.AddModelError("", "Usuário não encontrado!");
            return View(buscaUser);
        }
        private void Erros(IdentityResult ocorrenciaErros)
        {
            // estabeler um loop para iterar sobre todas as ocorrencias de eventuais erros que podem ocorrer
            foreach(IdentityError erro in ocorrenciaErros.Errors)
            {
                ModelState.AddModelError("", erro.Description);
            }
        }

        // 4 OP CRUD - DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser buscaUser = await _userManager.FindByIdAsync(id);

            if(buscaUser != null)
            {
                IdentityResult resultadoExc = await _userManager.DeleteAsync(buscaUser);

                if (resultadoExc.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    Erros(resultadoExc);
                }
            }
            else
            {
                ModelState.AddModelError("", "Usuario, infelizmente, não foi encontrado");
            }
            return View("Index", _userManager.Users);
        }
    }
}
