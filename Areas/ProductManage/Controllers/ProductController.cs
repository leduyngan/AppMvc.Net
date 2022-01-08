using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App.Controllers
{
    [Area("ProductManage")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductController> _iLogger;

        public ProductController(ProductService productService, ILogger<ProductController> iLogger)
        {
            _productService = productService;
            _iLogger = iLogger;
        }
        [Route("/cac-san-pham/{id?}")]
        public IActionResult Index()
        {
            var products = _productService.OrderBy(p => p.Name).ToList();
            return View(products);
        }
    }
}