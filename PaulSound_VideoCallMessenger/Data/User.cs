using System.ComponentModel.DataAnnotations;

namespace PaulSound_VideoCallMessenger.Data
{
    public class User
    {
        [Key]
        public int userId { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string UserName { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string UniqueIdentifier { get; set; } = null!;

    }
}
