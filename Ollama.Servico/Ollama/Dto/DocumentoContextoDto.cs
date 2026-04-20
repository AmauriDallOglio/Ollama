namespace Ollama.Servico.Ollama.Dto
{
    public class DocumentoContextoDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;

        public DocumentoContextoDto() { }

        public DocumentoContextoDto(int id, string titulo, string texto) : this()
        {
            this.Id = id;
            this.Titulo = titulo;
            this.Texto = texto;
        }

        public DocumentoContextoDto Criar(int id, string titulo, string texto) 
        {
            new DocumentoContextoDto
            {
                Id = id,
                Titulo = titulo,
                Texto = texto
            };
            return this;
        }

    }
}
