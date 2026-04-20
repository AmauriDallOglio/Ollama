using Ollama.Servico.Ollama.Dto;

namespace Ollama.Servico.Ollama.Interface
{
    public interface IEngenhariaPromptDocumentos
    {
        Task<string> ObterPromptComBaseDocumentos(  string pergunta,   List<DocumentoContextoDto> documentos, CancellationToken cancellationToken);
    }
}
