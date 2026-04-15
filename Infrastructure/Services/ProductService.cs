using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        public string GetStatus() => "Product service is working";
    }
}
