using AutoMapper;
using PaulSound_VideoCallMessenger.Data;
using PaulSound_VideoCallMessenger.Models;

namespace PaulSound_VideoCallMessenger.Mapper
{
    public class AppProfile:Profile
    {
        public AppProfile()
        {
            CreateMap<User,UserModel>().ReverseMap();
            CreateMap<Conversation,ConversationModel>().ReverseMap();
        }
    }
}
