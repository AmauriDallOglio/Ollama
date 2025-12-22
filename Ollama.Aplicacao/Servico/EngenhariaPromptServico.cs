using Ollama.Aplicacao.Dto;
using System.Text;

namespace Ollama.Aplicacao.Servico
{
    public class EngenhariaPromptServico
    {
        private static readonly Random _random = new Random();
        public PromptResponseDto PromptManutencao(string Pergunta)
        {
            string persona = @"Você é um especialista em manutenção de máquinas industriais, construção civil, predial e veículos voltados ao mundo industrial, 
                com amplo conhecimento em manutenção preventiva, preditiva e corretiva. 
                Sua função é fornecer respostas técnicas, detalhadas e práticas, considerando boas práticas e normas de segurança, sempre explique de forma clara e estruturada. 
                Se não souber a resposta, diga: Desculpe, não encontrei informações sobre isso na minha base de dados.";

            string contexto = @"
                - Manutenção preventiva: inspeções periódicas, lubrificação, troca de filtros, calibragem.
                - Manutenção preditiva: monitoramento por sensores (vibração, temperatura, pressão), análise de falhas, histórico de operação.
                - Manutenção corretiva: reparos após falha, substituição de peças danificadas, diagnóstico de problemas.
                - Normas de segurança: uso de EPIs, bloqueio de energia antes de manutenção, registro de manutenções.
                - Observações específicas:
                  * Tear: inspecionar lançadeiras, lubrificar partes móveis, verificar alinhamento dos quadros e revisar sistemas eletrônicos de controle.
                  * Revisadeira: checar integridade dos rolos, motor e transmissão, ajustar tensões de enrolamento, inspecionar sensores de contagem e sistemas de segurança.
                  * Injetora de plástico: verificar sistemas de aquecimento e refrigeração, calibrar pressão de injeção, inspecionar bicos e válvulas.
                  * Fresadora CNC: calibrar eixos, lubrificar guias lineares, verificar fusos e motores de passo, atualizar software de controle.
                  * Extrusora: inspecionar roscas, cilindros e resistências, monitorar temperatura, checar desgaste de matrizes.
                  * Compressores: verificar pressão, drenagem de condensado, troca de óleo e filtros, monitorar temperatura de operação.
                  * Esteiras transportadoras: checar alinhamento de correias, tensão dos rolos, lubrificação de mancais, inspeção de motores.
                  * Caldeiras: inspecionar válvulas de segurança, controlar pressão e temperatura, realizar testes de estanqueidade, limpar tubulações de combustão.
         
                ";
            PromptResponseDto promptDto = new PromptResponseDto(persona, contexto, Pergunta);
            return promptDto;
        }

        public PromptResponseDto PromptRevisaoTexto(string textoOriginal)
        {
            string persona = @"Você é um especialista em revisão e estruturação de textos acadêmicos, técnicos e literários. 
                Sua função é organizar e formatar textos de forma clara, lógica e padronizada, sem alterar o conteúdo original. 
                Sempre que estruturar o texto, utilize capítulos e seções, aplicando títulos coerentes e organizados. 
                Nunca modifique as ideias, apenas estruture e formate.";

            string contexto = @"
                - Não reescreva ou altere o conteúdo do texto fornecido.
                - Mantenha todas as ideias originais do autor.
                - Divida o texto em capítulos e subcapítulos coerentes.
                - Gere a saída de uma só vez, já formatada em estrutura de capítulos.
                - Use um formato estruturado, como:
                    Capítulo 1 - Introdução
                    Capítulo 2 - Desenvolvimento
                    2.1 Subtópico A
                    2.2 Subtópico B
                    Capítulo 3 - Conclusão
                - Saída final sempre em **texto organizado e numerado**, sem comentários extras.";

            PromptResponseDto promptDto = new PromptResponseDto(persona, contexto, textoOriginal);
            return promptDto;
        }

        public PromptResponseDto PromptOrdemServico(string manutentor)
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

            List<OrdemServicoDto>? listaOrdensServico = GerarListaOrdensServico(manutentor);
            var prompt = ConverterParaTexto(listaOrdensServico);

            PromptResponseDto promptDto = new PromptResponseDto(persona, contexto, prompt);
            return promptDto;
        }

        public PromptResponseDto PromptOrdemServicoHtml(string manutentor)
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
                - Priorize as ordens com status 'Parada', 'EmExecucao', 'Agendada'. 
                - Gere também uma estimativa do que o manutentor deve priorizar hoje, como se fosse uma orientação direta do chefe da manutenção.
                - Finalize com um resumo prático: 'Bom trabalho!'.
                - Gere a resposta em html e css para ser apresentada em um navegador.
            ";

            List<OrdemServicoDto>? listaOrdensServico = GerarListaOrdensServico(manutentor);
            var prompt = ConverterParaTexto(listaOrdensServico);

            PromptResponseDto promptDto = new PromptResponseDto(persona, contexto, prompt);
            return promptDto;
        }

         

 

        public List<OrdemServicoDto> GerarListaOrdensServico(string nomeManutentor)
        {
            var ordens = new List<OrdemServicoDto>();
            DateTime hoje = DateTime.Now;
            int codigo = 1;

            // 10 passadas
            for (int i = 1; i <= 8; i++)
            {
                ordens.Add(CriarOrdem(codigo++, hoje.AddDays(-i), nomeManutentor, "Manutentor 2"));
            }

            // 5 no dia atual
            for (int i = 0; i < 12; i++)
            {
                ordens.Add(CriarOrdem(codigo++, hoje, nomeManutentor, "Manutentor 2"));
            }

            // 15 futuras
            for (int i = 1; i <= 15; i++)
            {
                ordens.Add(CriarOrdem(codigo++, hoje.AddDays(i), nomeManutentor, "Manutentor 2"));
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

        public string ConverterParaTexto(List<OrdemServicoDto> ordens)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Lista de Ordens de Serviço:");
            foreach (var os in ordens)
            {
                sb.AppendLine($"- Código: {os.Codigo}, Data: {os.DataCadastro:dd/MM/yyyy}, " +
                              $"Status: {os.Status}, Tempo Estimado: {os.TempoEstimadoHoras}h, " +
                              $"Manutentor: {os.Manutentor}");
            }
            return sb.ToString();
        }

 




    }
}
