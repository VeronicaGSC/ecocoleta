using EcoColeta.Api.Models;

namespace EcoColeta.Api.Services;

public record MaterialPeso(string Material, double PesoTotalKg);

public record RelatorioEsg(
    int TotalColetas,
    double PesoTotalKg,
    IReadOnlyList<MaterialPeso> PesoPorMaterial,
    double Co2EvitadoKg,
    double AguaEconomizadaLitros,
    int PontosCadastrados,
    int PontosEmAlerta,
    DateTime GeradoEmUtc);

/// <summary>
/// Gera indicadores ESG a partir dos dados operacionais (relatório ambiental e de governança).
/// </summary>
public static class RelatorioService
{
    public static List<MaterialPeso> CalcularPesoPorMaterial(IEnumerable<Coleta> coletas) =>
        coletas.SelectMany(c => c.Materiais)
               .GroupBy(m => m.Tipo)
               .Select(g => new MaterialPeso(g.Key, Math.Round(g.Sum(m => m.PesoKg), 2)))
               .OrderByDescending(m => m.PesoTotalKg)
               .ToList();

    /// <summary>
    /// Calcula CO2 evitado e água economizada cruzando o peso coletado de cada
    /// material com os fatores de impacto do catálogo de materiais.
    /// </summary>
    public static (double Co2EvitadoKg, double AguaEconomizadaLitros) CalcularImpacto(
        IEnumerable<MaterialPeso> pesos, IEnumerable<MaterialReciclavel> catalogo)
    {
        var fatores = catalogo
            .Where(m => m.ImpactoAmbiental is not null)
            .GroupBy(m => m.Categoria)
            .ToDictionary(g => g.Key, g => g.First().ImpactoAmbiental!);

        double co2 = 0, agua = 0;
        foreach (var p in pesos)
        {
            if (!fatores.TryGetValue(p.Material, out var fator)) continue;
            co2 += p.PesoTotalKg * fator.Co2EvitadoKgPorKg;
            agua += p.PesoTotalKg * fator.AguaEconomizadaLitros;
        }
        return (Math.Round(co2, 2), Math.Round(agua, 2));
    }

    public static RelatorioEsg GerarRelatorio(
        IReadOnlyCollection<Coleta> coletas,
        IEnumerable<MaterialReciclavel> catalogo,
        IReadOnlyCollection<PontoColeta> pontos)
    {
        var pesos = CalcularPesoPorMaterial(coletas);
        var (co2, agua) = CalcularImpacto(pesos, catalogo);

        return new RelatorioEsg(
            TotalColetas: coletas.Count,
            PesoTotalKg: Math.Round(pesos.Sum(p => p.PesoTotalKg), 2),
            PesoPorMaterial: pesos,
            Co2EvitadoKg: co2,
            AguaEconomizadaLitros: agua,
            PontosCadastrados: pontos.Count,
            PontosEmAlerta: pontos.Count(p => p.PrecisaDeColeta()),
            GeradoEmUtc: DateTime.UtcNow);
    }
}
