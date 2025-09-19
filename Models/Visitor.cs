using System.ComponentModel.DataAnnotations;

namespace api_demo.Models
{
    public class Visitor : User
    {
        public Visitor()
        {
            Role = Role.Visitor;
        }
    }
}


