using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreBT.Controllers
{

    [Authorize(Roles = "Admin")]
    public class FormController : Controller
    {

        public IActionResult Index()
        {
            var guid = Guid.NewGuid().ToString();
            TempData["giud"]=guid;
            return View();
        }

        public IActionResult Document(string Type)
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }
    }
}
