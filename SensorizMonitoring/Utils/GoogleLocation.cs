using Newtonsoft.Json;

namespace SensorizMonitoring.Utils
{
    public class GoogleLocation
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleLocation(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        public string GetAddressByCoordinators(double lat, double lng)
        {
            Globals gb = new Globals();
            string sLat = gb.DotChange(lat);
            string sLng = gb.DotChange(lng);

            // Defina a chave de API do Google Maps
            var apiKey = _configuration["Settings:GOOGLE_API_TOKEN"];

            string fullUrl = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={sLat},{sLng}&key={apiKey}";


            // Crie uma solicitação de geocodificação reversa
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl.Trim());

            // Envie a solicitação e obtenha a resposta
            var httpClient = new HttpClient();
            var response = httpClient.SendAsync(request).Result;

            // Verifique se a resposta foi bem-sucedida
            if (response.IsSuccessStatusCode)
            {
                // Obtenha o conteúdo da resposta
                var responseBody = response.Content.ReadAsStringAsync().Result;

                // Deserializar o conteúdo em um objeto JSON
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // Obtenha o endereço formatado
                var address = jsonData.results[0].formatted_address;

                return address;
            }
            else
            {
                var errorResponse = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"Error: {errorResponse}");
                return "Não identificado";
            }
        }

        public string GetGoogleAddress(double lat, double lon)
        {
            // Check if the latitude or longitude values are invalid
            if (lat == 0.0 || double.IsNaN(lat) || lon == 0.0 || double.IsNaN(lon))
            {
                return "Não encontrado";
            }

            GoogleLocation gl = new GoogleLocation(_configuration);
            return gl.GetAddressByCoordinators(lat, lon);
        }
    }
}
