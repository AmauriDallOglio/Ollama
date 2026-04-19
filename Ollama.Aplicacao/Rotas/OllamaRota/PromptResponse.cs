namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptResponse
    {
        public string Pergunta { get; set; } = string.Empty;
        public string Resposta { get; set; } = string.Empty;
        public long TempoExecucaoMs { get; set; }

        public static PromptResponse Criar(string pergunta, string resposta, long tempoExecucaoMs)
        {
            return new PromptResponse
            {
                Pergunta = pergunta,
                Resposta = resposta,
                TempoExecucaoMs = tempoExecucaoMs
            };
        }
    }
}
