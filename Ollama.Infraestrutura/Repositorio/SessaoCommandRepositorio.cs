using Ollama.Dominio.Entidade;
using Ollama.Dominio.InterfaceRepositorio;
using Ollama.Infraestrutura.Contexto;
using Ollama.Infraestrutura.Repositorio.Configuracao;

namespace Ollama.Infraestrutura.Repositorio
{
    public class SessaoCommandRepositorio : GenericoCommandRepositorio<Sessao>, ISessaoCommandRepositorio
    {
        private readonly CommandContexto _CommandContexto;
        public SessaoCommandRepositorio(CommandContexto dbContext) : base(dbContext)
        {
            _CommandContexto = dbContext;
        }
    }
}