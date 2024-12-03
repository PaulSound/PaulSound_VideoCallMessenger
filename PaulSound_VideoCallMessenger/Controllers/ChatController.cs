using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaulSound_VideoCallMessenger.Services;

namespace PaulSound_VideoCallMessenger.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        public ChatController(IMapper mapper,ILogger<HomeController>logger) 
        { 
            _mapper = mapper;
            _logger = logger;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
       
    }
}
