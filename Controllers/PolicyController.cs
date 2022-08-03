using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CoreBT.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PolicyController : Controller
    {
        public IActionResult Roles()
        {
            return View();
        }

        public IActionResult RolePrivillages()
        {
            return View();
        }

        public IActionResult ApplicationUsers()
        {
            return View();
        }
    }
}
