namespace backend.Services
{
    public class EmailChecker
    {
        private const string POOL_URI = "https://raw.githubusercontent.com/FGRibreau/mailchecker/master/list.txt";

        public static string[] Pool = new string[] { };

        public EmailChecker()
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(POOL_URI)
            };

            string res = client.GetAsync(POOL_URI).Result.Content.ReadAsStringAsync().Result;

            Pool = res.Split("\n");
        }

        public bool IsBlacklisted(string email)
            => Pool.Contains(email);
    }
}