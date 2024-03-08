using MySql.Data.MySqlClient;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Database
{
    public class DataBase
    {
        private MySqlConnection sql_consqlserver;
        private MySqlCommand sql_cmdsqlserver;

        public DataTable SelectAccessDBSqlServer(string sql, string sc, params MySqlParameter[] parameters)
        {
            try
            {
                MySqlDataAdapter Adp;
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                string stringConnection = sc;

                sql_consqlserver = new MySqlConnection(stringConnection);//Conecta a Base passando a string de Conexão

                sql_cmdsqlserver = sql_consqlserver.CreateCommand();

                Adp = new MySqlDataAdapter(sql, sql_consqlserver);

                foreach (var param in parameters)
                {
                    Adp.SelectCommand.Parameters.Add(param);
                }

                Adp.Fill(ds);
                dt = ds.Tables[0]; //Joga resultado em um DataTable para retorno
                return dt;
            }
            catch (Exception ex)
            {
                //lg.GeraLog(ex.Message.ToString());
                return null;
            }
            finally
            {
                //lg.GeraLog("CONEXÃO FECHADA", 5);
                sql_consqlserver.Close();
            }
        }
        public string DataTableToJSON(DataTable table)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();

                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //serializer.MaxJsonLength = Int32.MaxValue;
            return serializer.Serialize(list);
        }

        public bool TrataRetorno(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                if (int.Parse(dt.Rows[0]["RETORNO"].ToString()) == 100)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool TrataExists(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                if (int.Parse(dt.Rows[0]["QTD"].ToString()) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}