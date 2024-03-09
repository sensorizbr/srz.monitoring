
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SensorizMonitoring.Controllers;
using SensorizMonitoring.Models;
using Xunit;

namespace SensorizMonitoring.Test
{
    public class UnitTest1
    {
        private readonly IConfiguration _configuration;

        public UnitTest1(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ObterProdutoPorId_DeveRetornarSucesso()
        {
            // Arrange
            var json = File.ReadAllText("monitoring.json");
            var monitoringJson = JsonConvert.DeserializeObject<MonitoringModel>(json);

            // Arrange
            var controller = new MonitoringController(_configuration);

            // Act
            var resultado = controller.InsertMonitoring(monitoringJson) as ViewResult;

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("DetalhesProduto", resultado.ViewName);
        }
    }
}

