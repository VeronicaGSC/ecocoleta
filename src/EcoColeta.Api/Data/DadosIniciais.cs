using EcoColeta.Api.Models;

namespace EcoColeta.Api.Data;

/// <summary>
/// Dados de exemplo baseados na modelagem NoSQL da EcoColeta (5 collections, 10+ documentos cada).
/// São carregados automaticamente quando o banco está vazio.
/// </summary>
public static class DadosIniciais
{
    private static DateTime D(int ano, int mes, int dia, int hora = 0, int min = 0) =>
        new(ano, mes, dia, hora, min, 0, DateTimeKind.Utc);

    private static Endereco End(string bairro, string cidade = "São Paulo", string uf = "SP", string? logradouro = null) =>
        new() { Logradouro = logradouro, Bairro = bairro, Cidade = cidade, Uf = uf };

    private static PontoColeta P(string codigo, string nome, string tipo, Endereco end, double lon, double lat,
        int capacidade, int nivel, string[] materiais, SensorIot? sensor = null, bool? solar = null) => new()
    {
        Codigo = codigo, Nome = nome, Tipo = tipo, Endereco = end,
        Localizacao = new Localizacao { Coordinates = new[] { lon, lat } },
        CapacidadeLitros = capacidade, NivelAtualPercent = nivel, LimiteAlertaPercent = 85,
        MateriaisAceitos = materiais.ToList(), Status = StatusPonto.Ativo,
        SensorIot = sensor, EnergiaSolar = solar
    };

    private static SensorIot Sensor(string modelo, int bateria) =>
        new() { Modelo = modelo, BateriaPercent = bateria, UltimaLeitura = D(2026, 9, 8, 7, 15) };

    private static readonly string[] Seletiva = { "papel", "plastico", "vidro", "metal" };

    public static List<PontoColeta> PontosColeta() => new()
    {
        P("PC-001", "Ecoponto Praça Central", "lixeira_inteligente", End("Centro", logradouro: "Praça da Sé, s/n"), -46.6339, -23.5505, 1000, 92, Seletiva, Sensor("EcoSense-X2", 78), true),
        P("PC-002", "Ecoponto Parque Ibirapuera", "ecoponto", End("Vila Mariana"), -46.6576, -23.5874, 2000, 45, Seletiva.Append("organico").ToArray()),
        P("PC-003", "Lixeira Inteligente Estação Metrô", "lixeira_inteligente", End("Sé"), -46.6345, -23.5498, 500, 88, Seletiva, Sensor("EcoSense-X2", 64)),
        P("PC-004", "PEV Shopping Eldorado", "pev", End("Pinheiros"), -46.6965, -23.5728, 1500, 60, new[] { "eletronico", "pilha_bateria", "lampada", "papel" }),
        P("PC-005", "Ecoponto Vila Madalena", "ecoponto", End("Vila Madalena"), -46.6911, -23.5546, 2500, 97, new[] { "vidro", "oleo_cozinha", "metal" }, solar: true),
        P("PC-006", "Container Rua Augusta", "container", End("Consolação"), -46.6580, -23.5530, 3000, 30, Seletiva),
        P("PC-007", "Lixeira Inteligente Av. Paulista", "lixeira_inteligente", End("Bela Vista"), -46.6544, -23.5614, 500, 86, Seletiva, Sensor("EcoSense-X3", 91), true),
        P("PC-008", "PEV Supermercado Mooca", "pev", End("Mooca"), -46.5993, -23.5599, 1200, 55, new[] { "eletronico", "pilha_bateria", "oleo_cozinha" }),
        P("PC-009", "Ecoponto Lapa", "ecoponto", End("Lapa"), -46.7039, -23.5226, 2000, 90, Seletiva.Append("isopor").ToArray()),
        P("PC-010", "Composteira Comunitária Butantã", "composteira", End("Butantã"), -46.7225, -23.5711, 800, 40, new[] { "organico" }),
        P("PC-011", "Lixeira Inteligente Orla Rio", "lixeira_inteligente", End("Copacabana", "Rio de Janeiro", "RJ"), -43.1822, -22.9711, 500, 94, Seletiva, Sensor("EcoSense-X2", 55), true),
        P("PC-012", "Ecoponto Tatuapé", "ecoponto", End("Tatuapé"), -46.5768, -23.5403, 2200, 15, Seletiva)
    };

    private static MaterialColetado M(string tipo, double peso, string destino) => new() { Tipo = tipo, PesoKg = peso, Destino = destino };

    public static List<Coleta> Coletas() => new()
    {
        new() { Codigo = "CL-1001", PontoColetaCodigo = "PC-001", DataColeta = D(2026, 9, 1, 9), Materiais = { M("papel", 42.5, "cooperativa"), M("plastico", 30.2, "cooperativa") }, Veiculo = new() { Placa = "ECO-1A23", Tipo = "caminhao_eletrico" }, Operador = "João Ribeiro", Rota = "R-CENTRO-01", ValorReciclagemEstimado = 118.90 },
        new() { Codigo = "CL-1002", PontoColetaCodigo = "PC-002", DataColeta = D(2026, 9, 1, 14), Materiais = { M("organico", 120.0, "compostagem") }, Veiculo = new() { Placa = "ECO-2B45", Tipo = "caminhao_diesel" }, Operador = "Marta Silva", Rota = "R-SUL-02" },
        new() { Codigo = "CL-1003", PontoColetaCodigo = "PC-005", DataColeta = D(2026, 9, 2, 8, 15), Materiais = { M("vidro", 55.3, "usina_reciclagem"), M("oleo_cozinha", 18.0, "biodiesel") }, Veiculo = new() { Placa = "ECO-3C67", Tipo = "caminhao_diesel" }, Operador = "Carlos Nunes", Rota = "R-OESTE-03", Incidentes = new() { new() { Descricao = "contaminação por resíduo orgânico", Gravidade = "baixa" } } },
        new() { Codigo = "CL-1004", PontoColetaCodigo = "PC-004", DataColeta = D(2026, 9, 3, 10), Materiais = { M("papel", 25.0, "cooperativa"), M("eletronico", 22.4, "logistica_reversa") }, Veiculo = new() { Placa = "ECO-1A23", Tipo = "caminhao_eletrico" }, Operador = "João Ribeiro", Rota = "R-OESTE-01", ValorReciclagemEstimado = 154.40 },
        new() { Codigo = "CL-1005", PontoColetaCodigo = "PC-003", DataColeta = D(2026, 9, 3, 16), Materiais = { M("plastico", 18.7, "cooperativa"), M("metal", 9.5, "usina_reciclagem") }, Veiculo = new() { Placa = "ECO-4D89", Tipo = "van_eletrica" }, Operador = "Ana Paula", Rota = "R-CENTRO-02", ValorReciclagemEstimado = 88.50 },
        new() { Codigo = "CL-1006", PontoColetaCodigo = "PC-008", DataColeta = D(2026, 9, 4, 11), Materiais = { M("eletronico", 35.7, "logistica_reversa"), M("pilha_bateria", 4.2, "logistica_reversa") }, Veiculo = new() { Placa = "ECO-4D89", Tipo = "van_eletrica" }, Operador = "Ana Paula", Rota = "R-LESTE-01" },
        new() { Codigo = "CL-1007", PontoColetaCodigo = "PC-007", DataColeta = D(2026, 9, 5, 7, 30), Materiais = { M("papel", 31.4, "cooperativa"), M("vidro", 22.6, "usina_reciclagem") }, Veiculo = new() { Placa = "ECO-1A23", Tipo = "caminhao_eletrico" }, Operador = "João Ribeiro", Rota = "R-CENTRO-01" },
        new() { Codigo = "CL-1008", PontoColetaCodigo = "PC-009", DataColeta = D(2026, 9, 6, 9, 45), Materiais = { M("plastico", 9.4, "cooperativa"), M("isopor", 3.1, "usina_reciclagem") }, Veiculo = new() { Placa = "ECO-2B45", Tipo = "caminhao_diesel" }, Operador = "Marta Silva", Rota = "R-OESTE-02", ValorReciclagemEstimado = 21.30 },
        new() { Codigo = "CL-1009", PontoColetaCodigo = "PC-010", DataColeta = D(2026, 9, 7, 8), Materiais = { M("organico", 76.0, "compostagem") }, Veiculo = new() { Placa = "ECO-5E12", Tipo = "triciclo_eletrico" }, Operador = "Rafael Lima", Rota = "R-OESTE-04" },
        new() { Codigo = "CL-1010", PontoColetaCodigo = "PC-011", DataColeta = D(2026, 9, 8, 6, 50), Materiais = { M("vidro", 41.3, "usina_reciclagem"), M("metal", 12.8, "usina_reciclagem"), M("papel", 28.0, "cooperativa") }, Veiculo = new() { Placa = "RIO-7F34", Tipo = "caminhao_eletrico" }, Operador = "Beatriz Costa", Rota = "R-ORLA-01", ValorReciclagemEstimado = 97.60 }
    };

    public static List<Usuario> Usuarios() => new()
    {
        new() { Nome = "Bruno Ferreira", Email = "bruno.f@email.com", Tipo = TipoUsuario.Individual, Cpf = "123.456.789-00", Endereco = End("Centro"), PontosRecompensa = 320, Gamificacao = new() { Nivel = "Reciclador Ouro", Badges = { "Primeiro Descarte", "30 Dias Seguidos" } }, DataCadastro = D(2025, 1, 15) },
        new() { Nome = "EcoTech Soluções", Email = "contato@ecotech.com", Tipo = TipoUsuario.Empresa, Cnpj = "22.333.444/0001-55", RazaoSocial = "EcoTech Soluções Ambientais S.A.", Contato = new() { Responsavel = "Patrícia Nunes", Telefone = "(11) 4444-5555" }, Endereco = End("Bela Vista"), PontosRecompensa = 1580, DataCadastro = D(2024, 9, 22) },
        new() { Nome = "Recicla Mais Ltda", Email = "contato@reciclamais.com", Tipo = TipoUsuario.Empresa, Cnpj = "12.345.678/0001-90", RazaoSocial = "Recicla Mais Comércio de Recicláveis Ltda", Contato = new() { Responsavel = "Jorge Almeida", Telefone = "(11) 3333-2222" }, Endereco = End("Mooca"), PontosRecompensa = 1200, DataCadastro = D(2024, 11, 3) },
        new() { Nome = "Condomínio Verde", Email = "sindico@condominioverde.com", Tipo = TipoUsuario.Empresa, Cnpj = "98.765.432/0001-10", RazaoSocial = "Condomínio Residencial Verde", Contato = new() { Responsavel = "Sérgio Prado", Telefone = "(11) 2222-1111" }, Endereco = End("Pinheiros"), PontosRecompensa = 640, DataCadastro = D(2025, 2, 10) },
        new() { Nome = "Padaria Pão Nosso", Email = "contato@paonosso.com", Tipo = TipoUsuario.Empresa, Cnpj = "11.222.333/0001-44", RazaoSocial = "Pão Nosso Panificadora Ltda", Contato = new() { Responsavel = "Luiza Rocha", Telefone = "(11) 5555-6666" }, Endereco = End("Lapa"), PontosRecompensa = 430, DataCadastro = D(2025, 4, 1) },
        new() { Nome = "Pedro Martins", Email = "pedro.m@email.com", Tipo = TipoUsuario.Individual, Cpf = "234.567.890-11", Endereco = End("Vila Mariana"), PontosRecompensa = 280, Gamificacao = new() { Nivel = "Reciclador Prata", Badges = { "Primeiro Descarte" } }, DataCadastro = D(2025, 3, 8) },
        new() { Nome = "Lucas Andrade", Email = "lucas.a@email.com", Tipo = TipoUsuario.Individual, Cpf = "345.678.901-22", Endereco = End("Copacabana", "Rio de Janeiro", "RJ"), PontosRecompensa = 150, DataCadastro = D(2025, 6, 20) },
        new() { Nome = "Beatriz Souza", Email = "beatriz.s@email.com", Tipo = TipoUsuario.Individual, Cpf = "456.789.012-33", Endereco = End("Tatuapé"), PontosRecompensa = 90, DataCadastro = D(2025, 8, 14) },
        new() { Nome = "Ana Costa", Email = "ana.c@email.com", Tipo = TipoUsuario.Individual, Cpf = "567.890.123-44", Endereco = End("Butantã"), PontosRecompensa = 510, Gamificacao = new() { Nivel = "Reciclador Ouro", Badges = { "Primeiro Descarte", "Mestre da Compostagem" } }, DataCadastro = D(2024, 12, 2) },
        new() { Nome = "Mariana Lopes", Email = "mariana.l@email.com", Tipo = TipoUsuario.Individual, Cpf = "678.901.234-55", Endereco = End("Consolação"), PontosRecompensa = 60, DataCadastro = D(2026, 1, 9) }
    };

    public static List<Notificacao> Notificacoes() => new()
    {
        new() { UsuarioEmail = "bruno.f@email.com", Tipo = TipoNotificacao.AlertaLimite, Titulo = "Ecoponto quase cheio", Mensagem = "O Ecoponto Praça Central atingiu 92% da capacidade. Prefira outro ponto próximo.", DataEnvio = D(2026, 9, 8, 7, 16), Contexto = new() { PontoCodigo = "PC-001", NivelPercent = 92 } },
        new() { UsuarioEmail = "pedro.m@email.com", Tipo = TipoNotificacao.OrientacaoDescarte, Titulo = "Como descartar pilhas", Mensagem = "Pilhas e baterias devem ir para pontos de coleta especializados.", DataEnvio = D(2026, 9, 2, 10), Contexto = new() { Material = "pilha_bateria", Instrucao = "ponto_descarte_especial" } },
        new() { UsuarioEmail = "contato@reciclamais.com", Tipo = TipoNotificacao.AlertaLimite, Titulo = "Container em nível crítico", Mensagem = "O Ecoponto Lapa está com 90% da capacidade.", DataEnvio = D(2026, 9, 7, 18), Contexto = new() { PontoCodigo = "PC-009", NivelPercent = 90 } },
        new() { UsuarioEmail = "lucas.a@email.com", Tipo = TipoNotificacao.AlertaLimite, Titulo = "Lixeira cheia perto de você", Mensagem = "A Lixeira Inteligente Orla Rio atingiu 94% da capacidade.", DataEnvio = D(2026, 9, 8, 6), Contexto = new() { PontoCodigo = "PC-011", NivelPercent = 94 } },
        new() { UsuarioEmail = "ana.c@email.com", Tipo = TipoNotificacao.Recompensa, Titulo = "Você ganhou 50 pontos!", Mensagem = "Obrigado pelo descarte correto de resíduos orgânicos.", Lida = true, DataEnvio = D(2026, 9, 1, 12), Contexto = new() { PontosGanhos = 50 } },
        new() { UsuarioEmail = "sindico@condominioverde.com", Tipo = TipoNotificacao.ColetaAgendada, Titulo = "Coleta agendada", Mensagem = "A coleta do PEV Shopping Eldorado foi agendada.", DataEnvio = D(2026, 9, 3, 9), Contexto = new() { PontoCodigo = "PC-004", DataAgendada = D(2026, 9, 4, 8) } },
        new() { UsuarioEmail = "mariana.l@email.com", Tipo = TipoNotificacao.OrientacaoDescarte, Titulo = "Óleo de cozinha não vai na pia", Mensagem = "Armazene o óleo usado em garrafa PET e leve a um PEV.", DataEnvio = D(2026, 9, 4, 19), Contexto = new() { Material = "oleo_cozinha", Instrucao = "garrafa_pet_pev" } },
        new() { UsuarioEmail = "beatriz.s@email.com", Tipo = TipoNotificacao.OrientacaoDescarte, Titulo = "Vidro: cuidado no descarte", Mensagem = "Embrulhe vidros quebrados em jornal antes de descartar.", Lida = true, DataEnvio = D(2026, 9, 1, 8), Contexto = new() { Material = "vidro", Instrucao = "embrulhar_jornal" } },
        new() { UsuarioEmail = "contato@ecotech.com", Tipo = TipoNotificacao.Recompensa, Titulo = "Empresa destaque do mês", Mensagem = "A EcoTech recebeu 200 pontos pelo volume reciclado.", DataEnvio = D(2026, 9, 5, 10), Contexto = new() { PontosGanhos = 200 } },
        new() { UsuarioEmail = "contato@paonosso.com", Tipo = TipoNotificacao.ColetaAgendada, Titulo = "Coleta de orgânicos amanhã", Mensagem = "A composteira comunitária receberá seus resíduos orgânicos.", DataEnvio = D(2026, 9, 6, 17), Contexto = new() { PontoCodigo = "PC-010", DataAgendada = D(2026, 9, 7, 8) } }
    };

    private static ImpactoAmbiental I(double co2, double agua) => new() { Co2EvitadoKgPorKg = co2, AguaEconomizadaLitros = agua };

    public static List<MaterialReciclavel> Materiais() => new()
    {
        new() { Nome = "Papel", Categoria = "papel", CorColetaSeletiva = "azul", InstrucoesDescarte = "Descarte limpo e seco, sem gordura. Dobre caixas de papelão.", TempoDecomposicao = "3 a 6 meses", ValorKgEstimado = 0.80, ImpactoAmbiental = I(1.5, 50), Subtipos = new() { "papelão", "jornal", "revista", "sulfite" } },
        new() { Nome = "Plástico", Categoria = "plastico", CorColetaSeletiva = "vermelho", InstrucoesDescarte = "Enxágue as embalagens e amasse para reduzir volume.", TempoDecomposicao = "até 400 anos", ValorKgEstimado = 1.20, ImpactoAmbiental = I(2.0, 40), Subtipos = new() { "PET", "PEAD", "PP" } },
        new() { Nome = "Vidro", Categoria = "vidro", CorColetaSeletiva = "verde", InstrucoesDescarte = "Enxágue e, se estiver quebrado, embrulhe em jornal.", TempoDecomposicao = "indeterminado", ValorKgEstimado = 0.30, ImpactoAmbiental = I(0.3, 5) },
        new() { Nome = "Metal", Categoria = "metal", CorColetaSeletiva = "amarelo", InstrucoesDescarte = "Enxágue latas e amasse. Não descarte aerossóis cheios.", TempoDecomposicao = "100 a 500 anos", ValorKgEstimado = 4.50, ImpactoAmbiental = I(9.0, 30), Subtipos = new() { "alumínio", "aço" } },
        new() { Nome = "Orgânico", Categoria = "organico", CorColetaSeletiva = "marrom", InstrucoesDescarte = "Restos de alimentos e podas vão para a compostagem.", TempoDecomposicao = "2 a 12 meses", ValorKgEstimado = 0.10, ImpactoAmbiental = I(0.5, 0) },
        new() { Nome = "Eletrônico", Categoria = "eletronico", CorColetaSeletiva = "laranja", InstrucoesDescarte = "Leve a um PEV com logística reversa. Apague seus dados antes.", TempoDecomposicao = "indeterminado", ValorKgEstimado = 6.00, ImpactoAmbiental = I(15.0, 100), AlertaEspecial = "Contém metais pesados. Descarte incorreto é crime ambiental.", Subtipos = new() { "celular", "computador", "eletrodoméstico", "cabo" } },
        new() { Nome = "Pilha e bateria", Categoria = "pilha_bateria", CorColetaSeletiva = "especial", InstrucoesDescarte = "Descarte SOMENTE em pontos de coleta especializados.", TempoDecomposicao = "indeterminado", ValorKgEstimado = 2.00, ImpactoAmbiental = I(5.0, 0), AlertaEspecial = "Metais tóxicos (mercúrio, cádmio). Risco de contaminação do solo." },
        new() { Nome = "Óleo de cozinha", Categoria = "oleo_cozinha", CorColetaSeletiva = "especial", InstrucoesDescarte = "Armazene em garrafa PET fechada e leve a um PEV.", ValorKgEstimado = 1.00, ImpactoAmbiental = I(2.5, 25000), AlertaEspecial = "1 litro de óleo pode contaminar 25 mil litros de água." },
        new() { Nome = "Isopor", Categoria = "isopor", CorColetaSeletiva = "vermelho", InstrucoesDescarte = "Descarte limpo, sem restos de alimentos.", TempoDecomposicao = "até 150 anos", ValorKgEstimado = 0.50, ImpactoAmbiental = I(1.0, 0) },
        new() { Nome = "Lâmpada", Categoria = "lampada", Reciclavel = true, CorColetaSeletiva = "especial", InstrucoesDescarte = "Não quebre. Leve à loja ou PEV com logística reversa.", TempoDecomposicao = "indeterminado", ValorKgEstimado = 0.50, AlertaEspecial = "Lâmpadas fluorescentes contêm mercúrio." }
    };
}
