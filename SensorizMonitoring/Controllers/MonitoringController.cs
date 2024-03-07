using Microsoft.AspNetCore.Mvc;

namespace SensorizMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : Controller
    {
        // Rota POST: api/Exemplo
        [HttpPost]
        public IActionResult Post([FromBody] dynamic value)
        {
            var apiKey = Request.Headers["X-Api-Key"];

            //if(apiKey.Equals(""))
            Utils utl = new Utils();
            utl.EscreverArquivo("Ret: " + value);
            // Lógica para manipular a solicitação POST
            return Ok($"Método POST com valor = {value}");
        }
    }
}