namespace api_demo.Models
{
    public class AuthenticatedUser : User
    {
        public AuthenticatedUser()
        {
            Role = Role.AuthenticatedUser;
        }
    }
}


