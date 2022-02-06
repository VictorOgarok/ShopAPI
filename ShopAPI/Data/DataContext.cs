using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopAPI.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopAPI.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
