using System.ComponentModel.DataAnnotations;

namespace Projeto.AspNet._03.MVC.Entity.Identity.Models
{
    // esta classe assume o papel de model domain da aplicação
    // significa que, aqui, serão estabelecidas as regras de manipulação dos dados que circularão pela aplicação
    public class User
    {
        // 1º passo: definir 3 props - o proposito é auxiliar na criação schema que possa refletir algumas colunas da table do DB

        [Required(ErrorMessage = "Por favor, informe seu nome.")]
        public string? Name { get; set; }

        [Required]
        [RegularExpression("[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Por favor, insira uma senha bacana")]
        public string? Password { get; set; }
    }
}
