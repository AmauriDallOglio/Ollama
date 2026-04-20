using Ollama.Servico.Ollama.Dto;

namespace Ollama.Servico.Ollama.Interface
{
    public interface ISessaoMemoriaServico
    {
        Task RegistrarAsync(SessaoMemoriaDto log, CancellationToken cancellationToken);
        Task<List<SessaoMemoriaDto>> ObterTodosAsync(CancellationToken cancellationToken);
       // Task<List<SessaoMemoriaDto>> ObterUltimasInteracoesPorConversaAsync(string idConversa, int quantidadeMaxima, CancellationToken cancellationToken);
        Task ProcessadoAsync(int id, CancellationToken cancellationToken);
        Task RemoverAsync(CancellationToken cancellationToken);
    }
}
