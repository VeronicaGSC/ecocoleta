using EcoColeta.Api.Models;
using EcoColeta.Api.Services;

namespace EcoColeta.Tests;

public class AlertaServiceTests
{
    [Fact]
    public void FiltrarPontosEmAlerta_RetornaApenasPontosNoLimite_OrdenadosDoMaisCheio()
    {
        var pontos = new List<PontoColeta>
        {
            new() { Codigo = "A", NivelAtualPercent = 90 },
            new() { Codigo = "B", NivelAtualPercent = 40 },
            new() { Codigo = "C", NivelAtualPercent = 97 },
            new() { Codigo = "D", NivelAtualPercent = 99, Status = StatusPonto.Manutencao }
        };

        var alertas = AlertaService.FiltrarPontosEmAlerta(pontos);

        Assert.Equal(new[] { "C", "A" }, alertas.Select(p => p.Codigo));
    }

    [Fact]
    public void GerarNotificacaoAlerta_PreencheTipoEContextoDoPonto()
    {
        var ponto = new PontoColeta { Codigo = "PC-001", Nome = "Ecoponto Praça Central", NivelAtualPercent = 92, LimiteAlertaPercent = 85 };

        var n = AlertaService.GerarNotificacaoAlerta(ponto, "operacao@ecocoleta.app");

        Assert.Equal(TipoNotificacao.AlertaLimite, n.Tipo);
        Assert.Equal("operacao@ecocoleta.app", n.UsuarioEmail);
        Assert.False(n.Lida);
        Assert.Equal("PC-001", n.Contexto?.PontoCodigo);
        Assert.Equal(92, n.Contexto?.NivelPercent);
        Assert.Contains("92%", n.Mensagem);
    }
}
