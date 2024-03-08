using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : Controller
    {
        private readonly IConfiguration _configuration;

        public MonitoringController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Rota POST: api/Exemplo
        [HttpPost]
        public IActionResult Post([FromBody] MonitoringModel monitoring)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            Monitoring mnt = new Monitoring(_configuration);
            Utils utl = new Utils();
            utl.EscreverArquivo("----------------------");
            utl.EscreverArquivo("DEVICE ID: " + monitoring.deviceId);
            utl.EscreverArquivo("STATUS: " + monitoring.status.batteryState);
            utl.EscreverArquivo("TEMPERATURA: " + monitoring.status.temperature);
            utl.EscreverArquivo("PRESSÃO ATMOSFERICA: " + monitoring.status.atmosphericPressure);
            utl.EscreverArquivo("LONGITUDE: " + monitoring.pos.lon);
            utl.EscreverArquivo("LATITUDE: " + monitoring.pos.lat);
            utl.EscreverArquivo("CEP: " + monitoring.pos.cep);
            utl.EscreverArquivo("----------------------");
            // Lógica para manipular a solicitação POST
            if (mnt.InsertMonitoring(monitoring))
            {
                return Ok($"Recebido!");
            }
            else
            {
                return BadRequest($"Ooops!");
            }
        }
    }
}