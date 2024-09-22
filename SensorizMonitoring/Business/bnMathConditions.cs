namespace SensorizMonitoring.Business
{
    public class bnMathConditions
    {
        Globals gb = new Globals();
        public bool CheckMathConditionDouble(int comparationID, double minValue, double maxValue, double? monitoringValue, bool isRecovery)
        {
            switch (comparationID)
            {
                case 1: // Equal to
                    return isRecovery ? minValue != monitoringValue && maxValue != monitoringValue : minValue == monitoringValue && maxValue == monitoringValue;
                case 2: // Not equal to
                    return isRecovery ? minValue == monitoringValue || maxValue == monitoringValue : minValue != monitoringValue && maxValue != monitoringValue;
                case 3: // Greater than
                    return isRecovery ? monitoringValue <= maxValue : monitoringValue > maxValue;
                case 4: // Less than
                    return isRecovery ? monitoringValue >= minValue : monitoringValue < minValue;
                case 5: // Between
                    return isRecovery ? monitoringValue < minValue || monitoringValue > maxValue : monitoringValue >= minValue && monitoringValue <= maxValue;
                case 6: // Outside
                    return isRecovery ? monitoringValue >= minValue && monitoringValue <= maxValue : monitoringValue < minValue || monitoringValue > maxValue;
                default:
                    return false;
            }
        }

        public bool CheckMathComparationBool(int comparationID, bool bool_value, bool monitoringValue)
        {
            switch (comparationID)
            {
                case 1: // Equal to
                    return bool_value == monitoringValue;
                case 2: // Not equal to
                    return bool_value != monitoringValue;
                case 3: // Greater than
                    return false;
                case 4: // Less than
                    return false;
                case 5: // Between
                    return false;
                case 6: // Outside
                    return false;
                default:
                    return false;
            }
        }

        public bool CheckMathComparation_Directions_GetInGetOut(int comparationID, double latDevice, double longDevice, double latSettings, double longSettings, int? radius, ref string evento, bool isRecovery)
        {
            double latDevicePrecision = gb.FormatValuePrecision(latDevice);
            double longDevicePrecision = gb.FormatValuePrecision(longDevice);
            double latSettingsPrecision = gb.FormatValuePrecision(latSettings);
            double longSettingsPrecision = gb.FormatValuePrecision(longSettings);
            double radiusPrecision = gb.FormatValuePrecision(radius ?? 0);

            double distance = gb.CalculateDistance(latDevicePrecision, longDevicePrecision, latSettingsPrecision, longSettingsPrecision);

            switch (comparationID)
            {
                case 1: // Get In To
                    evento = isRecovery ? "SAIDA" : "ENTRADA";
                    return isRecovery ? distance >= radius : distance <= radius;
                case 2: // Out To
                    evento = isRecovery ? "ENTRADA" : "SAIDA";
                    return isRecovery ? distance < radius : distance > radius;
                default:
                    return false;
            }
        }

        public bool CheckMathComparation_Router(int comparationID, double latDevice, double longDevice, double latSettings, double longSettings)
        {
            switch (comparationID)
            {
                case 1: // Get In To
                    return latDevice == latSettings && longDevice == longSettings;
                case 2: // Out To
                    return latDevice != latSettings && longDevice != longSettings;
                default:
                    return false;
            }
        }
    }
}
