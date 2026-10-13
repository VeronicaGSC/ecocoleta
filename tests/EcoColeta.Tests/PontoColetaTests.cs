using EcoColeta.Api.Models;

namespace EcoColeta.Tests;

public class PontoColetaTests
{
    private static PontoColeta Ponto(int nivel, int limite = 85, string status = StatusPonto.Ativo) =>
        new() { Codigo = "PC-T", Nome = "Teste", CapacidadeLitros = 1000, NivelAtualPercent = nivel, LimiteAlertaPercent = limite, Status = status };

    [Theory]
    [InlineData(85)]
    [InlineData(92)]
    [InlineData(100)]
    public void PrecisaDeColeta_QuandoNivelAtingeOuSuperaLimite_RetornaTrue(int nivel)
    {
        Assert.True(Ponto(nivel).PrecisaDeColeta());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(84)]
    public void PrecisaDeColeta_QuandoNivelAbaixoDoLimite_RetornaFalse(int nivel)
    {
        Assert.False(Ponto(nivel).PrecisaDeColeta());
    }

    [Fact]
    public void PrecisaDeColeta_QuandoColetaJaAgendada_RetornaFalse()
    {
        Assert.False(Ponto(97, status: StatusPonto.ColetaAgendada).PrecisaDeColeta());
    }
}
