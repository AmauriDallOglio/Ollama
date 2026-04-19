using Microsoft.EntityFrameworkCore;
using Ollama.Dominio.Entidade;
using Ollama.Infraestrutura.Mapeamento;

namespace Ollama.Infraestrutura.Contexto
{
    public class GenericoContexto : DbContext
    {

        public GenericoContexto(DbContextOptions options) : base(options)
        {
        }


        public DbSet<Documento> Documento { get; set; }

        public DbSet<Sessao> Sessao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new DocumentoMapeamento());
            modelBuilder.ApplyConfiguration(new SessaoMapeamento());

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
