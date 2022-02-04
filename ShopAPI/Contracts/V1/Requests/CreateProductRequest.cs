using System;
using System.ComponentModel.DataAnnotations;

namespace ShopAPI.Contracts.V1.Requests
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
    }
}
