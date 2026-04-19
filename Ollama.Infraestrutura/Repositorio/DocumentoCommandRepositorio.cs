using Ollama.Dominio.Entidade;
using Ollama.Dominio.InterfaceRepositorio;
using Ollama.Infraestrutura.Contexto;
using Ollama.Infraestrutura.Repositorio.Configuracao;

namespace Ollama.Infraestrutura.Repositorio
{
    public class DocumentoCommandRepositorio : GenericoCommandRepositorio<Documento>, IDocumentoCommandRepositorio
    {
        private readonly CommandContexto _CommandContexto;
        public DocumentoCommandRepositorio(CommandContexto dbContext) : base(dbContext)
        {
            _CommandContexto = dbContext;
        }
    }
}