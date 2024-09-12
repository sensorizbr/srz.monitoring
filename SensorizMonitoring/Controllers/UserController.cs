using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [ApiKey]
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

                byte[] bytes = Encoding.UTF8.GetBytes(user.password);

                insertUser.branch_id = user.branch_id;
                insertUser.document = user.document; 
                insertUser.role_id = user.role_id;
                insertUser.functional_number = user.functional_number;
                insertUser.full_name = user.full_name;
                insertUser.mail = user.mail;
                insertUser.phone_number = user.phone_number;
                insertUser.password = Convert.ToBase64String(bytes);
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

                var user = _context.User
                    .AsNoTracking()
                    .Where(u => u.branch_id == iBranchId)
                    .Join(_context.Role, u => u.role_id, r => r.id, (u, r) => new {
                        u.id,
                        u.branch_id,
                        u.role_id,
                        u.full_name,
                        u.mail,
                        u.created_at,
                        role_name = r.role_name
                    })
                    .OrderBy(u => u.created_at)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToList();

                return Ok(user);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Users: " + ex.Message);
            }
        }
    }
}