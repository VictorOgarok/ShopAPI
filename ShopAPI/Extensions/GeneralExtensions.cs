using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopAPI.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserByid(this HttpContext httpContext)
        {
            if (httpContext.User==null)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.Single(i => i.Type == "id").Value;
        }
    }
}
