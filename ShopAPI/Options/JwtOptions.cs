using System;

namespace ShopAPI.Options
{
    public class JwtOptions
    {
        public string Secret { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
    }
}
