using System.IO;
using System.Linq;
using App.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App.Controllers
{
    public class FirstController : Controller
    {
        ILogger<FirstController> _logger;
        ProductService _productService;
        [TempData]
        public string StatusMessage { set; get; }
        public FirstController(ILogger<FirstController> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public string Index()
        {
            _logger.LogWarning("Thong bao");
            _logger.LogError("Thong Bao");
            _logger.LogDebug("Thong Bao");
            _logger.LogCritical("Thong Bao");
            _logger.LogInformation("Index Action");
            return "Toi la Index cua FirstController";
        }
        public void Nothing()
        {
            _logger.LogInformation("Nothing Action");
            Response.Headers.Add("hi", "Xin chao cac ban");
        }
        public IActionResult ReadMe()
        {
            var contet = @"
            Xin chao,
            cac ban dang hoc ve ASP.NET MVC
            
            
            
            
            
            XUANTHULAB.NET
            ";
            return Content(contet, "text/plain");
        }
        public IActionResult Bird()
        {
            string filePath = Path.Combine(Startup.ContentRootPath, "Files", "bird.jpg");
            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "image/jpg");
        }
        public IActionResult IphonePrice()
        {
            return Json(
                new
                {
                    product = "Iphon X",
                    price = 1000
                }
            );
        }
        public IActionResult Privacy()
        {
            var url = Url.Action("Privacy", "Home");
            return LocalRedirect(url);
        }
        public IActionResult Google()
        {
            var url = "https://www.google.com/";
            return Redirect(url);
        }
        public IActionResult HelloView(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = "Khach";
            }
            //return View("xinchao2", userName);
            //return View((object)userName);
            return View("xinchao3", userName);
        }

        public IActionResult ViewProduct(int? id)
        {
            var product = _productService.Where(p => p.Id == id).FirstOrDefault();
            if (product == null)
            {
                //TempData["StatusMessage"] = "San pham ban yeu cau khong co";
                StatusMessage = "San pham ban yeu cau khong co";
                return Redirect(Url.Action("Index", "Home"));
            }
            // return View(product);
            // this.ViewData["product"] = product;
            // ViewData["title"] = product.Name;
            // return View("ViewProduct2");
            ViewBag.product = product;
            return View("ViewProduct3");
        }
    }
}