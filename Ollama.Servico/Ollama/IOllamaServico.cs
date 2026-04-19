namespace Ollama.Servico.Ollama
{
    public interface IOllamaServico
    {
        Task<string> ProcessaPerguntaRagAsync(string promptMontado, string usuario, CancellationToken cancellationToken);
        Task<string> ProcessaPromptAsync(string promptCompleto, CancellationToken cancellationToken);

    }
}
