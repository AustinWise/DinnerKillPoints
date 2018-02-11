using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DkpWeb.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }
    }
}
