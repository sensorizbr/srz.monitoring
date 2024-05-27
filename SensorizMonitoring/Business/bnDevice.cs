using Database;
using MySql.Data.MySqlClient;
using Nancy.Json;
using Newtonsoft.Json;
using SensorizMonitoring.Models;
using System.Data;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                var baseUrl = _configuration["Settings:LocoBaseUrlRPC"];
                var token = _configuration["Settings:LocoToken"];
                var endpoint = "getDevice";

                Include inc = new Include();
                inc.pairings = false;

                DeviceInformationRequest dir = new DeviceInformationRequest();
                dir.device = sDeviceId;
                dir.include = inc;

                var apiClient = new SensorizMonitoring.Utils.ApiClient(baseUrl, token);
                string apiData = await apiClient.PostApiDataAsync(endpoint, dir);

                Console.WriteLine(apiData);

                if (!string.IsNullOrEmpty(apiData))
                {
                    // Deserialize the apiData string to a dynamic object
                    dynamic jsonData = JsonConvert.DeserializeObject(apiData);
                    return jsonData;
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

        public async Task<LocoDevicesResponse> GetDevicesToList()
        {
            try
            {
                string sql = string.Empty;

                var baseUrl = _configuration["Settings:LocoBaseUrl"];
                var token = _configuration["Settings:LocoToken"];
                var owner = _configuration["Settings:LocoOwner"];
                var endpoint = "devices";

                Include inc = new Include();
                inc.pairings = false;

                Filters flt = new Filters();
                flt.owners.Add(owner);

                Sort srt = new Sort();
                srt.by = "id";
                srt.direction = "asc";

                LocoPaginationRequest pag = new LocoPaginationRequest();
                pag.limit = 1000;
                pag.filters = flt;
                pag.sort = srt;

                string json = JsonConvert.SerializeObject(pag);

                // imprima a string JSON
                Console.WriteLine(json);

                var apiClient = new SensorizMonitoring.Utils.ApiClient(baseUrl, token);
                string apiData = await apiClient.PostApiDataAsync(endpoint, pag);

                Console.WriteLine(apiData);

                if (!string.IsNullOrEmpty(apiData))
                {
                    // Deserialize the apiData string to a dynamic object
                    var serializer = new JavaScriptSerializer();
                    LocoDevicesResponse deviceInfo = serializer.Deserialize<LocoDevicesResponse>(apiData);

                    //return deviceInfo;
                    return deviceInfo;
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
