namespace EcoColeta.Api.Services;

/// <summary>
/// Regras de gamificação do pilar Social: o nível do usuário depende
/// dos pontos acumulados por descarte correto.
/// </summary>
public static class GamificacaoService
{
    public static string CalcularNivel(int pontos) => pontos switch
    {
        < 100 => "Reciclador Iniciante",
        < 300 => "Reciclador Prata",
        < 1000 => "Reciclador Ouro",
        _ => "Guardião Verde"
    };
}
