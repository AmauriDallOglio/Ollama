using Microsoft.EntityFrameworkCore;

namespace Ollama.Infraestrutura.Contexto
{
    public class CommandContexto : GenericoContexto
    {
        public CommandContexto(DbContextOptions<CommandContexto> options) : base(options)
        {
        }
    }
}
