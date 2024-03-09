using MySql.Data.MySqlClient;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Database
{
    public class DataBase
    {
        private MySqlConnection sql_consql;
        private MySqlCommand sql_cmdsql;

        public DataTable SelectAccessDB(string sql, string sc, params MySqlParameter[] parameters)
        {
            try
            {
                MySqlDataAdapter Adp;
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                string stringConnection = sc;

                sql_consql = new MySqlConnection(stringConnection);//Conecta a Base passando a string de Conexão

                sql_cmdsql = sql_consql.CreateCommand();

                Adp = new MySqlDataAdapter(sql, sql_consql);

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        Adp.SelectCommand.Parameters.Add(param);
                    }
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
                sql_consql.Close();
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

        public dynamic DTToJson(DataTable dataTable)
        {
            List<ExpandoObject> listaObjetos = new List<ExpandoObject>();

            foreach (DataRow row in dataTable.Rows)
            {
                dynamic objetoDinamico = new ExpandoObject();
                var objetoDinamicoDict = (IDictionary<string, object>)objetoDinamico;

                foreach (DataColumn coluna in dataTable.Columns)
                {
                    objetoDinamicoDict[coluna.ColumnName] = row[coluna];
                }

                listaObjetos.Add(objetoDinamico);
            }

            return listaObjetos;
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