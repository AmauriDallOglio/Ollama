using Ollama.Servico.Ollama.Dto;

namespace Ollama.Servico.Ollama.Interface
{
    public interface IOllamaServico
    {
        public Task<string> ExecutaPromptGeneraticoAsync(string pergunta, string promptMontado, string usuario, CancellationToken cancellationToken);
        public Task<string> ExecutaPromptAsync(string promptCompleto, CancellationToken cancellationToken);
        public Task<string> GerarPromptGenerativo(string pergunta, List<DocumentoContextoDto> documentos, CancellationToken cancellationToken);


        //public string PromptOrdemServico(string manutentor);
        public string PromptOrdemServicoHtml(string manutentor);
    }
}
