using System.ComponentModel.DataAnnotations;

namespace PaulSound_VideoCallMessenger.Data
{
    public class Conversation
    {
        [Key]
        public int conversationId { get; set; }
        [MaxLength(50)]
        public string? ConversationName { get; set; }
    }
}
