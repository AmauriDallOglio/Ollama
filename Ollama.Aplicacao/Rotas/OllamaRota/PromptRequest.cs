using Ollama.Aplicacao.Util;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptRequest : IRequest<ResultadoOperacao>
    {

        public string Pergunta { get; set; } = string.Empty;
    }
}
