namespace BangazonWorkforce.IntegrationTests
{
    public static class Config
    {
        public static string ConnectionSring
        {
            get
            {
                return "Server=RUSSELL-SURFACE\\SQLEXPRESS;Initial Catalog=BangazonSprint;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            }
        }
    }
}
