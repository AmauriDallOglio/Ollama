using Ollama.Servico.Ollama.Dto;
using Ollama.Servico.Ollama.Interface;
using System.Text;

namespace Ollama.Aplicacao.Servico
{
    public class EngenhariaPromptDadosMocados : IEngenhariaPromptDadosMocados
    {
        private static readonly Random _random = new Random();

        public EngenhariaPromptDadosMocados()
        {

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

        private string ObterListaOrdemServico(string manutentor)
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

        private string OrdemServicoConverterParaTexto(List<OrdemServicoDto> ordens)
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


        private List<OrdemServicoDto> GerarListaOrdensServico(string nomeManutentor)
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


         
    }
}
