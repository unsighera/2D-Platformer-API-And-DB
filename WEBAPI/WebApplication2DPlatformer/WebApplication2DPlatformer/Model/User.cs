using System.ComponentModel.DataAnnotations;

namespace WebApplication2DPlatformer.Model
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
