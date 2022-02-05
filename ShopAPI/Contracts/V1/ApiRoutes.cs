namespace ShopAPI.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string root = "api";

        public const string version = "v1";

        public const string Base = $"{root}/{version}";

        public static class Products
        {
            public const string GetAll = Base + "/products";
            public const string Create = Base + "/products";
            public const string Get = Base + "/products/{id}";
            public const string Update = Base + "/products/{id}";
            public const string Delete = Base + "/products/{id}";
        }

        public static class Auth
        {
            public const string Login = Base + "/auth/login";
            public const string Register = Base + "/auth/register";
        }
    }
}
