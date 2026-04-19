namespace Ollama.Servico.Ollama
{
    public interface IEngenhariaPromptDocumentos
    {
        Task<string> ObterPromptComBaseDocumentos(string pergunta, List<DocumentoContextoDto> documentos, CancellationToken cancellationToken);
    }
}
