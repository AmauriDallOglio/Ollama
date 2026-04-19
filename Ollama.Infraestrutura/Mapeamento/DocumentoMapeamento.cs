using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ollama.Dominio.Entidade;

namespace Ollama.Infraestrutura.Mapeamento
{
    public class DocumentoMapeamento : IEntityTypeConfiguration<Documento>
    {
        public void Configure(EntityTypeBuilder<Documento> builder)
        {
            builder.ToTable("Documento");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Titulo).IsRequired().HasMaxLength(200);
            builder.Property(d => d.Texto).IsRequired();
            builder.Property(d => d.TipoArquivo).HasMaxLength(50);
            builder.Property(d => d.TamanhoArquivo);
            builder.Property(d => d.DataImportacao).HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(d => d.DataAtualizacao);
        }
    }
}


/*
 * 
 * 
 * 
use Ollama 


CREATE TABLE Documento (
    Id INT IDENTITY PRIMARY KEY,
    Titulo NVARCHAR(200) NOT NULL,
    Texto NVARCHAR(MAX) NOT NULL,

    TipoArquivo NVARCHAR(50) NULL,        -- ex: PDF, TXT, DOCX
    TamanhoArquivo BIGINT NULL,           -- em bytes
    DataImportacao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DataAtualizacao DATETIME2 NULL
);




 * 
 */