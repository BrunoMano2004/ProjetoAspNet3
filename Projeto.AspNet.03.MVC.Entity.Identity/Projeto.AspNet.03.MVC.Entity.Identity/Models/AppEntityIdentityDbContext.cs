using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Projeto.AspNet._03.MVC.Entity.Identity.Models
{
    // esta classe proporciona o contexto/referencia do DB SSQL Server aqui representado.
    // para este proposito será praticado o mecanismo de herança com a classe embarcada IdentityDbContext<>
    //o objetivo é: oferecer à subclasse todos os recursos necessários para o contexto/referencia de integração entre as aplicações - back-end e front-end
    public class AppEntityIdentityDbContext : IdentityDbContext<AppUser> // a especificidade do elemento generico IdentityDbContext<> é dado pela representação da table do DB
    {
        // definir o construtor da classe porque é necessário "priorizar" a referencia do contexto, aqui, definido
        public AppEntityIdentityDbContext(DbContextOptions<AppEntityIdentityDbContext> options) : base(options) { }
        
    }
}
