using System.ComponentModel.DataAnnotations;
namespace PaulSound_VideoCallMessenger.Data
{
    public class ConversationMember
    {
        [Key]
        public int memberId { get; set; }


        public int conversationId { get; set; }
        public Conversation? conversation { get; set; }

        public int userId { get; set; }
        public User? user { get; set; }
    }
}
