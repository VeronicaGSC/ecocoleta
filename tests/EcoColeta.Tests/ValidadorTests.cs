using EcoColeta.Api.Models;
using EcoColeta.Api.Services;

namespace EcoColeta.Tests;

public class ValidadorTests
{
    [Fact]
    public void ValidarPontoColeta_PontoValido_SemErros()
    {
        var p = new PontoColeta { Codigo = "PC-100", Nome = "Novo ponto", CapacidadeLitros = 500, NivelAtualPercent = 10 };
        Assert.Empty(Validador.ValidarPontoColeta(p));
    }

    [Fact]
    public void ValidarPontoColeta_NivelAcimaDe100_RetornaErro()
    {
        var p = new PontoColeta { Codigo = "PC-100", Nome = "Novo ponto", CapacidadeLitros = 500, NivelAtualPercent = 150 };
        Assert.Contains(Validador.ValidarPontoColeta(p), e => e.Contains("nivelAtualPercent"));
    }

    [Fact]
    public void ValidarUsuario_EmpresaSemCnpj_RetornaErro()
    {
        var u = new Usuario { Nome = "Empresa X", Email = "x@empresa.com", Tipo = TipoUsuario.Empresa };
        Assert.Contains(Validador.ValidarUsuario(u), e => e.Contains("cnpj"));
    }

    [Fact]
    public void ValidarUsuario_IndividualComCpf_SemErros()
    {
        var u = new Usuario { Nome = "Maria", Email = "maria@email.com", Tipo = TipoUsuario.Individual, Cpf = "000.000.000-00" };
        Assert.Empty(Validador.ValidarUsuario(u));
    }

    [Fact]
    public void ValidarColeta_SemMateriais_RetornaErro()
    {
        var c = new Coleta { Codigo = "CL-1", PontoColetaCodigo = "PC-001" };
        Assert.Contains(Validador.ValidarColeta(c), e => e.Contains("pelo menos um material"));
    }

    [Fact]
    public void ValidarNotificacao_TipoInvalido_RetornaErro()
    {
        var n = new Notificacao { UsuarioEmail = "a@b.com", Titulo = "Oi", Tipo = "spam" };
        Assert.Contains(Validador.ValidarNotificacao(n), e => e.Contains("tipo inválido"));
    }

    [Fact]
    public void ValidarMaterial_SemInstrucoes_RetornaErro()
    {
        var m = new MaterialReciclavel { Nome = "Papel", Categoria = "papel" };
        Assert.Contains(Validador.ValidarMaterial(m), e => e.Contains("instrucoesDescarte"));
    }
}
