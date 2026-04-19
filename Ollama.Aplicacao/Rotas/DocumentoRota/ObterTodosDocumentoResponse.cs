using Ollama.Dominio.Entidade;

namespace Ollama.Aplicacao.Rotas.DocumentoRota
{
    public class ObterTodosDocumentoResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public string? TipoArquivo { get; set; }
        public long? TamanhoArquivo { get; set; }
        public DateTime DataImportacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public static ObterTodosDocumentoResponse Criar(Documento entidade)
        {
            return new ObterTodosDocumentoResponse
            {
                Id = entidade.Id,
                Titulo = entidade.Titulo,
                Texto = entidade.Texto,
                TipoArquivo = entidade.TipoArquivo,
                TamanhoArquivo = entidade.TamanhoArquivo,
                DataImportacao = entidade.DataImportacao,
                DataAtualizacao = entidade.DataAtualizacao
            };
        }

        public static List<ObterTodosDocumentoResponse> CriarLista(IEnumerable<Documento> documentos)
        {
            return documentos.Select(Criar).ToList();
        }
    }
}
