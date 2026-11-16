using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ProductInventory.PL.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Error()
        {
            ViewBag.Code = 500;
            ViewBag.Message = "Something went wrong while processing your request.";
            return View();
        }

        public IActionResult HttpStatus(int code)
        {
            ViewBag.Code = code;
            ViewBag.Message = code == 404
                ? "The page or product you are looking for does not exist."
                : "The request could not be completed.";
            return View("Error");
        }
    }
}

