using EcoColeta.Api.Data;
using EcoColeta.Api.Services;

namespace EcoColeta.Tests;

/// <summary>Garante que a carga inicial respeita a modelagem (10+ documentos e dados válidos).</summary>
public class DadosIniciaisTests
{
    [Fact]
    public void TodasAsCollections_PossuemPeloMenos10Documentos()
    {
        Assert.True(DadosIniciais.PontosColeta().Count >= 10);
        Assert.True(DadosIniciais.Coletas().Count >= 10);
        Assert.True(DadosIniciais.Usuarios().Count >= 10);
        Assert.True(DadosIniciais.Notificacoes().Count >= 10);
        Assert.True(DadosIniciais.Materiais().Count >= 10);
    }

    [Fact]
    public void PontosColeta_PossuemCodigosUnicos()
    {
        var codigos = DadosIniciais.PontosColeta().Select(p => p.Codigo).ToList();
        Assert.Equal(codigos.Count, codigos.Distinct().Count());
    }

    [Fact]
    public void Usuarios_PossuemEmailsUnicos_EDadosValidos()
    {
        var usuarios = DadosIniciais.Usuarios();
        Assert.Equal(usuarios.Count, usuarios.Select(u => u.Email).Distinct().Count());
        Assert.All(usuarios, u => Assert.Empty(Validador.ValidarUsuario(u)));
    }

    [Fact]
    public void DadosIniciais_PassamNasValidacoes()
    {
        Assert.All(DadosIniciais.PontosColeta(), p => Assert.Empty(Validador.ValidarPontoColeta(p)));
        Assert.All(DadosIniciais.Coletas(), c => Assert.Empty(Validador.ValidarColeta(c)));
        Assert.All(DadosIniciais.Notificacoes(), n => Assert.Empty(Validador.ValidarNotificacao(n)));
        Assert.All(DadosIniciais.Materiais(), m => Assert.Empty(Validador.ValidarMaterial(m)));
    }
}
