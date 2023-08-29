using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Projeto.AspNet._03.MVC.Entity.Identity.Models
{
    public class Login
    {   // esta classe assume o "papel" de ser um elemento lógico que opere como um conjunto de propriedades credenciais - como se fosse um cartão de acesso 
        // definir as props necessárias para a operação de autorização/autenticação
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }

        public string? ReturnUrl { get; set; }

        // por padrão, o AspNetCore vai adotar uma URL para o acesso ao "espaço" de inserção de credenciais
        // http://localhost:XXXXX/NomeQualquer/Login
        // ao utilizar a prop ReturnURL, estamos dizendo que é possível customizar a rota para esta área restrita - ou seja, a aplicação pode "fugir"
        // do elemento padrão do AspNet
 
        
    }
}
