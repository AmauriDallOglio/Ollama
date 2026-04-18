using Microsoft.EntityFrameworkCore;

namespace Ollama.Infraestrutura.Contexto
{
    public class GenericoContexto : DbContext
    {

        public GenericoContexto(DbContextOptions options) : base(options)
        {
        }


        //public DbSet<Documento> Documento { get; set; }

        //public DbSet<Historico> Historico { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.ApplyConfiguration(new DocumentoMapeamento());
        //    modelBuilder.ApplyConfiguration(new HistoricoMapeamento());

        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
