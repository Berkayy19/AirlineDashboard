using System.Net.Http;
using System.Net.Http.Headers;

namespace FlightBackend.Services
{
    public class LufthansaService
    {
        private readonly HttpClient _httpClient;
        private readonly LufthansaAuthService _auth;

        public LufthansaService(HttpClient httpClient, LufthansaAuthService auth)
        {
            _httpClient = httpClient;
            _auth = auth;
        }

        public async Task<string> GetFlightsAsync(string from, string to, string date)
        {
            var token = await _auth.GetTokenAsync();

            var url = $"https://api.lufthansa.com/v1/operations/schedules/{from}/{to}/{date}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetFlightStatusByNumberAsync(string flightNumber, string date)
        {
            var token = await _auth.GetTokenAsync();

            var url = $"https://api.lufthansa.com/v1/operations/flightstatus/{flightNumber}/{date}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
