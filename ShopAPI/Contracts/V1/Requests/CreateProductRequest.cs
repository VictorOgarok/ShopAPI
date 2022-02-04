using System;
using System.ComponentModel.DataAnnotations;

namespace ShopAPI.Contracts.V1.Requests
{
    public class CreateProductRequest
    {
        public Guid Id { get; set; }
    }
}
