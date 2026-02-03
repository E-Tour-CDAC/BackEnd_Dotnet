namespace Backend_dotnet.Services.Implementations
{
    using Backend_dotnet.DTOs;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;

    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("AuthService");
        }

        // REGISTER
        public async Task<bool> RegisterAsync(RegisterRequestDto dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/register", content);

            return response.IsSuccessStatusCode;
        }

        // LOGIN
        public async Task<string?> LoginAsync(LoginRequestDto dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponseDto>(
                responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Token;
        }
    }

}
