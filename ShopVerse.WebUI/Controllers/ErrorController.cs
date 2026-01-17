using Microsoft.AspNetCore.Mvc;

namespace ShopVerse.WebUI.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Üzgünüz, aradığınız sayfayı bulamadık.";
                    return View("NotFound");

            }

            return View("Error");
        }
    }
}