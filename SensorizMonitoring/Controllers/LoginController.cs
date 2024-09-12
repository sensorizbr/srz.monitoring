using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Crud;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using SensorizMonitoring.Business;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using SensorizMonitoring.Models.DeviceReference;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SensorizMonitoring.Controllers
{
    [Route("[action]")]
    [ApiKey]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public LoginController(IConfiguration configuration, AppDbContext context, ILogger<UserController> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        // Rota POST: api/Exemplo
        [HttpPost]
        public IActionResult Login([FromBody] AuthLoginModel credentials)
        {
            Globals utl = new Globals();
            utl.EscreverArquivo("Starting a Login...");

            var responseFail = new
            {
                logged = false
            };

            try
            {
                //var user = _context.User
                //    .Where(u => u.mail == credentials.mail && u.enabled == 1)
                //.FirstOrDefault();

                var user = _context.User
                    .Where(u => u.mail == credentials.mail && u.enabled == 1)
                    .Join(_context.Role, u => u.role_id, r => r.id, (u, r) => new { User = u, Role = r })
                    .Select(x => new {
                        x.User.id,
                        x.User.branch_id,
                        x.User.role_id,
                        x.User.full_name,
                        x.User.mail,
                        x.User.password,
                        role_name = x.Role.role_name
                    })
                    .FirstOrDefault();


                if (user != null)
                {
                    AuthLoginResponseModel auth = new AuthLoginResponseModel();

                    byte[] bytes = Encoding.UTF8.GetBytes(credentials.password);
                    string originalString = Convert.ToBase64String(bytes);

                    if (originalString == user.password)
                    {

                        auth.user_id = user.id;
                        auth.branch_id = user.branch_id;
                        auth.role_id = user.role_id;
                        auth.role_name = user.role_name;
                        auth.fullname = user.full_name;
                        auth.mail = user.mail;
                        //var token = GenerateToken(user, _configuration);
                        return Ok(auth);
                    }
                    else
                    {
                        return Unauthorized("Invalid username or password");
                    }
                }

                return Unauthorized("Invalid username or password");
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return password == storedHash;
            //var hasher = new PasswordHasher<object>(); // or use your user class instead of object
            //return hasher.VerifyHashedPassword(null, storedHash, password) == PasswordVerificationResult.Success;
        }

        private string GenerateToken(User user, IConfiguration configuration)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Email, user.mail),
                new Claim(ClaimTypes.Name, user.full_name)
            };

            var secretKey = configuration["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddSeconds(30));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}