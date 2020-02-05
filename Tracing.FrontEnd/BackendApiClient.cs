using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tracing.FrontEnd
{
    public class BackendApiClient
    {
        private readonly HttpClient _client;

        public BackendApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<WeatherForecast>> GetForecast()
        {
            var response = await _client.GetAsync("https://localhost:5002/WeatherForecast");
            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<WeatherForecast>>(stringResponse);
        }
    }
}