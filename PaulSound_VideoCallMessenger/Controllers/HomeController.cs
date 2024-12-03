using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaulSound_VideoCallMessenger.Data;
using PaulSound_VideoCallMessenger.Mapper;
using PaulSound_VideoCallMessenger.Models;
using PaulSound_VideoCallMessenger.Services;
using System.Diagnostics;

namespace PaulSound_VideoCallMessenger.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly LoginService _loginService;
        public HomeController(ILogger<HomeController> logger,IMapper mapper,LoginService login)
        {
            _logger = logger;
            _mapper = mapper;
            _loginService = login;
        }
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Chat");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserModel user)
        {
           
            var mapperUser = _mapper.Map<UserModel, User>(user);
            var validUser=_loginService.GetValidUser(mapperUser);
            if (validUser == null)
            {
                TempData["logProblem"] = "Пользователя с указанным логином ил паролем не существует";
                return View(user);
            }
            await _loginService.CreateAuthentication(validUser, HttpContext);// аутифицируем пользователя
            return RedirectToAction("Index", "Chat");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","Chat");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            if(ModelState.IsValid)
            {
                var mappedUser = _mapper.Map<UserModel, User>(user);
                var foundUserByName = _loginService.GetUserByUsername(mappedUser.UserName);
                if (foundUserByName != null)
                {
                    TempData["error"] = "Данный пользователь с указанным ником уже существует";
                    return RedirectToAction("Index","Chat");
                }
                else
                {
                    _loginService.RegisterUser(mappedUser);
                    ModelState.Clear();
                    TempData["success"] = "Пользователь был успешно зарегестрирован";
                    return RedirectToAction("Index");
                }
            }
            if(!ModelState.IsValid)
            {
                return Register();
            }
            return LocalRedirect("~/Home/Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            var user = _loginService.GetUserByClaim(User);
            if(user==null)
            {
                TempData["logProblem"] = "Ошибка аккаунта. Перезайдите в учетную запись и попробуйте снова";
                return RedirectToAction("Index","Chat");
            }
            var userModel = _mapper.Map<User,UserModel>(user);
            return View(userModel);
        }
    }
}
