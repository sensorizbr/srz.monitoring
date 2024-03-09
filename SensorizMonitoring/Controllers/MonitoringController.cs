using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
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
        public IActionResult InsertMonitoring([FromBody] MonitoringModel monitoring)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            Monitoring mnt = new Monitoring(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a inserction...");

            // Lógica para manipular a solicitação POST
            if (mnt.InsertMonitoring(monitoring))
            {
                utl.EscreverArquivo("Alright!");
                return Ok($"Recebido!");
            }
            else
            {
                utl.EscreverArquivo("Was not possible to insert");
                return BadRequest($"Ooops!");
            }
        }
    }
}