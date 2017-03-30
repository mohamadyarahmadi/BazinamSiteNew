using Microsoft.AspNet.Identity.EntityFramework;

namespace Bazinam.DomainClasses
{
    public class ApplicationUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {

    }
    
}
