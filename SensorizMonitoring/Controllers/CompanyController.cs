using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiKey]
    [ApiController]
    public class CompanyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public CompanyController(IConfiguration configuration, AppDbContext context, ILogger<Company> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Insere a Companhia
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> InsertCompany([FromBody] CompanyModel company)
        {
            try
            {
                var insertCompany = new Company();

                insertCompany.name = company.name;
                insertCompany.document = company.document;
                insertCompany.head_mail = company.head_mail;
                insertCompany.head_phonenumber = company.head_phonenumber;
                insertCompany.enabled = 1;
                insertCompany.created_at = DateTime.Now;

                _context.Add(insertCompany);
                _context.SaveChanges();
                return Ok(insertCompany);
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
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyModel company)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingCompany = _context.Company.Find(id);

                existingCompany.name = company.name;
                existingCompany.document = company.document;
                existingCompany.head_mail = company.head_mail;
                existingCompany.head_phonenumber = company.head_phonenumber;
                //existingCompany.enabled = company.enabled;

                if (existingCompany == null)
                {
                    return NotFound("Company not found.");
                }

                _context.Update(existingCompany);
                _context.SaveChanges();
                return Ok(existingCompany);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error updating company: " + ex.Message);
            }
        }

        /// <summary>
        /// Remove Companhia
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
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
        public async Task<IActionResult> EnableDisableCompany(int id,int flag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingCompany = await _context.Company.FindAsync(id);

                existingCompany.enabled = flag;

                if (existingCompany == null)
                {
                    return NotFound("Company not found.");
                }

                _context.Update(existingCompany);
                _context.SaveChanges();
                return Ok(existingCompany);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error updating company: " + ex.Message);
            }
        }


        /// <summary>
        /// Lista todas as companhias de forma paginada
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCompanies(int limit, int page)
        {
            try
            {
                var existingCompany = await _context.Company.AsNoTracking().ToListAsync<Company>();

                // Lógica para manipular a solicitação POST
                var companies = existingCompany
                   .OrderBy(c => c.name) // or any other column you want to sort by
                   .Skip((page - 1) * limit)
                   .Take(limit);

                return Ok(companies);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Company: " + ex.Message);
            }
        }

        /// <summary>
        /// Lista a Companhia por ID
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingCompany = await _context.Company.FindAsync(id);

                return Ok(existingCompany);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Company: " + ex.Message);
            }
        }
    }
}