public enum enumSensorType
{
    [Description("Temperatura")]
    Temperatura = 1,

    [Description("Pressão Atmosférica")]
    PressaoAtmosferica = 2,

    [Description("Geolocalização (Fora do Ponto)")]
    Geolocation = 3,

    [Description("Geolocalização (Rota)")]
    Cep = 4,

    [Description("Potência Externa")]
    PotenciaExterna = 5,

    [Description("Status Bateria")]
    StatusBateria = 6,

    [Description("Voltagem Bateria")]
    VoltagemBateria = 7,

    [Description("Nível da Luz")]
    NivelLuz = 8,

    [Description("Orientation x")]
    OrientationX = 9,

    [Description("Orientation y")]
    OrientationY = 10,

    [Description("Orientation z")]
    OrientationZ = 11,

    [Description("Vibration x")]
    VibrationX = 12,

    [Description("Vibration y")]
    VibrationY = 13,

    [Description("Vibration z")]
    VibrationZ = 14,

    [Description("Status Sinal Comunicação")]
    StatusSinalCom = 15,

    [Description("Tamper")]
    Tamper = 16,

    [Description("Movimentação")]
    Movement = 17,

    [Description("Direção")]
    Direction = 18,

    [Description("Impacto")]
    Impact = 19
}