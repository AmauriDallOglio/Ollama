using Ollama.Aplicacao.Util;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptGenerativoDadosMocadosRequest : IRequest<ResultadoOperacao>
    {
        public string Manutentor { get; set; } = "Amauri";
    }
}
