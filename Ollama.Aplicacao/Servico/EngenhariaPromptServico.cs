using Ollama.Aplicacao.Dto;
using System.Text;
using System.Text.RegularExpressions;

namespace Ollama.Aplicacao.Servico
{
    public class EngenhariaPromptServico
    {
        private static readonly Random _random = new Random();
        private readonly List<DocumentoContextoDto> _documentos = [];

        public EngenhariaPromptServico()
        {
            // Exemplo: inicializa alguns documentos. Em produção carregue de BD/arquivo.
            _documentos.Add(new DocumentoContextoDto("1", "Batman", "É um super-herói da DC conhecido como o Cavaleiro das Trevas, que protege Gotham City."));
            _documentos.Add(new DocumentoContextoDto("2", "Superman", "É um super-herói da DC vindo de Krypton, com poderes como superforça, visão de calor e voo."));
            _documentos.Add(new DocumentoContextoDto("3", "Mulher-Maravilha", "É uma amazona guerreira da DC, com força sobre-humana e o Laço da Verdade."));
            _documentos.Add(new DocumentoContextoDto("4", "Flash", "É o velocista escarlate da DC, capaz de correr em velocidades incríveis e manipular o tempo."));
            _documentos.Add(new DocumentoContextoDto("5", "Aquaman", "É o rei de Atlântida na DC, com poderes de controlar o mar e se comunicar com criaturas marinhas."));
        }

        public string PromptSessao(string pergunta, string sessaoMemoria)
        {
            string persona = @$"
                Você é um especialista no assunto enviado.
                - Se não souber a resposta, diga exatamente: Desculpe, não encontrei informações sobre isso na minha base de dados.'
            ";

            string contexto = @$"
                - Inicialize a resposta contextualizando o assunto da sessão.
                - Utilize o conteúdo da sessão como base principal para responder.
                - Se a pergunta não tiver relação com o assunto da sessão, informe que não há dados disponíveis.
                - Evite respostas muito longas, seja direto e preciso.
            ";

            return MontarPrompt(persona, contexto, sessaoMemoria + $"\n\nPergunta: {pergunta}");
        }


        public string PromptOrdemServico(string manutentor)
        {
            string persona = @$"
                Você é um especialista em PCM (Planejamento e Controle da Manutenção) e organizador do trabalho dos manutentores. 
                Sua função é analisar a lista de Ordens de Serviço recebida, avaliar os prazos e status, e organizar as atividades como se fosse o chefe da manutenção orientando o manutentor {manutentor}. 
                Sempre fale de forma objetiva e clara, simulando uma comunicação prática de rotina como um técnico de manutenção.
            ";

            string contexto = @$"
                - Inicialize falando: Olá {manutentor} bom dia, seu cronograma de trabalho para hoje: .
                - Analise a lista de Ordens de Serviço recebida em texto, destinada para o manutentor {manutentor} .
                - Manutentor {manutentor} tem disponibilidade total de trabalho de 8h por dia, iniciando seu turno as 05:00 até as 13:30, parando das 09:00 até as 09:30 para descanso.
                - Calculando meu inicio de trabalho na hora de execução desse prompt.
                - Informe de forma clara:
                    1. Quantas ordens de serviço estão atrasadas, apresentando abaixo os registros em tabela.
                    2. Quantas podem ser atendidas no dia de hoje {DateTime.Now}, apresentando abaixo os registros em tabela.
                    3. Quantas podem ser atendidas no futuro, apresentando abaixo os registros em tabela.
                - Priorize as ordens com status 'Parada', 'EmExecucao', 'Agendada'. 
                - Gere também uma estimativa do que o manutentor deve priorizar hoje, como se fosse uma orientação direta do chefe da manutenção.
                - Apresente os dados de forma organizada em lista ou tópicos.
                - Finalize com um resumo prático: 'Bom trabalho!'.
                - Apresente a lista completa das ordens de serviço recebida, organizada por data de cadastro, do mais antigo para o mais recente.
            ";

            string ordensServico = ObterListaOrdemServico(manutentor);
            return MontarPrompt(persona, contexto, ordensServico);
        }

        public string PromptOrdemServicoHtml(string manutentor)
        {
            string persona = @$"
                Você é um especialista em PCM (Planejamento e Controle da Manutenção) e organizador do trabalho dos manutentores. 
                Sua função é analisar a lista de Ordens de Serviço recebida, avaliar os prazos e status, e organizar as atividades como se fosse o chefe da manutenção orientando o manutentor {manutentor}. 
                Sempre fale de forma objetiva e clara, simulando uma comunicação prática de rotina como um técnico de manutenção.
            ";

            string dataatual = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string contexto = @$"
                - Inicialize falando: Olá {manutentor}, bom dia! Seu cronograma de trabalho para hoje: .
                - Analise a lista de Ordens de Serviço recebida em texto, destinada para o manutentor {manutentor} .
                - Manutentor {manutentor} tem disponibilidade total de trabalho de 8h por dia, iniciando seu turno as 08:00 até as 18:00, parando das 12:00 até as 13:30 para descanso.
                - Calculando meu inicio de trabalho as {dataatual}.
                - Priorize as ordens com status 'Parada', 'EmExecucao', 'Agendada'. 
                - Gere também uma estimativa do que o manutentor deve priorizar hoje, como se fosse uma orientação direta do chefe da manutenção.
                - Finalize com um resumo prático: 'Bom trabalho!'.
                - Gere a resposta em html e css para ser apresentada em um navegador.
            ";

            string ordensServico = ObterListaOrdemServico(manutentor);
            return MontarPrompt(persona, contexto, ordensServico);
        }

        private string ObterListaOrdemServico( string manutentor)
        {
            List<OrdemServicoDto>? listaOrdensServico = GerarListaOrdensServico(manutentor);
            string ordens = OrdemServicoConverterParaTexto(listaOrdensServico);
            return ordens;
        }

        private string MontarPrompt(string persona, string contexto, string prompt)
        {

            PromptResponseDto promptDto = new PromptResponseDto(persona, contexto, prompt);
            string promptFormatado =  string.Join("\n", promptDto.Mensagens.Select(m => $"{m.Papel.ToUpper()}: {m.Conteudo}"));

            return promptFormatado;
        }

        public string OrdemServicoConverterParaTexto(List<OrdemServicoDto> ordens)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Lista de Ordens de Serviço:");
            foreach (var os in ordens)
            {
                sb.AppendLine($"- Código: {os.Codigo}, Data: {os.DataCadastro:dd/MM/yyyy}, " +
                              $"Status: {os.Status}, Tempo Estimado: {os.TempoEstimadoHoras}h, " +
                              $"Manutentor: {os.Manutentor}");
            }
            return sb.ToString();
        }


        public List<OrdemServicoDto> GerarListaOrdensServico(string nomeManutentor)
        {
            var ordens = new List<OrdemServicoDto>();
            DateTime hoje = DateTime.Now;
            int codigo = 1;

            // 10 passadas
            for (int i = 1; i <= 8; i++)
            {
                ordens.Add(CriarOrdem(codigo++, hoje.AddDays(-i), nomeManutentor, "Maximus Decimus Meridius"));
            }

            // 5 no dia atual
            for (int i = 0; i < 12; i++)
            {
                ordens.Add(CriarOrdem(codigo++, hoje, nomeManutentor, "Maximus Decimus Meridius"));
            }

            // 15 futuras
            for (int i = 1; i <= 15; i++)
            {
                ordens.Add(CriarOrdem(codigo++, hoje.AddDays(i), nomeManutentor, "Maximus Decimus Meridius"));
            }

            return ordens;
        }

        private static OrdemServicoDto CriarOrdem(int codigo, DateTime data, string manutentor1, string manutentor2)
        {
            return new OrdemServicoDto
            {
                Codigo = codigo.ToString(),
                DataCadastro = data,
                Status = GerarStatusAleatorio(),
                TempoEstimadoHoras = (int)Math.Round(_random.NextDouble() * 8 + 1, 2), // entre 1 e 9 horas
                Manutentor = codigo % 2 == 0 ? manutentor1 : manutentor2
            };
        }

        private static StatusOrdemServico GerarStatusAleatorio()
        {
            var valores = Enum.GetValues(typeof(StatusOrdemServico));
            return (StatusOrdemServico)valores.GetValue(_random.Next(valores.Length))!;
        }




        public string ObterPromptComBaseDocumentos(string assunto, CancellationToken cancellationToken)
        {
            // Recupera os trechos de contexto
            var docs = ObterDocumentos(assunto).ToList();
            if (docs is null || docs.Count == 0)
            {
                return string.Empty;
            }
            // Monta o prompt com instruções + contexto relevante
            var sb = new StringBuilder();
            sb.AppendLine("Você é um assistente especializado. Use estritamente o contexto abaixo para responder.");
            sb.AppendLine();
            sb.AppendLine("--- CONTEXTO RELEVANTE ---");
            int i = 1;
            foreach (var d in docs)
            {
                sb.AppendLine($"[{i}] {d.Titulo}: {d.Texto}");
                sb.AppendLine();
                i++;
            }
            sb.AppendLine("--- FIM DO CONTEXTO ---");
            sb.AppendLine();
            sb.AppendLine("Pergunta:");
            sb.AppendLine(assunto);
            sb.AppendLine();
            sb.AppendLine("Instrução: Seja objetivo, indique a fonte [n] quando usar um dos trechos acima. Se não houver informação suficiente, admita que não sabe.");

            string promptCompleto = sb.ToString();
            return promptCompleto;
        }

        // Busca simples por similaridade: pontua documentos pela frequência de termos (TF simples)
        private IEnumerable<DocumentoContextoDto> ObterDocumentos(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return Enumerable.Empty<DocumentoContextoDto>();

            // Quebra o prompt em tokens
            List<string> promptTokens = Tokenizar(prompt);

            // Calcula o score de cada documento
            var documentosComScore = _documentos.
                Select(doc => new
                {
                    Documento = doc,
                    Score = CalcularScore(doc, promptTokens)
                });

            //Filtra apenas os assuntos relevantes
            var assuntosEncontrados = documentosComScore.Where(x => x.Score > 0).OrderByDescending(x => x.Score).Select(x => x.Documento);

            return assuntosEncontrados;
        }

        private int CalcularScore(DocumentoContextoDto doc, IEnumerable<string> promptTokens)
        {
            var tokensDocumento = Tokenizar($"{doc.Titulo} {doc.Texto}");

            // Conta quantas vezes cada termo aparece nos tokens
            int score = 0;
            foreach (var token in promptTokens)
            {
                int achou = tokensDocumento.Count(tokenDocumento => tokenDocumento == token);
                score += achou;
            }

            return score;
        }


        private static List<string> Tokenizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return new();
            texto = texto.ToLowerInvariant();
            // remove pontuação e divide por espaço
            var words = Regex.Split(texto, @"\W+").Where(w => w.Length > 0).ToList();
            return words;
        }



    }
}
