using Ollama.Aplicacao.Dto;

namespace Ollama.Aplicacao.Servico
{
    /// <summary>
    /// Serviço para registrar e consultar logs de interação para aprendizado de máquina.
    /// </summary>
    public class SessaoMemoriaServico
    {
        private readonly List<SessaoMemoriaDto> _logs = new();

        /// <summary>
        /// Registra uma nova interação.
        /// </summary>
        public Task RegistrarAsync(SessaoMemoriaDto log, CancellationToken cancellationToken)
        {
            _logs.Add(log);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Retorna todos os logs registrados.
        /// </summary>
        public Task<List<SessaoMemoriaDto>> ObterTodosAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_logs);
        }
    }
}
