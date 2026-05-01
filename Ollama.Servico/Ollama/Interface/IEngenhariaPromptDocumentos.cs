using Ollama.Servico.Ollama.Dto;

namespace Ollama.Servico.Ollama.Interface
{
    public interface IEngenhariaPromptDocumentos
    {
        Task<string> GerarPrompt(  string pergunta,   List<DocumentoContextoDto> documentos, CancellationToken cancellationToken);
    }
}
