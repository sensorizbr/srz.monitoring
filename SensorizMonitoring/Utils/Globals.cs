using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace SensorizMonitoring
{
    public class Globals
    {
        private const string PastaArquivos = "LOGS";

        public void EscreverArquivo(string line)
        {
            // Verifica se a pasta de arquivos existe, senão cria
            string pastaArquivos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PastaArquivos);
            if (!Directory.Exists(pastaArquivos))
            {
                Directory.CreateDirectory(pastaArquivos);
            }

            // Gera o nome do arquivo com base na data atual
            string nomeArquivo = $"arquivo_{DateTime.Now:yyyyMMdd}.txt";
            string caminhoArquivo = Path.Combine(pastaArquivos, nomeArquivo);

            string conteudo = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - " + line + "\n";
            // Escreve o conteúdo no arquivo
            File.AppendAllText(caminhoArquivo, conteudo);

            // Remove arquivos com mais de 5 dias
            RemoverArquivosAntigos(pastaArquivos, TimeSpan.FromDays(5));
        }
        private void RemoverArquivosAntigos(string pastaArquivos, TimeSpan retencao)
        {
            DateTime dataLimite = DateTime.Now - retencao;

            // Percorre os arquivos na pasta e remove os mais antigos
            foreach (string arquivo in Directory.GetFiles(pastaArquivos))
            {
                DateTime dataCriacao = File.GetCreationTime(arquivo);
                if (dataCriacao < dataLimite)
                {
                    File.Delete(arquivo);
                }
            }
        }
        public bool IsValidEmail(string email)
        {
            // Padrão de expressão regular para verificar a estrutura do email
            string pattern = @"^[\w\.-]+@([\w-]+\.)+[\w-]{2,4}$";

            // Verifica se o email corresponde ao padrão
            return Regex.IsMatch(email, pattern);
        }

        public int OffSet(int limit, int page)
        {
            // Padrão de expressão regular para verificar a estrutura do email
            return (page - 1) * limit;
        }
        public double ToDouble(string value)
        {

            value = value.Replace(".", ",");
            CultureInfo culture = new CultureInfo("pt-BR");
            double result = Double.Parse(value, culture);
            return result;
        }

        public string TrataTamper(int iValue)
        {
            if (iValue == 1)
            {
                return "Aberto";
            }
            else
            {
                return "Fechado";
            }
        }

        public string TrataBool(bool bValue)
        {
            if (bValue)
            {
                return "Sim";
            }
            else
            {
                return "Não";
            }
        }

        public bool IntToBool(int iValue)
        {
            if (iValue == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int BoolToInt(bool bValue)
        {
            if (bValue)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public string DotChange(double dValue)
        {
            return dValue.ToString().Replace(",", ".");
        }

        public DateTime ToBRDateTime(long valueTime)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(valueTime);
            TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime brazilDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, brazilTimeZone);

            return brazilDateTime;
        }

        public DateTime ToBRDateTimeDT(DateTime dt)
        {
            TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime brazilDateTime = TimeZoneInfo.ConvertTime(dt, brazilTimeZone);

            return brazilDateTime;
        }

        public double FormatValuePrecision(double vlr)
        {
            string formattedLatitude = vlr.ToString("F6"); // F6 significa 6 casas decimais
            return double.Parse(formattedLatitude);
        }

        public double CalculateDistance(double lat1, double long1, double lat2, double long2)
        {
            const double R = 6371; // Raio da Terra em quilômetros
            double dLat = ToRadians(lat2 - lat1);
            double dLong = ToRadians(long2 - long1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Sin(dLong / 2) * Math.Sin(dLong / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c * 1000; // Convertendo quilômetros para metros

            return distance;
        }

        private double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public string TrataHtmlRegex(string html)
        {
            string cleanHtml = HttpUtility.HtmlDecode(html);
            cleanHtml = System.Text.RegularExpressions.Regex.Replace(cleanHtml, @"[^\u0020-\u007E]", m =>
            {
                int charCode = (int)m.Groups[0].Value[0];
                return "&#" + charCode + ";";
            });

            return cleanHtml; // return the cleaned HTML
        }

        public string RemoveSpecialCharacters(string input)
        {
            return Regex.Replace(input, @"[^\w\s]|,|\.", "");
        }
    }

}