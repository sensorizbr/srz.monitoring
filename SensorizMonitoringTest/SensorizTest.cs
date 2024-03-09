using SensorizMonitoring.Controllers;
using Xunit;

namespace SensorizMonitoringTest
{
    public class SensorizTest
    {
        [Fact]
        public void InsertCompany_DeveRetornarVerdadeiro()
        {
            // Arrange
            var srv = new Senso();

            // Act
            var resultado = meuServico.MeuMetodoDeTeste();

            // Assert
            Assert.True(resultado);
        }
    }
}