using Database;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SensorizMonitoring.Models;
using System.Data;
using System.Diagnostics;

namespace SensorizMonitoring.Business
{
    public class bnDevice
    {
        private readonly IConfiguration _configuration;

        public bnDevice(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        Globals utl = new Globals();

        public class Include
        {
            public bool pairings { get; set; }
        }
        public class DeviceInformationRequest
        {
            public string device { get; set; }
            public Include include { get; set; } 
        }

        public async Task<dynamic> GetDeviceInformationByDeviceId(string sDeviceId)
        {
            try
            {
                string sql = string.Empty;

                //var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "Data", "family_model.json");
                //var conteudoJson = File.ReadAllText(caminhoArquivo);

                var baseUrl = _configuration["Settings:LocoBaseUrlRPC"];
                var token = _configuration["Settings:LocoToken"];
                var endpoint = "getDevice";

                Include inc = new Include();
                inc.pairings = false;

                DeviceInformationRequest dir = new DeviceInformationRequest();
                dir.device = sDeviceId;
                dir.include = inc;

                var apiClient = new SensorizMonitoring.Utils.ApiClient(baseUrl, token);
                var apiData = await apiClient.PostApiDataAsync(endpoint, dir);

                Console.WriteLine(apiData);

                if (!string.IsNullOrEmpty(apiData))
                {
                    return apiData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return null;
            }
        }
    }
}
