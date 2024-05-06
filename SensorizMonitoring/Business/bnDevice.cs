using Database;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SensorizMonitoring.Models;
using System.Data;
using System.Diagnostics;

namespace SensorizMonitoring.Business
{
    public class bnDevice
    {
        private readonly IConfiguration _configuration;

        public bnDevice(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        DataTable dt = new DataTable();
        DataBase db = new DataBase();
        Globals utl = new Globals();

        //public bool InsertDevice(DeviceModel mv)
        //{
        //    try
        //    {
        //        string sql = string.Empty;

        //        sql += "INSERT INTO device " + Environment.NewLine;
        //        sql += "( " + Environment.NewLine;
        //        sql += "device_code, company_id, description, " + Environment.NewLine;
        //        sql += "device_reference_id, enabled, created_at " + Environment.NewLine;
        //        sql += ") VALUES ( " + Environment.NewLine;
        //        sql += "?device_code, ?company_id, ?description, " + Environment.NewLine;
        //        sql += "?device_reference_id, 1, ?created_at " + Environment.NewLine;
        //        sql += "); " + Environment.NewLine;

        //        sql += "SELECT 100 AS RETORNO, LAST_INSERT_ID() AS LASTID;";


        //        dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
        //            new MySqlParameter("?device_code", mv.device_code),
        //            new MySqlParameter("?company_id", mv.company_id),
        //            new MySqlParameter("?description", mv.description),
        //            new MySqlParameter("?device_reference_id", mv.modelID)
        //            );

        //        if (db.TrataRetorno(dt))
        //        {
        //            utl.EscreverArquivo("Inserted. " + dt.Rows[0]["LASTID"]);
        //            return true;
        //        }
        //        else
        //        {
        //            utl.EscreverArquivo("Cannot Insert Device. Probably SQL Syntax Error.");
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        utl.EscreverArquivo(ex.Message.ToString());
        //        return false;
        //    }
        //}

        //public bool UpdateDevice(DeviceModel mv, int id)
        //{
        //    try
        //    {
        //        string sql = string.Empty;

        //        sql += "UPDATE device " + Environment.NewLine;
        //        sql += " SET " + Environment.NewLine;
        //        sql += " device_code = ?device_code " + Environment.NewLine;
        //        sql += ",company_id = ?company_id " + Environment.NewLine;
        //        sql += ",description = ?description " + Environment.NewLine;
        //        sql += ",device_reference_id = ?device_reference_id " + Environment.NewLine;
        //        sql += "WHERE id = ?id " + Environment.NewLine;
        //        sql += "SELECT 100 AS RETORNO;";


        //        dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
        //            new MySqlParameter("?device_code", mv.device_code),
        //            new MySqlParameter("?company_id", mv.company_id),
        //            new MySqlParameter("?description", mv.description),
        //            new MySqlParameter("?device_reference_id", mv.modelID),
        //            new MySqlParameter("?id", id)
        //            );

        //        if (db.TrataRetorno(dt))
        //        {
        //            utl.EscreverArquivo("Updated. " + id + " | " + mv.device_code);
        //            return true;
        //        }
        //        else
        //        {
        //            utl.EscreverArquivo("Cannot Update Device. Probably SQL Syntax Error.");
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        utl.EscreverArquivo(ex.Message.ToString());
        //        return false;
        //    }
        //}

        //public bool DisableEnableDevice(int id, int flag)
        //{
        //    try
        //    {
        //        string sql = string.Empty;

        //        sql += "UPDATE device " + Environment.NewLine;
        //        sql += " SET " + Environment.NewLine;
        //        sql += " enabled = ?enabled " + Environment.NewLine;
        //        sql += "WHERE id = ?id " + Environment.NewLine;
        //        sql += "SELECT 100 AS RETORNO;";


        //        dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
        //            new MySqlParameter("?enabled", flag),
        //            new MySqlParameter("?id", id)
        //            );

        //        if (db.TrataRetorno(dt))
        //        {
        //            if (flag == 1)
        //            {
        //                utl.EscreverArquivo("Enabled Device. " + id);
        //            }
        //            else
        //            {
        //                utl.EscreverArquivo("Disabled Device. " + id);
        //            }

        //            return true;
        //        }
        //        else
        //        {
        //            utl.EscreverArquivo("Cannot Enable/Disable Device. Probably SQL Syntax Error.");
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        utl.EscreverArquivo(ex.Message.ToString());
        //        return false;
        //    }
        //}

        //public bool DeleteDevice(int id)
        //{
        //    try
        //    {
        //        string sql = string.Empty;

        //        sql += "DELETE device " + Environment.NewLine;
        //        sql += "WHERE id = ?id " + Environment.NewLine;
        //        sql += "SELECT 100 AS RETORNO;";


        //        dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
        //            new MySqlParameter("?id", id)
        //            );

        //        if (db.TrataRetorno(dt))
        //        {
        //            utl.EscreverArquivo("Deleted. " + id);
        //            return true;
        //        }
        //        else
        //        {
        //            utl.EscreverArquivo("Cannot Delete Device. Probably SQL Syntax Error.");
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        utl.EscreverArquivo(ex.Message.ToString());
        //        return false;
        //    }
        //}

        //public dynamic GetDeviceByCompanyId(int limit, int page, int companyId)
        //{
        //    try
        //    {
        //        string sql = string.Empty;

        //        sql += "SELECT * FROM device " + Environment.NewLine;
        //        sql += "WHERE company_id = ?company_id " + Environment.NewLine;
        //        sql += "ORDER BY name " + Environment.NewLine;
        //        sql += "LIMIT ?limit offset ?offset " + Environment.NewLine;

        //        dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
        //            new MySqlParameter("?limit", limit),
        //            new MySqlParameter("?offset", utl.OffSet(limit, page)),
        //            new MySqlParameter("?company_id", companyId)
        //            );

        //        return db.DTToJson(dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        utl.EscreverArquivo(ex.Message.ToString());
        //        return null;
        //    }
        //}

        //public dynamic GetDeviceById(int DeviceId)
        //{
        //    try
        //    {
        //        string sql = string.Empty;

        //        sql += "SELECT * FROM device " + Environment.NewLine;
        //        sql += "WHERE id = ?id " + Environment.NewLine;

        //        dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
        //            new MySqlParameter("?id", DeviceId)
        //            );

        //        return db.DTToJson(dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        utl.EscreverArquivo(ex.Message.ToString());
        //        return null;
        //    }
        //}

        //public bool DeviceExists(string DeviceCode)
        //{
        //    try
        //    {
        //        string sql = string.Empty;

        //        sql += "SELECT COUNT(*) as QTD FROM Device " + Environment.NewLine;
        //        sql += "WHERE device_code = ?device_code " + Environment.NewLine;

        //        dt = db.SelectAccessDB(sql, _configuration.GetConnectionString("DefaultConnection"),
        //            new MySqlParameter("?device_code", DeviceCode)
        //            );

        //        return db.TrataExists(dt);
        //    }
        //    catch (Exception ex)
        //    {
        //        utl.EscreverArquivo(ex.Message.ToString());
        //        return false;
        //    }
        //}

        public class Include
        {
            public bool pairings { get; set; }
        }
        public class DeviceInformationRequest
        {
            public string device { get; set; }
            public Include include { get; set; } 
        }

        public async Task<dynamic> GetDeviceInformationByDeviceId(string sDeviceId)
        {
            try
            {
                string sql = string.Empty;

                //var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "Data", "family_model.json");
                //var conteudoJson = File.ReadAllText(caminhoArquivo);

                var baseUrl = _configuration["Settings:LocoBaseUrlRPC"];
                var token = _configuration["Settings:LocoToken"];
                var endpoint = "getDevice";

                Include inc = new Include();
                inc.pairings = false;

                DeviceInformationRequest dir = new DeviceInformationRequest();
                dir.device = sDeviceId;
                dir.include = inc;

                var apiClient = new SensorizMonitoring.Utils.ApiClient(baseUrl, token);
                var apiData = await apiClient.PostApiDataAsync(endpoint, dir);

                Console.WriteLine(apiData);

                if (!string.IsNullOrEmpty(apiData))
                {
                    return apiData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                utl.EscreverArquivo(ex.Message.ToString());
                return null;
            }
        }
    }
}
