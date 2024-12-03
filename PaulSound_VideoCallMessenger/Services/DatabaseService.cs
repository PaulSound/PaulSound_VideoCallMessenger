using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaulSound_VideoCallMessenger.Context;
using PaulSound_VideoCallMessenger.Data;
using PaulSound_VideoCallMessenger.Hubs;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace PaulSound_VideoCallMessenger.Services
{
    public class DatabaseService 
    {
        private readonly MessengerDbContext _dbContext;
        public DatabaseService(MessengerDbContext context)
        {
            _dbContext = context;
        }
        /// <summary>
        /// Получение 7-и "случайных пользователей"
        /// </summary>
        /// <returns></returns>
        public List<SearchUser> GetAvailableUsers() 
        {
            var result = _dbContext.users.Select(x=>new SearchUser() { 
                UserName=x.UserName,
                UniqueIdentifier=x.UniqueIdentifier,
            }).Take(7).ToList();
            return result;
        }
        /// <summary>
        /// Поиск пользователей по введенному шаблону
        /// </summary>
        /// <param name="searchPattern">Шаблон по которому будет работать поиск</param>
        /// <returns></returns>
        public List<SearchUser> GetAvailableUsersByPattern(string searchPattern) 
        {
            string line = searchPattern.ToLower();
            var userList = _dbContext.users.Where(x=>x.UserName.ToLower().Contains(searchPattern)||x.UniqueIdentifier.ToLower().Contains(searchPattern)).Select(x => new SearchUser()
            {
                UserName = x.UserName,
                UniqueIdentifier = x.UniqueIdentifier,
            }).Take(7).ToList();
            return userList;
        }
        /// <summary>
        /// Получить списка пользователей
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetUsers()
        {
            var users = await _dbContext.users.ToListAsync();
            return users;
        }
        /// <summary>
        /// Регистрация нового пользователя в базе данных
        /// </summary>
        /// <param name="user"> Экземпляр User который необходимо добавить в базу данных</param>
        public void RegisterNewUser(User user) 
        {
            _dbContext.users.Add(user);
            _dbContext.SaveChanges();
        }
        /// <summary>
        /// Создание новой беседы
        /// </summary>
        /// <param name="chat">Экземпляр класса Conversation</param>
        public void AddNewConversation(Conversation chat) 
        {
            _dbContext.conversations.Add(chat);
            _dbContext.SaveChanges();
        }
        /// <summary>
        /// ПОлучение экземпляра беседы по специализированному названию беседы
        /// </summary>
        /// <param name="conversationName">Специализированное имя беседы двух пользователей</param>
        /// <returns></returns>
        public Conversation GetConversationByName(string conversationName) 
        {
            var conversation = _dbContext.conversations.FirstOrDefault(x=>x.ConversationName== conversationName);
            return conversation;
        }
        /// <summary>
        /// Получение списка контактов конкретного пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя контакты которого ты хочешь получить</param>
        /// <returns></returns>
        public List<HubUser> GetContactList(string userName) 
        {
            var user = _dbContext.users.Where(x => x.UserName == userName).FirstOrDefault();
            var contactList = _dbContext.contact_list.Where(x => x.user1_id == user.userId).ToList();
            var userList =  _dbContext.users.Where(x => contactList.Select(x => x.user2_id).Contains(x.userId)).ToList();
            List<HubUser>hubUserCollection = userList.Select(ToHubUserModel).ToList();
            return hubUserCollection;
        }
        /// <summary>
        /// Добавление пользователя в список контактов
        /// </summary>
        /// <param name="newUser">Экземпляр пользователя которых будет добавлен в список контактов</param>
        /// <param name="userId">Id текущего пользователя</param>
        public void AddUserToContact(User newUser,string userId) 
        {
            _dbContext.contact_list.Add(new ContactList() { user1_id = int.Parse(userId), user2_id = newUser.userId });
            _dbContext.SaveChanges();
        }
        /// <summary>
        /// Получение пользователя по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User? GetUserById(int id)
        {
            var item = _dbContext.users.FirstOrDefault(x => x.userId == id);
            return item;
        }
        /// <summary>
        /// Найти пользователя по уникальному идефикатору присваемому в момент регистрации
        /// </summary>
        /// <param name="uniqueIdent">Уникальный индефикатор</param>
        /// <returns></returns>
        public User? GetUserByUniqueIdent(string uniqueIdent)
        {
            var item= _dbContext.users.FirstOrDefault(x=>x.UniqueIdentifier== uniqueIdent);
            return item!=null?item:null;
        }
        /// <summary>
        /// Получить пользователя по имени
        /// </summary>
        /// <param name="name">Получить пользователя по имени</param>
        /// <returns></returns>
        public User? GetUserByName(string name) 
        {
            var item = _dbContext.users.FirstOrDefault(x => x.UserName==name);
            return item;
        }
        /// <summary>
        /// Обновить пароль
        /// </summary>
        /// <param name="user"> Экземпляр User c обновленным паролем</param>
        /// <returns></returns>
        public User UpdatePassword(User user)   
        {
            var currentUser = _dbContext.users.FirstOrDefault(x => x.userId == user.userId);
            currentUser.Password = user.Password;
            _dbContext.SaveChanges();
            return currentUser;
        }
        /// <summary>
        /// Поиск и получение валидного пользователя из базы данных
        /// </summary>
        /// <param name="user">Экземпляр искомого пользователя</param>
        /// <returns></returns>
        public User? GetValidUser(User user) 
        {
            var currentUser=_dbContext.users.Where(
                u => u.UserName == user.UserName).FirstOrDefault();
            if (currentUser != null) {
                var result = new PasswordHasher<User>().VerifyHashedPassword(user, currentUser.Password, user.Password);
                if(result==PasswordVerificationResult.Success) return currentUser;
            }
            return null;
        }
        /// <summary>
        /// Сохранение сообщения пользователя в базе дыннх
        /// </summary>
        /// <param name="message">Сообщение пользователя</param>
        /// <param name="dateTime">Дата отправки</param>
        /// <param name="conversationId">Id беседы к которому относится сообщение</param>
        /// <returns></returns>
        public async Task SaveMessage(string message,DateTime dateTime,int conversationId)
        {
            Message smgToSave = new Message() {MessageText=message,SentTime=dateTime,conversationId=conversationId };
            await _dbContext.messages.AddAsync(smgToSave);
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        ///  Регистрация членов беседы при первой отправке сообщения
        /// </summary>
        /// <param name="firstUser">Первый пользователь</param>
        /// <param name="secondUser">Второй пользователь</param>
        /// <param name="conversationId">Id беседы</param>
        /// <returns></returns>
        public async Task RegisterMembers(HubUser firstUser,HubUser secondUser,int conversationId )
        {
            int id1 = int.Parse(firstUser.UserIdentifier);
            int id2 = int.Parse(secondUser.UserIdentifier);
            _dbContext.members.Add(new ConversationMember() { userId=id1,conversationId=conversationId});
            _dbContext.members.Add(new ConversationMember() { userId = id2, conversationId = conversationId });
            await _dbContext.SaveChangesAsync();
        }
        private HubUser ToHubUserModel(User user)
        {
            var result= new HubUser() { 
                UserIdentifier=user.userId.ToString(),
                Name=user.UserName
            };  
            return result;
        }
        /// <summary>
        /// Получение полной переписки из базы данных
        /// </summary>
        /// <param name="conversationName">Название беседы из которой будет получена история переписки</param>
        /// <returns></returns>

       public async Task<List<Message>> GetMessageList(string conversationName)
        {
            var searchedConversation =_dbContext.conversations.Where(x=>x.ConversationName==conversationName).FirstOrDefault();
            var result = await _dbContext.messages.Where(x => x.conversationId == searchedConversation.conversationId).ToListAsync();
            return result;
        }
    }
}
