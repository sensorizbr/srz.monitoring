namespace SensorizMonitoring.Business
{
    public class bnSensorRoute
    {
        public bool isOnRoute(double latOrigin, double lngOrigin, double latDestination, double lngDestination, double latCurrent, double lngCurrent, double toleranceRadius)
        {
            // Calcula a distância entre a coordenada corrente e a linha que conecta o ponto de origem e o ponto de destino
            double distanceToLine = CalculateDistanceToLine(latOrigin, lngOrigin, latDestination, lngDestination, latCurrent, lngCurrent);

            // Verifica se a distância está dentro do raio de tolerância
            if (distanceToLine <= toleranceRadius)
            {
                Console.WriteLine("O ponto está na rota.");
                return true;
            }
            else
            {
                Console.WriteLine("O ponto está fora da rota.");
                return false;
            }
        }

        static double CalculateDistanceToLine(double lat1, double lng1, double lat2, double lng2, double latCurrent, double lngCurrent)
        {
            // Calcula a distância entre os dois pontos da linha
            double lineDistance = CalculaDistanciaHaversine(lat1, lng1, lat2, lng2);

            // Calcula a distância entre o ponto corrente e o ponto de origem
            double distanceToOrigin = CalculaDistanciaHaversine(latCurrent, lngCurrent, lat1, lng1);

            // Calcula a distância entre o ponto corrente e o ponto de destino
            double distanceToDestination = CalculaDistanciaHaversine(latCurrent, lngCurrent, lat2, lng2);

            // Calcula a distância perpendicular entre o ponto corrente e a linha
            double distanceToLine = Math.Sqrt(Math.Pow(distanceToOrigin, 2) + Math.Pow(distanceToDestination, 2) - Math.Pow(lineDistance, 2));

            return distanceToLine;
        }


        static double CalculaDistanciaHaversine(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double R = 6371; // Raio da Terra em quilômetros
            double dLat = ToRadians(latitude2 - latitude1);
            double dLon = ToRadians(longitude2 - longitude1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(latitude1)) * Math.Cos(ToRadians(latitude2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c;
        }

        static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
