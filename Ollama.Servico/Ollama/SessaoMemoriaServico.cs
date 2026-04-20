using Ollama.Servico.Ollama.Dto;
using Ollama.Servico.Ollama.Interface;

namespace Ollama.Aplicacao.Servico
{
    public class SessaoMemoriaServico : ISessaoMemoriaServico
    {
        private readonly List<SessaoMemoriaDto> _memoriaSessao = new();
        private readonly object _travaMemoria = new();
        private int _sequencialId = 0;

        public Task RegistrarAsync(SessaoMemoriaDto log, CancellationToken cancellationToken)
        {
            lock (_travaMemoria)
            {
                _sequencialId++;
                log.Id = _sequencialId;
                log.Remover = false;
             //   log.IdConversa = string.IsNullOrWhiteSpace(log.IdConversa) ? "conversa-padrao" : log.IdConversa.Trim();
                _memoriaSessao.Add(log);
            }

            return Task.CompletedTask;
        }

        public Task<List<SessaoMemoriaDto>> ObterTodosAsync(CancellationToken cancellationToken)
        {
            lock (_travaMemoria)
            {
                return Task.FromResult(_memoriaSessao.Where(x => x.Remover == false).ToList());
            }
        }

        //public Task<List<SessaoMemoriaDto>> ObterUltimasInteracoesPorConversaAsync(string idConversa, int quantidadeMaxima, CancellationToken cancellationToken)
        //{
        //    string conversa = string.IsNullOrWhiteSpace(idConversa) ? "conversa-padrao" : idConversa.Trim();
        //    int limite = quantidadeMaxima <= 0 ? 4 : quantidadeMaxima;

        //    lock (_travaMemoria)
        //    {
        //        List<SessaoMemoriaDto> interacoes = _memoriaSessao
        //            .Where(x => !x.Remover && string.Equals(x.IdConversa, conversa, StringComparison.OrdinalIgnoreCase))
        //            .OrderByDescending(x => x.DataHora)
        //            .Take(limite)
        //            .OrderBy(x => x.DataHora)
        //            .ToList();

        //        return Task.FromResult(interacoes);
        //    }
        //}

        public Task ProcessadoAsync(int id, CancellationToken cancellationToken)
        {
            lock (_travaMemoria)
            {
                SessaoMemoriaDto? item = _memoriaSessao.FirstOrDefault(x => x.Id == id);
                if (item != null)
                    item.Remover = true;
            }

            return Task.CompletedTask;
        }

        public Task RemoverAsync(CancellationToken cancellationToken)
        {
            lock (_travaMemoria)
            {
                _memoriaSessao.RemoveAll(x => x.Remover);
            }

            return Task.CompletedTask;
        }
    }
}
