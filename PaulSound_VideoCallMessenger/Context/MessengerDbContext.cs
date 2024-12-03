using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaulSound_VideoCallMessenger.Data;

namespace PaulSound_VideoCallMessenger.Context
{
    public class MessengerDbContext:DbContext
    {
        private readonly IConfiguration _config;
        public MessengerDbContext(IConfiguration config,DbContextOptions<MessengerDbContext>options):base(options)
        {
            _config = config;
        }
        public DbSet<User>users=>Set<User>();
        public DbSet<Message> messages => Set<Message>();
        public DbSet<Conversation> conversations => Set<Conversation>();
        public DbSet<ConversationMember> members => Set<ConversationMember>();
        public DbSet<ContactList> contact_list=> Set<ContactList>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactList>().HasKey(u => new {u.user1_id,u.user2_id });
        }
    }
}
