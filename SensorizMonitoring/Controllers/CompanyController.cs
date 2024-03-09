using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private readonly IConfiguration _configuration;

        public CompanyController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Rota POST: api/Exemplo
        [HttpPost]
        public IActionResult InsertCompany([FromBody] CompanyModel company)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            Company mnt = new Company(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a inserction...");

            // Lógica para manipular a solicitação POST

            if (!mnt.CompanyExists(company.document.Trim()))
            {
                if (mnt.InsertCompany(company))
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
            else
            {
                utl.EscreverArquivo("Company already exists! " + company.document + " - " + company.name);
                return BadRequest("Company already exists!");
            }
        }

        [HttpPut]
        public IActionResult UpdateCompany([FromBody] CompanyModel company, int id)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            Company mnt = new Company(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a update...");

            // Lógica para manipular a solicitação POST


            if (mnt.UpdateCompany(company, id))
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

        [HttpPut]
        public IActionResult DeleteCompany(int id)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            Company mnt = new Company(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a Deletion...");


            if (mnt.DeleteCompany(id))
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

        [HttpPut]
        public IActionResult EnableDisableCompany(int id, int flag)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            Company mnt = new Company(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a update...");


            if (mnt.DeleteCompany(id))
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

        [HttpGet]
        public IActionResult GetCompanies(int limit, int page)
        {
            Company mnt = new Company(_configuration);
            Globals utl = new Globals();

            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            utl.EscreverArquivo("Getting data...");

            // Lógica para manipular a solicitação POST
            dynamic ret = mnt.GetCompanies(limit, page);

            return Ok(ret);
        }

        [HttpGet]
        public IActionResult GetCompanyById(int id)
        {
            Company mnt = new Company(_configuration);
            Globals utl = new Globals();

            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            utl.EscreverArquivo("Getting data...");

            // Lógica para manipular a solicitação POST
            dynamic ret = mnt.GetCompanyById(id);

            if (ret.Count == 0)
            {
                return NoContent();
            }

            return Ok(ret);
        }
    }
}