using System.ComponentModel.DataAnnotations;

namespace PaulSound_VideoCallMessenger.Data
{
    public class ContactList
    {
        [Key]
        public int contactId { get; set; }
        [Required]
        public int user1_id { get; set; }
        [Required]
        public int user2_id { get; set;}
    }
}
