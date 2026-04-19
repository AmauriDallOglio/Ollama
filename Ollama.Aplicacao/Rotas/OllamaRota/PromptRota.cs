using Ollama.Aplicacao.Util;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptRota
    {
        public sealed class IncluirExercicioRequest : IRequest<ResultadoOperacao>
        {
            public int IdTenant { get; set; }
            public int IdUsuario { get; set; }
            public short Ano { get; set; }
            public string? Descricao { get; set; }
        }
    }
}
