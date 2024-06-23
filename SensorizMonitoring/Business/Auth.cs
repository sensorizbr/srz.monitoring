using Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SensorizMonitoring.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SensorizMonitoring.Business
{
    public class Auth
    {
        private readonly IConfiguration _configuration;

        public Auth(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        DataTable dt = new DataTable();
        DataBase db = new DataBase();
        Globals utl = new Globals();

        public AuthLoginResponseModel Login(AuthLoginModel mv)
        {
            try
            {
                string sql = string.Empty;

                sql += "select usr.id as user_id, fullname, mail, phone_number, usrtp.description as role, cpn.name as company_name, 'true' as logged " + Environment.NewLine;
                sql += "from user usr " + Environment.NewLine;
                sql += "inner join user_type usrtp on usr.user_type_id = usrtp.id " + Environment.NewLine;
                sql += "inner join company cpn on usr.company_id  = cpn.id " + Environment.NewLine;
                sql += "where mail = ?mail " + Environment.NewLine;
                sql += "and password = md5(?pass) " + Environment.NewLine;
                sql += "and usr.enabled = 1 " + Environment.NewLine;


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?mail", mv.mail),
                    new MySqlParameter("?pass", mv.password)
                );

                var ret = db.DRowToJson(dt.Rows[0]);

                string json = JsonConvert.SerializeObject(ret);

                AuthLoginResponseModel alr = JsonConvert.DeserializeObject<AuthLoginResponseModel>(json);

                return alr;
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return null;
            }
        }

        public string GenerateToken(AuthLoginResponseModel auth)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", auth.user_id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
