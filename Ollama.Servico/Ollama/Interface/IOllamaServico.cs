namespace Ollama.Servico.Ollama.Interface
{
    public interface IOllamaServico
    {
        Task<string> ProcessaPromptDocumentosAsync(string pergunta, string promptMontado, string usuario, CancellationToken cancellationToken);
        Task<string> ProcessaPromptAsync(string promptCompleto, CancellationToken cancellationToken);

    }
}
