using Database;
using MySql.Data.MySqlClient;
using SensorizMonitoring.Data.Context;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using System.Data;
using System.Diagnostics;

namespace SensorizMonitoring.Business
{
    public class bnCompany
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public bnCompany(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        DataTable dt = new DataTable();
        DataBase db = new DataBase();
        Globals utl = new Globals();

        public bool InsertCompany(CompanyModel mv)
        {
            try
            {
                string sql = string.Empty;

                //if (CompanyExists(mv.document))
                //{
                //    throw new Exception("Company already exists! " + mv.document + " - " + mv.name);
                //}

                sql += "INSERT INTO company " + Environment.NewLine;
                sql += "( " + Environment.NewLine;
                sql += "name, document, head_mail, " + Environment.NewLine;
                sql += "head_phonenumber, enabled, created_at " + Environment.NewLine;
                sql += ") VALUES ( " + Environment.NewLine;
                sql += "?name, ?document, ?head_mail, " + Environment.NewLine;
                sql += "?head_phonenumber, 1, NOW() " + Environment.NewLine;
                sql += "); " + Environment.NewLine;

                sql += "SELECT 100 AS RETORNO, LAST_INSERT_ID() AS LASTID;";


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?name", mv.name),
                    new MySqlParameter("?document", mv.document),
                    new MySqlParameter("?head_mail", mv.head_mail),
                    new MySqlParameter("?head_phonenumber", mv.head_phonenumber)
                    );

                if (db.TrataRetorno(dt))
                {
                    utl.EscreverArquivo("Inserted. " + dt.Rows[0]["LASTID"]);
                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Insert Company. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public bool UpdateCompany(CompanyModel mv, int id)
        {
            try
            {
                string sql = string.Empty;

                sql += "UPDATE company " + Environment.NewLine;
                sql += " SET " + Environment.NewLine;
                sql += " name = ?name " + Environment.NewLine;
                sql += ",document = ?document " + Environment.NewLine;
                sql += ",head_mail = ?head_mail " + Environment.NewLine;
                sql += ",head_phonenumber = ?head_phonenumber " + Environment.NewLine;
                sql += "WHERE id = ?id " + Environment.NewLine;
                sql += "SELECT 100 AS RETORNO;";


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?name", mv.name),
                    new MySqlParameter("?document", mv.document),
                    new MySqlParameter("?head_mail", mv.head_mail),
                    new MySqlParameter("?head_phonenumber", mv.head_phonenumber),
                    new MySqlParameter("?id", id)
                    );

                if (db.TrataRetorno(dt))
                {
                    utl.EscreverArquivo("Updated. " + id + " | " + mv.document);
                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Update Company. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public bool DisableEnableCompany(int id, int flag)
        {
            try
            {
                string sql = string.Empty;

                sql += "UPDATE company " + Environment.NewLine;
                sql += " SET " + Environment.NewLine;
                sql += " enabled = ?enabled " + Environment.NewLine;
                sql += "WHERE id = ?id " + Environment.NewLine;
                sql += "SELECT 100 AS RETORNO;";


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?enabled", flag),
                    new MySqlParameter("?id", id)
                    );

                if (db.TrataRetorno(dt))
                {
                    if (flag == 1)
                    {
                        utl.EscreverArquivo("Enabled Comapny. " + id);
                    }
                    else
                    {
                        utl.EscreverArquivo("Disabled Comapny. " + id);
                    }

                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Enable/Disable Company. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public bool DeleteCompany(int id)
        {
            try
            {
                string sql = string.Empty;

                sql += "DELETE company " + Environment.NewLine;
                sql += "WHERE id = ?id " + Environment.NewLine;
                sql += "SELECT 100 AS RETORNO;";


                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?id", id)
                    );

                if (db.TrataRetorno(dt))
                {
                    utl.EscreverArquivo("Deleted. " + id);
                    return true;
                }
                else
                {
                    utl.EscreverArquivo("Cannot Delete Company. Probably SQL Syntax Error.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }

        public dynamic GetCompanies(int limit, int page)
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT * FROM company " + Environment.NewLine;
                sql += "ORDER BY name " + Environment.NewLine;
                sql += "LIMIT ?limit offset ?offset " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?limit", limit),
                    new MySqlParameter("?offset", utl.OffSet(limit, page))
                    );

                return db.DTToJson(dt);
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return null;
            }
        }

        public dynamic GetCompanyById(int CompanyId)
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT * FROM company " + Environment.NewLine;
                sql += "WHERE id = ?id " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?id", CompanyId)
                    );

                return db.DTToJson(dt);
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return null;
            }
        }

        public dynamic GetCompanyByDocument(string document)
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT * FROM company " + Environment.NewLine;
                sql += "ORDER BY name " + Environment.NewLine;
                sql += "WHERE document = ?document " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?document", document)
                    );

                return db.DTToJson(dt);
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return null;
            }
        }

        public bool CompanyExists(string document)
        {
            try
            {
                string sql = string.Empty;

                sql += "SELECT COUNT(*) as QTD FROM company " + Environment.NewLine;
                sql += "WHERE document = ?document " + Environment.NewLine;

                dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
                    new MySqlParameter("?document", document)
                    );

                return db.TrataExists(dt);
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return false;
            }
        }
    }
}
