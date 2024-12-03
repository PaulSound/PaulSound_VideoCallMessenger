using System.ComponentModel.DataAnnotations;

namespace PaulSound_VideoCallMessenger.Data
{
    public class Message
    {
        [Key]
        public int messageId { get; set; }

        public string? MessageText { get; set; }
        public DateTime SentTime { get; set; }


        public int conversationId { get; set; }
        public Conversation? Conversation { get; set; }
    }
}
