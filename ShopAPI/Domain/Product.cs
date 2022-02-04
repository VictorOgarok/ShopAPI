using System;
using System.ComponentModel.DataAnnotations;

namespace ShopAPI.Domain
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
