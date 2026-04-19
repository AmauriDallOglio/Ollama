using Ollama.Aplicacao.Dto;

namespace Ollama.Aplicacao.Servico
{
    public class EngenhariaPromptBase
    {
        private static readonly Random _random = new Random();
        private readonly AppSettingsDto _appSettings;

        public EngenhariaPromptBase(Microsoft.Extensions.Options.IOptionsMonitor<AppSettingsDto> options)
        {
            _appSettings = options.CurrentValue;
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

        private string MontarPrompt(string persona, string contexto, string prompt)
        {

            PromptResponseDto promptDto = new PromptResponseDto(persona, contexto, prompt);
            string promptFormatado = string.Join("\n", promptDto.Mensagens.Select(m => $"{m.Papel.ToUpper()}: {m.Conteudo}"));

            return promptFormatado;


        }
    }
}
