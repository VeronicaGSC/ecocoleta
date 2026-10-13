using EcoColeta.Api.Services;

namespace EcoColeta.Tests;

public class GamificacaoServiceTests
{
    [Theory]
    [InlineData(0, "Reciclador Iniciante")]
    [InlineData(99, "Reciclador Iniciante")]
    [InlineData(100, "Reciclador Prata")]
    [InlineData(320, "Reciclador Ouro")]
    [InlineData(1580, "Guardião Verde")]
    public void CalcularNivel_RetornaNivelPorFaixaDePontos(int pontos, string nivelEsperado)
    {
        Assert.Equal(nivelEsperado, GamificacaoService.CalcularNivel(pontos));
    }
}
