namespace Ollama.Dominio.InterfaceRepositorio
{
    public interface IGenericoQueryRepositorio<T> where T : class
    {
        Task<List<T>> ObterTodosAsync(CancellationToken cancellationToken);
        Task<T> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    }
}
