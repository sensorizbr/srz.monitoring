using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SensorizMonitoring.Business;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;

namespace SensorizMonitoring.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CompanyController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public CompanyController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Insere a Companhia
        /// </summary>
        [HttpPost]
        public IActionResult InsertCompany([FromBody] CompanyModel company)
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
        public IActionResult UpdateCompany([FromRoute] int id, [FromBody] CompanyModel company)
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
        [HttpPut]
        public IActionResult DeleteCompany(int id)
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
        [HttpPut("{id}")]
        public IActionResult EnableDisableCompany([FromRoute] int id, [FromBody] int iFlag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingCompany = _context.Company.Find(id);
                
                existingCompany.enabled = iFlag;

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


        ///// <summary>
        ///// Lista todas as companhias de forma paginada
        ///// </summary>
        [HttpGet]
        public IActionResult GetCompanies(int limit, int page)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingCompany = _context.Company.ToList<Company>();

                // Lógica para manipular a solicitação POST
                var companies = existingCompany
                   .OrderBy(c => c.name) // or any other column you want to sort by
                   .Skip((page - 1) * limit)
                   .Take(limit);

                dynamic ret = companies.ToList();

                return Ok(ret);
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
        public IActionResult GetCompanyById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingCompany = _context.Company.Find(id);

                return Ok(existingCompany);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, "Error Listing Company: " + ex.Message);
            }
        }
    }
}