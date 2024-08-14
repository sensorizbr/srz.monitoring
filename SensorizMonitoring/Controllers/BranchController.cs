using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class BranchController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public BranchController(IConfiguration configuration, AppDbContext context, ILogger logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Insere a Companhia
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertBranch([FromBody] BranchModel branch)
        {
            try
            {
                var insertBranch = new Branch();

                insertBranch.company_id = branch.company_id;
                insertBranch.full_name = branch.name;
                insertBranch.document = branch.document;
                insertBranch.head_mail = branch.head_mail;
                insertBranch.head_phonenumber = branch.head_phonenumber;
                insertBranch.enabled = 1;
                insertBranch.created_at = DateTime.Now;

                _context.Add(insertBranch);
                _context.SaveChanges();
                return Ok(insertBranch);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Atualiza a Companhia
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] BranchModel branch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingBranch = _context.Branch.Find(id);

                existingBranch.company_id = branch.company_id;
                existingBranch.full_name = branch.name;
                existingBranch.document = branch.document;
                existingBranch.head_mail = branch.head_mail;
                existingBranch.head_phonenumber = branch.head_phonenumber;

                if (existingBranch == null)
                {
                    return NotFound("Branch not found.");
                }

                _context.Update(existingBranch);
                _context.SaveChanges();
                return Ok(existingBranch);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error updating branch: " + ex.Message);
            }
        }

        /// <summary>
        /// Remove Companhia
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var delCompany = _context.Company.Find(id);
                _context.Remove(delCompany);
                _context.SaveChanges();
                return Ok("Registro removido!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Ativa/Desativa Companhia. Basta passar o id e a flag, sendo 1 para ativar e 0 para desativar.
        /// </summary>
        [HttpPut("{id}/{flag}")]
        public async Task<IActionResult> EnableDisableBranch(int id,int flag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingBranch = await _context.Branch.FindAsync(id);

                existingBranch.enabled = flag;

                if (existingBranch == null)
                {
                    return NotFound("Branch not found.");
                }

                _context.Update(existingBranch);
                _context.SaveChanges();
                return Ok(existingBranch);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error updating branch: " + ex.Message);
            }
        }


        /// <summary>
        /// Lista todas as companhias de forma paginada
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBranches(int limit, int page)
        {
            try
            {
                var existingBranch = await _context.Branch.AsNoTracking().ToListAsync<Branch>();

                // Lógica para manipular a solicitação POST
                var branches = existingBranch
                   .OrderBy(c => c.full_name) // or any other column you want to sort by
                   .Skip((page - 1) * limit)
                   .Take(limit);

                return Ok(branches);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Branch: " + ex.Message);
            }
        }

        /// <summary>
        /// Lista a Companhia por ID
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBranchById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingBranch = await _context.Branch.FindAsync(id);

                return Ok(existingBranch);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Branch: " + ex.Message);
            }
        }
    }
}