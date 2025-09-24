using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace api_demo.Models
{
    [Table("members")]
    public class Member : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
    }
}


