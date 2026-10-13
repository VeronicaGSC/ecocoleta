using EcoColeta.Api.Models;
using EcoColeta.Api.Services;

namespace EcoColeta.Tests;

public class RelatorioServiceTests
{
    private static Coleta Coleta(params (string tipo, double peso)[] itens) => new()
    {
        Codigo = "CL-T",
        PontoColetaCodigo = "PC-T",
        Materiais = itens.Select(i => new MaterialColetado { Tipo = i.tipo, PesoKg = i.peso, Destino = "cooperativa" }).ToList()
    };

    [Fact]
    public void CalcularPesoPorMaterial_SomaPesosPorTipo_EOrdenaDecrescente()
    {
        var coletas = new[] { Coleta(("papel", 10), ("vidro", 5)), Coleta(("papel", 2.5), ("metal", 20)) };

        var resultado = RelatorioService.CalcularPesoPorMaterial(coletas);

        Assert.Equal(3, resultado.Count);
        Assert.Equal(new MaterialPeso("metal", 20), resultado[0]);
        Assert.Equal(new MaterialPeso("papel", 12.5), resultado[1]);
        Assert.Equal(new MaterialPeso("vidro", 5), resultado[2]);
    }

    [Fact]
    public void CalcularImpacto_UsaFatoresDoCatalogo_EIgnoraMateriaisSemFator()
    {
        var pesos = new[] { new MaterialPeso("metal", 10), new MaterialPeso("desconhecido", 100) };
        var catalogo = new[]
        {
            new MaterialReciclavel { Categoria = "metal", ImpactoAmbiental = new ImpactoAmbiental { Co2EvitadoKgPorKg = 9, AguaEconomizadaLitros = 30 } }
        };

        var (co2, agua) = RelatorioService.CalcularImpacto(pesos, catalogo);

        Assert.Equal(90, co2);
        Assert.Equal(300, agua);
    }

    [Fact]
    public void GerarRelatorio_SemDados_RetornaIndicadoresZerados()
    {
        var relatorio = RelatorioService.GerarRelatorio(new List<Coleta>(), new List<MaterialReciclavel>(), new List<PontoColeta>());

        Assert.Equal(0, relatorio.TotalColetas);
        Assert.Equal(0, relatorio.PesoTotalKg);
        Assert.Empty(relatorio.PesoPorMaterial);
        Assert.Equal(0, relatorio.PontosEmAlerta);
    }
}
