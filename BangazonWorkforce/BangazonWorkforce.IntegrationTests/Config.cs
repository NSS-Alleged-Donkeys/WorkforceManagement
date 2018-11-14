namespace BangazonWorkforce.IntegrationTests
{
    public static class Config
    {
        public static string ConnectionSring
        {
            get
            {
                return "Server=DESKTOP-S7R9EFR\\SQLEXPRESS;Initial Catalog=Bangazon;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            }
        }
    }
}
