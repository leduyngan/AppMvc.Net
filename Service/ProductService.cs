using System.Collections.Generic;
using App.Models;

namespace App.Service
{
    public class ProductService : List<ProductModel>
    {
        public ProductService()
        {
            this.AddRange(new ProductModel[]{
                new ProductModel(){Id = 1, Name = "Iphone X", Price = 1000},
                new ProductModel(){Id = 2, Name = "Samsung Abc", Price = 2000},
                new ProductModel(){Id = 3, Name = "Sony XYZ", Price = 3000},
                new ProductModel(){Id = 4, Name = "Nokia BCS", Price = 4000},
            });
        }
    }
}