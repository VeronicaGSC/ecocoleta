using EcoColeta.Api.Models;

namespace EcoColeta.Api.Services;

/// <summary>
/// Validações de entrada de cada collection. Retorna a lista de erros (vazia = válido).
/// </summary>
public static class Validador
{
    public static List<string> ValidarPontoColeta(PontoColeta p)
    {
        var erros = new List<string>();
        if (string.IsNullOrWhiteSpace(p.Codigo)) erros.Add("codigo é obrigatório.");
        if (string.IsNullOrWhiteSpace(p.Nome)) erros.Add("nome é obrigatório.");
        if (p.CapacidadeLitros <= 0) erros.Add("capacidadeLitros deve ser maior que zero.");
        if (p.NivelAtualPercent is < 0 or > 100) erros.Add("nivelAtualPercent deve estar entre 0 e 100.");
        if (p.LimiteAlertaPercent is < 1 or > 100) erros.Add("limiteAlertaPercent deve estar entre 1 e 100.");
        if (!StatusPonto.Todos.Contains(p.Status)) erros.Add($"status inválido. Use: {string.Join(", ", StatusPonto.Todos)}.");
        return erros;
    }

    public static List<string> ValidarColeta(Coleta c)
    {
        var erros = new List<string>();
        if (string.IsNullOrWhiteSpace(c.Codigo)) erros.Add("codigo é obrigatório.");
        if (string.IsNullOrWhiteSpace(c.PontoColetaCodigo)) erros.Add("pontoColetaCodigo é obrigatório.");
        if (c.Materiais.Count == 0) erros.Add("a coleta deve ter pelo menos um material.");
        if (c.Materiais.Any(m => m.PesoKg <= 0)) erros.Add("pesoKg de cada material deve ser maior que zero.");
        if (c.Materiais.Any(m => string.IsNullOrWhiteSpace(m.Destino))) erros.Add("destino de cada material é obrigatório (rastreabilidade).");
        return erros;
    }

    public static List<string> ValidarUsuario(Usuario u)
    {
        var erros = new List<string>();
        if (string.IsNullOrWhiteSpace(u.Nome)) erros.Add("nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(u.Email) || !u.Email.Contains('@')) erros.Add("email inválido.");
        if (u.PontosRecompensa < 0) erros.Add("pontosRecompensa não pode ser negativo.");

        switch (u.Tipo)
        {
            case TipoUsuario.Individual when string.IsNullOrWhiteSpace(u.Cpf):
                erros.Add("usuário individual deve informar cpf.");
                break;
            case TipoUsuario.Empresa when string.IsNullOrWhiteSpace(u.Cnpj):
                erros.Add("usuário empresa deve informar cnpj.");
                break;
            case TipoUsuario.Individual:
            case TipoUsuario.Empresa:
                break;
            default:
                erros.Add("tipo deve ser 'individual' ou 'empresa'.");
                break;
        }
        return erros;
    }

    public static List<string> ValidarNotificacao(Notificacao n)
    {
        var erros = new List<string>();
        if (string.IsNullOrWhiteSpace(n.UsuarioEmail) || !n.UsuarioEmail.Contains('@')) erros.Add("usuarioEmail inválido.");
        if (string.IsNullOrWhiteSpace(n.Titulo)) erros.Add("titulo é obrigatório.");
        if (!TipoNotificacao.Todos.Contains(n.Tipo)) erros.Add($"tipo inválido. Use: {string.Join(", ", TipoNotificacao.Todos)}.");
        return erros;
    }

    public static List<string> ValidarMaterial(MaterialReciclavel m)
    {
        var erros = new List<string>();
        if (string.IsNullOrWhiteSpace(m.Nome)) erros.Add("nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(m.Categoria)) erros.Add("categoria é obrigatória.");
        if (string.IsNullOrWhiteSpace(m.InstrucoesDescarte)) erros.Add("instrucoesDescarte é obrigatória.");
        if (m.ValorKgEstimado < 0) erros.Add("valorKgEstimado não pode ser negativo.");
        return erros;
    }
}
