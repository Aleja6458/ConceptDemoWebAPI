using System.ComponentModel.DataAnnotations;

namespace _2Share.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Password { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
