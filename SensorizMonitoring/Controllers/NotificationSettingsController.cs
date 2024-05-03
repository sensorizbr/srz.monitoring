using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Models;
using SensorizMonitoring.Models.NotificationsSettings;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class NotificationSettingsController : Controller
    {
        private readonly IConfiguration _configuration;

        public NotificationSettingsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Cria a configuração de uma notificação de um determinado dispositivo
        /// </summary>
        [HttpPost]
        public IActionResult InsertNotificationSettings([FromBody] NotificationSettingsModel ns)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            NotificationSettings mnt = new NotificationSettings(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a inserction...");

            // Lógica para manipular a solicitação POST

            if (mnt.InsertNotificationSettings(ns))
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

        /// <summary>
        /// Atualiza a configuração de uma notificação de um determinado dispositivo
        /// </summary>
        [HttpPut]
        public IActionResult UpdateNotificationSettings([FromBody] NotificationSettingsModel company, int id)
        {
            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            NotificationSettings mnt = new NotificationSettings(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a update...");

            // Lógica para manipular a solicitação POST


            if (mnt.UpdateNotificationSettings(company, id))
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

        /// <summary>
        /// Exclui a configuração de uma notificação de um determinado dispositivo
        /// </summary>
        [HttpPut]
        public IActionResult DeleteNotificationSettings(int id)
        {
            NotificationSettings mnt = new NotificationSettings(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a Deletion...");


            if (mnt.DeleteNotificationSettings(id))
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

        /// <summary>
        /// Ativa ou Desativa a configuração de uma notificação de um determinado dispositivo
        /// </summary>
        [HttpPut]
        public IActionResult EnableDisableNotificationSettings(int id, int flag)
        {
            NotificationSettings mnt = new NotificationSettings(_configuration);
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a update...");


            if (mnt.DisableEnableNotificationSettings(id, flag))
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


        /// <summary>
        /// Lista todas as configurações de uma notificação de um determinado dispositivo
        /// </summary>
        [HttpGet]
        public IActionResult GetNotificationSettings(int limit, int page)
        {
            NotificationSettings mnt = new NotificationSettings(_configuration);
            Globals utl = new Globals();

            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            utl.EscreverArquivo("Getting data...");

            // Lógica para manipular a solicitação POST
            dynamic ret = mnt.GetNotificationSettings(limit, page);

            return Ok(ret);
        }

        /// <summary>
        /// Lista a configuração de uma notificação de um determinado dispositivo por ID da Configuração
        /// </summary>
        [HttpGet]
        public IActionResult GetNotificationSettingsById(int id)
        {
            NotificationSettings mnt = new NotificationSettings(_configuration);
            Globals utl = new Globals();

            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            utl.EscreverArquivo("Getting data...");

            // Lógica para manipular a solicitação POST
            dynamic ret = mnt.GetNotificationSettingsById(id);

            if (ret.Count == 0)
            {
                return NoContent();
            }

            return Ok(ret);
        }

        /// <summary>
        /// Lista a configuração de uma notificação de um determinado dispositivo por ID da Configuração
        /// </summary>
        [HttpGet]
        public IActionResult GetNotificationSettingsByDeviceID(string DeviceID)
        {
            NotificationSettings mnt = new NotificationSettings(_configuration);
            Globals utl = new Globals();

            //MonitoringModel monitoring = JsonConvert.DeserializeObject<MonitoringModel>(value);
            utl.EscreverArquivo("Getting data...");

            // Lógica para manipular a solicitação POST
            dynamic ret = mnt.GetNotificationSettingsByDeviceId(DeviceID);

            if (ret.Count == 0)
            {
                return NoContent();
            }

            return Ok(ret);
        }
    }
}