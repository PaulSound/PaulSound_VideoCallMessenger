using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using PaulSound_VideoCallMessenger.Data;
using PaulSound_VideoCallMessenger.Services;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.VisualBasic;

namespace PaulSound_VideoCallMessenger.Hubs
{
    public class ChatHub : Hub
    {
        public static List<HubUser> connectedUsers = new List<HubUser>(); 
        public static List<HubUser>? contactList; 
      
        private readonly DatabaseService _databaseService;
        public ChatHub(DatabaseService databaseService) {
            _databaseService=databaseService;
        } 
        public override async Task OnConnectedAsync() 
        {
            var hubUser = new HubUser() 
            {
                Name = Context.User.Identity.Name,    
                UserIdentifier = Context.UserIdentifier 
            };
            
            if (connectedUsers.Find(x => x.Name == hubUser.Name) == null)
                connectedUsers.Add(hubUser);
            contactList = _databaseService.GetContactList(hubUser.Name);

            await Clients.Caller.SendAsync("IdentifyUser", hubUser); // инициализировать пользователя который вошел в хаб
            await Clients.Caller.SendAsync("ReceiveContactList", contactList); // подключаем уникальный список контактов
            await Clients.Caller.SendAsync("RecieveOnlineUsers", connectedUsers); 
            await Clients.All.SendAsync("RecieveAvailableUsers", _databaseService.GetAvailableUsers()); 
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? e) 
        {
            var hubUser = connectedUsers.Find(x => x.UserIdentifier == Context.UserIdentifier);
            lock (connectedUsers)
            {
                if (hubUser != null)
                {
                    connectedUsers.Remove(hubUser);
                }
            }
            await Clients.All.SendAsync("RecieveOnlineUsers", connectedUsers);
            await base.OnDisconnectedAsync(e);
        }
        public async Task ConnectUserToConversation(HubUser currentUser,HubUser toUser)
        {
            var conversationName = CreatePrivateConversationName(currentUser, toUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationName);
            await Clients.User(currentUser.UserIdentifier).SendAsync("SetCurrentConversation", conversationName); 
            var messageList = await _databaseService.GetMessageList(conversationName);
            await Clients.User(Context.UserIdentifier).SendAsync("LoadChat", messageList);
        }
        public async Task AddNewUser(string uniqueIdentifier) 
        {
            var user=_databaseService.GetUserByUniqueIdent(uniqueIdentifier);
            _databaseService.AddUserToContact(user,Context.UserIdentifier);
            contactList = null;
            contactList = _databaseService.GetContactList(Context.User.Identity.Name);
            await Clients.Caller.SendAsync("ReceiveContactList", contactList); 
            await Clients.Caller.SendAsync("RecieveOnlineUsers", connectedUsers);
            await Clients.Caller.SendAsync("RecieveAvailableUsers", _databaseService.GetAvailableUsers());
        }
        public async Task UserSearchPattern (string searchPattern) 
        {
            var result = _databaseService.GetAvailableUsersByPattern(searchPattern);
            await Clients.Caller.SendAsync("RecieveAvailableUsers", result);
        }
        public async Task SendPrivateMessage(HubUser currentHubUser,HubUser toUser,string message) 
        {
            var hubUser = new HubUser() 
            {
                UserIdentifier = Context.UserIdentifier,
                Name = Context.User.Identity.Name
            };
            var conversationName = CreatePrivateConversationName(currentHubUser, toUser);
            var conversation = _databaseService.GetConversationByName(conversationName);

            if(conversation==null)
            {
                conversation = new Conversation() { ConversationName = conversationName };
                _databaseService.AddNewConversation(conversation);
                await _databaseService.RegisterMembers(currentHubUser,toUser,conversation.conversationId);
            }

            await _databaseService.SaveMessage(message, DateTime.Now, conversation.conversationId); 
            
            var timeNow=DateTime.Now;
            var saveMessage = new HubMessage()
            {
                FromUserName = hubUser.Name,
                Time = timeNow.ToString("HH:mm:ss"),
                Message=message
            };

            await Clients.Group(conversationName).SendAsync("ReceiveMessage", hubUser, message, timeNow.ToString("HH:mm:ss"));
        }
  
        public async Task JoinRoom(string groupName) 
        {
            var hubUser = new HubUser() 
            {
                UserIdentifier = Context.UserIdentifier,
                Name = Context.User.Identity.Name
            };
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName); // добавления пользователя к определенной группе
        }
      
        public async Task LeaveRoom(string groupName) 
        {
            var hubUser = new HubUser()
            {
                UserIdentifier = Context.UserIdentifier,
                Name = Context.User.Identity.Name
            };
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName); // удаляем пользователя с определенной индефикатором из группы
        }

        public async Task LoadConversation(string conversationName)
        {
            var hubUser = new HubUser() // Используется чтобы передать в пользвательский js код
            {
                UserIdentifier = Context.UserIdentifier,
                Name = Context.User.Identity.Name
            };
            var messageList = await _databaseService.GetMessageList(conversationName);
            await Clients.User(hubUser.UserIdentifier).SendAsync("LoadChat", messageList);

        }
        public string CreatePrivateConversationName(HubUser currentUser,HubUser userTo) 
        {
               int user_1Id = int.Parse(currentUser.UserIdentifier);
               int user_2Id = int.Parse(userTo.UserIdentifier);
               List<int>idList=new List<int>() { user_1Id, user_2Id };
               idList.Sort();
               var conversationName = $"[{idList[0]}-{idList[1]}]";
               return conversationName;
        }
    }
}

