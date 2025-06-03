using MedhatFileNet.Models;
using Newtonsoft.Json;

namespace MedhatFileNet.Services
{
    public class FileNetService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public FileNetService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<FileNetDocument>> GetDocumentsAsync()
        {
            var url = _config["FileNetApi:BaseUrl"] + "/documents";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FileNetDocument>>(content);
        }
    }
}
