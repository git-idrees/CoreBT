using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBT.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class Project : Controller
    {
      
        public IActionResult List(string alert)
        {
            return View();
        }

        public IActionResult Setup()
        {
            return View();
        }

        public IActionResult Detail(string access)
        {
            return View();
        }
    }
}
