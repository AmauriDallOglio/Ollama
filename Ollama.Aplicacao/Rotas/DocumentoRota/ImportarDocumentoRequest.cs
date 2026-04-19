using Microsoft.AspNetCore.Http;
using Ollama.Aplicacao.Util;

namespace Ollama.Aplicacao.Rotas.DocumentoRota
{
    public class ImportarDocumentoRequest : IRequest<ResultadoOperacao>
    {
        public IFormFile Arquivo { get; set; } = default!;
 
    }
}
