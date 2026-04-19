using Ollama.Aplicacao.Util;
using Ollama.Dominio.InterfaceRepositorio;

namespace Ollama.Aplicacao.Rotas.SessaoRota
{
    public class ObterTodosSessaoHandler : IContratoBaseHandler<ObterTodosSessaoRequest, ResultadoOperacao>
    {
        private readonly ISessaoCommandRepositorio _sessaoRepositorio;

        public ObterTodosSessaoHandler(ISessaoCommandRepositorio sessaoRepositorio)
        {
            _sessaoRepositorio = sessaoRepositorio;
        }

        public async Task<ResultadoOperacao> Executar(ObterTodosSessaoRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var sessoes = await _sessaoRepositorio.ObterTodosAsync(cancellationToken);
                var response = ObterTodosSessaoResponse.CriarLista(sessoes);

                return ResultadoOperacao.GerarSucesso(response);
            }
            catch (Exception ex)
            {
                return ResultadoOperacao.GerarErro($"Erro interno: {ex.Message}", 500);
            }
        }
    }
}
