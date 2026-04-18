using Microsoft.EntityFrameworkCore;

namespace Ollama.Infraestrutura.Contexto
{
    public class QueryContexto : GenericoContexto
    {
        public QueryContexto(DbContextOptions<QueryContexto> options) : base(options)
        {
        }

    }
}
