using Ollama.Aplicacao.Util;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptGenerativoDadosMocadosRequest : IRequest<ResultadoOperacao>
    {
        public string Pergunta { get; set; } = "Amauri";
    }
}
