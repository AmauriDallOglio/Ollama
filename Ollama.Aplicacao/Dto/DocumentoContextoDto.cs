namespace Ollama.Aplicacao.Dto
{
    public class DocumentoContextoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;

        public DocumentoContextoDto() { }

        public DocumentoContextoDto(string id, string titulo, string texto) : this()
        {
            this.Id = id;
            this.Titulo = titulo;
            this.Texto = texto;
        }
    }
}
