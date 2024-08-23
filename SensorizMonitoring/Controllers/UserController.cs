using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Business;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using System.Text;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public UserController(IConfiguration configuration, AppDbContext context, ILogger<UserController> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Insere o Usuario
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertUser([FromBody] UserModel user)
        {
            try
            {
                var insertUser = new User();

                insertUser.branch_id = user.branch_id;
                insertUser.document = user.document;
                insertUser.full_name = user.full_name;
                insertUser.mail = user.mail;
                insertUser.phone_number = user.phone_number;
                insertUser.password = Encoding.UTF8.GetBytes(user.password);
                insertUser.enabled = 0;
                insertUser.created_at = DateTime.Now;

                _context.Add(insertUser);
                _context.SaveChanges();
                return Ok(insertUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Atualiza o usuário
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = _context.User.Find(id);

                existingUser.branch_id = user.branch_id;
                existingUser.document = user.document;
                existingUser.full_name = user.full_name;
                existingUser.mail = user.mail;
                existingUser.phone_number = user.phone_number;

                if (existingUser == null)
                {
                    return NotFound("User not found.");
                }

                _context.Update(existingUser);
                _context.SaveChanges();
                return Ok(existingUser);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error updating user: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var delUser = _context.User.Find(id);
                _context.Remove(delUser);
                _context.SaveChanges();
                return Ok("Registro removido!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Ativa/Desativa Usuário. Basta passar o id e a flag, sendo 1 para ativar e 0 para desativar.
        /// </summary>
        [HttpPut("{id}/{flag}")]
        public async Task<IActionResult> EnableDisableUser(int id, int flag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = await _context.User.FindAsync(id);

                existingUser.enabled = flag;

                if (existingUser == null)
                {
                    return NotFound("User not found.");
                }

                _context.Update(existingUser);
                _context.SaveChanges();
                return Ok(existingUser);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error updating user: " + ex.Message);
            }
        }

        /// <summary>
        /// Lista todas as configurações de forma paginada
        /// </summary>
        [HttpGet("{limit}/{page}")]
        public async Task<IActionResult> GetAllUsersByBranch(int iBranchId, int limit, int page)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingUser = await _context.User

                    .AsNoTracking()
                    .Where(c => c.branch_id == iBranchId)
                    .Select(c => new
                    {
                        // Selecione apenas as colunas que você precisa
                        c.id,
                        c.full_name,
                        c.mail,
                        c.branch_id,
                        c.created_at
                    })
                    .OrderBy(c => c.created_at)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                return Ok(existingUser);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Users: " + ex.Message);
            }
        }
    }
}