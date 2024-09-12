using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models.NotificationOwner;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiKey]
    [ApiController]
    public class NotificationOwnerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public NotificationOwnerController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Insere Owner da Notificação de Monitoramento
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertNotificationOwner([FromBody] NotificationOwnerModel nom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var insertNotificationOwner = new NotificationOwner();

                insertNotificationOwner.device_id = nom.device_id;
                insertNotificationOwner.description = nom.description;
                insertNotificationOwner.notification_type_id = nom.notification_type_id;
                insertNotificationOwner.phone_number = nom.phone_number;
                insertNotificationOwner.mail = nom.mail;
                insertNotificationOwner.enabled = 1;
                insertNotificationOwner.created_at = DateTime.Now;

                _context.Add(insertNotificationOwner);
                _context.SaveChanges();
                return Ok(insertNotificationOwner);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Atualiza o Owner da Notificação de Monitoramento
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotificationOwner(int id, [FromBody] NotificationOwner nom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingNotificationOwner = _context.NotificationOwner.Find(id);

                existingNotificationOwner.device_id = nom.device_id;
                existingNotificationOwner.description = nom.description;
                existingNotificationOwner.notification_type_id = nom.notification_type_id;
                existingNotificationOwner.phone_number = nom.phone_number;
                existingNotificationOwner.mail = nom.mail;

                if (existingNotificationOwner == null)
                {
                    return NotFound("Device not found.");
                }

                _context.Update(existingNotificationOwner);
                _context.SaveChanges();
                return Ok(existingNotificationOwner);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Ativa e desativa o Owner da Notificação de Monitoramento
        /// </summary>
        [HttpPut("{id}/{flag}")]
        public async Task<IActionResult> EnableDisableNotificationOwner(int id, int flag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingNotificationOwner = await _context.NotificationOwner.FindAsync(id);

                existingNotificationOwner.enabled = flag;

                if (existingNotificationOwner == null)
                {
                    return NotFound("Notification Owner not found.");
                }

                _context.Update(existingNotificationOwner);
                _context.SaveChanges();
                return Ok(existingNotificationOwner);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Remove o Owner da Notificação de Monitoramento
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> RemoveNotificationOwner([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var delNotificationOwner = await _context.NotificationOwner.FindAsync(id);
                _context.Remove(delNotificationOwner);
                _context.SaveChanges();
                return Ok("Registro removido!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Lista os Owners da Notificação de Monitoramento por código de dispositivo
        /// </summary>
        [HttpGet("{device_id}")]
        public async Task<IActionResult> GetNotificationOwnerByDeviceID(long device_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                List<NotificationOwner> no = await _context.NotificationOwner
                  .Where(d => d.device_id == device_id)
                  .AsNoTracking()
                  .ToListAsync();
                                     

                return Ok(no);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
    }
}