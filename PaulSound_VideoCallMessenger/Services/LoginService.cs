using PaulSound_VideoCallMessenger.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace PaulSound_VideoCallMessenger.Services
{
    public class LoginService
    {
        private readonly DatabaseService _databaseService;
        public LoginService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        public User? GetUserById(int id)
        {
            var foundUser = _databaseService.GetUserById(id);
            return foundUser;
        }
        public User? GetUserByUsername(string user) 
        {
            var foundUserByUsername = _databaseService.GetUserByName(user);
            return foundUserByUsername;
        }
        public User UpdateUserPassword(User user, string newPassword) // хэширование нужно сменить
        {

            string hashedPassword = new PasswordHasher<User>().HashPassword(user, newPassword);
            user.Password =hashedPassword;
            return _databaseService.UpdatePassword(user);
        }
        public User? GetValidUser(User user)
        {
            var validUser= _databaseService.GetValidUser(user);
            return validUser;
        }
        public void RegisterUser(User user) // Может стоить поменять способ хэширования пароля на что то более новое?
        {
            var hashedUser = new User()
            {
                
                UserName = user.UserName,
                Password = user.Password, // поменяй способ хэширования
                Email = user.Email,
                UniqueIdentifier=$"@{user.UserName}{new Random().Next(10000)}{user.userId}"
            };
            var result = new PasswordHasher<User>().HashPassword(hashedUser, user.Password);
            hashedUser.Password = result;
            _databaseService.RegisterNewUser(hashedUser);
        }
        public async Task CreateAuthentication(User user, HttpContext context) 
        {
            var claims = new List<Claim> 
            {
                 new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                 new Claim(ClaimTypes.Name, user.UserName)
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme); 
            var principal = new ClaimsPrincipal(identity);
            var props = new AuthenticationProperties();

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                props);
        }
        public User? GetUserByClaim(ClaimsPrincipal principal) 
        {
      
            var userClaim = principal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => c.Value).SingleOrDefault();
            int userId;

            if (!int.TryParse(userClaim, out userId))
            {
                return null;
            }

            return GetUserById(userId);
        }

    }
}
