using Ollama.Dominio.Entidade;

namespace Ollama.Aplicacao.Rotas.DocumentoRota
{
    public class ImportarDocumentoResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string TipoArquivo { get; set; } = string.Empty;
        public long TamanhoArquivo { get; set; }
        public DateTime DataImportacao { get; set; }

        public static ImportarDocumentoResponse Criar(Documento entidade)
        {
            return new ImportarDocumentoResponse
            {
                Id = entidade.Id,
                Titulo = entidade.Titulo,
                TipoArquivo = entidade.TipoArquivo ?? string.Empty,
                TamanhoArquivo = entidade.TamanhoArquivo ?? 0,
                DataImportacao = entidade.DataImportacao
            };
        }
    }
}
