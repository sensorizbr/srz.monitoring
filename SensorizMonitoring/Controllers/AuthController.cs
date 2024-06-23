using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Crud;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Models;
using SensorizMonitoring.Models.DeviceReference;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Rota POST: api/Exemplo
        [HttpPost]
        public IActionResult AuthLogin([FromBody] AuthLoginModel credentials)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            Auth auth = new Auth(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a Login...");

            var responseFail = new
            {
                logged = false
            };

            try
            {
                // Lógica para manipular a solicitação POST
                var ret = auth.Login(credentials);

                if (ret.logged)
                {
                    ret.token = auth.GenerateToken(ret);

                    return Ok(ret);
                }
                else
                {
                    return Ok(responseFail);
                };
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
    }
}