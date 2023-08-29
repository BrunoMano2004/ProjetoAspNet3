using Microsoft.AspNetCore.Identity;

namespace Projeto.AspNet._03.MVC.Entity.Identity.Models
{
    // esta classe assume o papel de "representação/entidade/entity" da table do Db. Para tal proposito, será praticado o mecanismo de herança com a classe IdentityUser
    public class AppUser : IdentityUser // superclasse/pai/base
    {
        // em função do mecanismo de herança algumas das props que a superclasse IdentityUser disponibiliza para a criação desta representação são, por exemplo:
        // Id
        // Username
        // Name
        // Email
        // Hash Password
    }
}
