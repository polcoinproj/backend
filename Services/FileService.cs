namespace backend.Services
{
    public class FileService<T>
    {
        private IConfiguration _configuration { get; }
        private string _prefix;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;

            _prefix = configuration.GetSection("Storage").GetValue<string>(typeof(T).Name.Replace("Controller", "") + "Path")!;
        }

        public async Task<string> Upload(IFormFile file)
        {
            if (file == null)
            {
                return "";
            }

            IConfigurationSection pathsSection = _configuration.GetSection("Storage");
            string folder = Path.Combine(pathsSection.GetValue<string>("Path")!, pathsSection.GetValue<string>("AchievementPath")!);
            Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString();
            string url = new Uri(new Uri(_configuration.GetValue<string>("CurrentHost")!), Path.Combine("/static", pathsSection.GetValue<string>("AchievementPath")!, fileName)).ToString();

            FileStream fileStream = new FileStream(Path.Combine(folder, fileName), FileMode.Create, FileAccess.ReadWrite);
            await file.CopyToAsync(fileStream);

            return url;
        }
    }
}