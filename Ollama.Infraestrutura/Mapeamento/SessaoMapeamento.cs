using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ollama.Dominio.Entidade;

namespace Ollama.Infraestrutura.Mapeamento
{
    public class SessaoMapeamento :  IEntityTypeConfiguration<Sessao>
    {
        public void Configure(EntityTypeBuilder<Sessao> builder)
        {
            builder.ToTable("Sessao");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Pergunta).IsRequired();
            builder.Property(s => s.PromptMontado).IsRequired();
            builder.Property(s => s.RespostaModelo).IsRequired();
            builder.Property(s => s.ContextosUtilizados);
            builder.Property(s => s.RespostaCorreta).HasDefaultValue(false);
            builder.Property(s => s.FeedbackUsuario);
            builder.Property(s => s.DataCriacao).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(s => s.DataAtualizacao);
        }
    }
}


/*
 * 
 * 
 * 
CREATE TABLE Sessao (
    Id INT IDENTITY PRIMARY KEY,
    Pergunta NVARCHAR(MAX) NOT NULL,
    PromptMontado NVARCHAR(MAX) NOT NULL,
    RespostaModelo NVARCHAR(MAX) NOT NULL,
    ContextosUtilizados NVARCHAR(MAX) NULL, -- lista de IDs ou títulos
    RespostaCorreta BIT NOT NULL DEFAULT 0,
    FeedbackUsuario NVARCHAR(MAX) NULL,
    DataCriacao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    DataAtualizacao DATETIME2 NULL
);
 * 
 * 
 */