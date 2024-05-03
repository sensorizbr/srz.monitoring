using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DeviceController : Controller
    {
        private readonly IConfiguration _configuration;

        public DeviceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Insere o dispositivo apenas na base da Sensoriz
        /// </summary>
        [HttpPost]
        public IActionResult InsertDevice([FromBody] DeviceModel device)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            Device dvc = new Device(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a inserction...");

            // Lógica para manipular a solicitação POST

            if (!dvc.DeviceExists(device.device_code.Trim()))
            {
                if (dvc.InsertDevice(device))
                {
                    utl.EscreverArquivo("Alright!");
                    return Ok($"Received!");
                }
                else
                {
                    utl.EscreverArquivo("Was not possible to insert");
                    return BadRequest($"Ooops!");
                }
            }
            else
            {
                utl.EscreverArquivo("Device already exists! " + device.device_code + " - " + device.description);
                return BadRequest("Device already exists!");
            }
        }

        /// <summary>
        /// Pega as informações do Dispositivo na API da LocoAware pelo Device Code.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDeviceInformationByDeviceIdAsync(string sDeviceId)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            Device dvc = new Device(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a Listing...");

            // Lógica para manipular a solicitação POST

            return Content(await dvc.GetDeviceInformationByDeviceId(sDeviceId));
        }
    }
}