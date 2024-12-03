using System.ComponentModel.DataAnnotations;

namespace PaulSound_VideoCallMessenger.Models
{
    public class ConversationModel
    {
        [Key]
        public int conversationId { get; set; }
        [MaxLength(50)]
        public string? ConversationName { get; set; }
    }
}
