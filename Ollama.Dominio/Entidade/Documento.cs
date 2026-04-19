namespace Ollama.Dominio.Entidade
{
    public class Documento
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;

        public string? TipoArquivo { get; set; }   // PDF, TXT, DOCX
        public long? TamanhoArquivo { get; set; }  // em bytes

        public DateTime DataImportacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }

        protected Documento() { }

        // Construtor de domínio
        public static Documento Criar(string titulo, string texto, string? tipoArquivo, long? tamanhoArquivo)
        {
            return new Documento
            {
                Titulo = titulo,
                Texto = SanitizarTexto(texto),
                TipoArquivo = tipoArquivo,
                TamanhoArquivo = tamanhoArquivo,
                DataImportacao = DateTime.UtcNow
            };
        }

        // sanitizar texto
        private static string SanitizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            // Normaliza quebras de linha para espaços simples
            return texto.Replace("\r\n", " ").Replace("\n", " ").Trim();
        }

        // atualizar texto
        public void AtualizarTexto(string novoTexto)
        {
            Texto = SanitizarTexto(novoTexto);
            DataAtualizacao = DateTime.UtcNow;
        }




        // Método de domínio para atualizar metadados
        public void AtualizarMetadados(string? tipoArquivo, long? tamanhoArquivo)
        {
            TipoArquivo = tipoArquivo;
            TamanhoArquivo = tamanhoArquivo;
            DataAtualizacao = DateTime.UtcNow;
        }


    }
}
