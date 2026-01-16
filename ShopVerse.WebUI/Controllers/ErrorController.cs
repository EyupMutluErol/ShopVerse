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
                    // 404 için özel view döndür
                    return View("NotFound");

                    // İleride 500 veya 403 hataları için de case ekleyebilirsin.
            }

            return View("Error");
        }
    }
}