using Microsoft.AspNetCore.Identity;

namespace Transport.Infrastructure.Data
{

    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

    }
}