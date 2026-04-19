using Microsoft.EntityFrameworkCore;
using Ollama.Dominio.InterfaceRepositorio.Configurcao;
using Ollama.Infraestrutura.Contexto;

namespace Ollama.Infraestrutura.Repositorio.Configuracao
{
    public class GenericoQueryRepositorio<T> : IGenericoQueryRepositorio<T> where T : class
    {
        protected readonly GenericoContexto _GenericoContexto;
        private readonly DbSet<T> _dbSet;

        public GenericoQueryRepositorio(GenericoContexto context)
        {
            _GenericoContexto = context;
            _dbSet = _GenericoContexto.Set<T>();
        }



        public async Task<T> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var resultado = await _dbSet.FindAsync(id, cancellationToken);
            return resultado;
        }

        public async Task<List<T>> ObterTodosAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }
    }
}

