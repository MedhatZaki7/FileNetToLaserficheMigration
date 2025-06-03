using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using MedhatFileNet.Models;

namespace MedhatFileNet.Services
{
    public class LaserficheService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public LaserficheService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var tokenUrl = _config["Laserfiche:OAuthEndpoint"];
            var values = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _config["Laserfiche:ClientId"] },
                { "client_secret", _config["Laserfiche:ClientSecret"] },
                { "scope", "repository.Read repository.Write" }
            };
            var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<dynamic>(json);
            return (string)obj.access_token;
        }

        public async Task<bool> UploadDocumentAsync(LaserficheDocument doc)
        {
            var token = await GetAccessTokenAsync();
            var url = $"{_config["Laserfiche:ApiBaseUrl"]}/Repositories/{_config["Laserfiche:RepositoryId"]}/Entries/{doc.ParentEntryId}/Folder/Import";

            using var multipart = new MultipartFormDataContent();
            multipart.Add(new ByteArrayContent(doc.FileContent), "file", doc.FileName);

            var metadata = new
            {
                name = doc.FileName,
                autoRename = true,
                metadata = new
                {
                    templateName = doc.TemplateName,
                    fields = doc.Metadata.Select(m => new {
                        name = m.Key,
                        values = new[] { m.Value }
                    })
                }
            };

            var metaContent = new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json");
            multipart.Add(metaContent, "request");

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = multipart;

            var result = await _httpClient.SendAsync(request);
            return result.IsSuccessStatusCode;
        }
    }
}
