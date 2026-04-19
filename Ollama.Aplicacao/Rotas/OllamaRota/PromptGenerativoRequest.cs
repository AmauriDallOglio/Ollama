using Ollama.Aplicacao.Util;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptGenerativoRequest : IRequest<ResultadoOperacao>
    {
        public string Pergunta { get; set; } = string.Empty;
    }

}
