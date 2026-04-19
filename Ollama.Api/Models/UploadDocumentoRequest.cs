using Microsoft.AspNetCore.Http;

namespace Ollama.Api.Models
{
    public class UploadDocumentoRequest
    {
        public IFormFile Arquivo { get; set; }
    }
}