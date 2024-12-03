using System.ComponentModel.DataAnnotations;

namespace PaulSound_VideoCallMessenger.Models
{
    public class ConversationMemberModel
    {
        [Key]
        public int Id { get; set; }
        public int conversationId { get; set; }
        public int userId { get; set; }
    }
}
