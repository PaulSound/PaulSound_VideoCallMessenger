using System.ComponentModel.DataAnnotations;

namespace PaulSound_VideoCallMessenger.Models
{
    public class MessageModel
    {
        [Key]
        public int MessageId { get; set; }
        public string? MessageFrom { get; set; }
        public string? MessageToUser { get; set; }
        public string? MessageContent { get; set; }
        public DateTime SentTime { get; set; }
    }
}
