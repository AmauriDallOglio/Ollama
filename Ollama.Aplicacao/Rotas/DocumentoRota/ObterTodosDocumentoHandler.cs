using Ollama.Aplicacao.Util;
using Ollama.Dominio.InterfaceRepositorio;

namespace Ollama.Aplicacao.Rotas.DocumentoRota
{
    public class ObterTodosDocumentoHandler : IContratoBaseHandler<ObterTodosDocumentoRequest, ResultadoOperacao>
    {
        private readonly IDocumentoCommandRepositorio _documentoRepositorio;

        public ObterTodosDocumentoHandler(IDocumentoCommandRepositorio documentoRepositorio)
        {
            _documentoRepositorio = documentoRepositorio;
        }

        public async Task<ResultadoOperacao> Executar(ObterTodosDocumentoRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentos = await _documentoRepositorio.ObterTodosAsync(cancellationToken);
                var response = ObterTodosDocumentoResponse.CriarLista(documentos);

                return ResultadoOperacao.GerarSucesso(response);
            }
            catch (Exception ex)
            {
                return ResultadoOperacao.GerarErro($"Erro interno: {ex.Message}", 500);
            }
        }
    }
}
