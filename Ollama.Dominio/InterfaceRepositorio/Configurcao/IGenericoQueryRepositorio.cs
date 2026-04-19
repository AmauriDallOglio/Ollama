namespace Ollama.Dominio.InterfaceRepositorio.Configurcao
{
    public interface IGenericoQueryRepositorio<T> where T : class
    {
        Task<List<T>> ObterTodosAsync(CancellationToken cancellationToken);
        Task<T> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    }
}
