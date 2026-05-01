using Ollama.Aplicacao.Util;
using Ollama.Dominio.InterfaceRepositorio;
using Ollama.Servico.Ollama.Dto;
using Ollama.Servico.Ollama.Interface;
using System.Diagnostics;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptGenerativoHandler : IContratoBaseHandler<PromptGenerativoRequest, ResultadoOperacao>
    {
        private readonly IEngenhariaPromptDocumentos _engenhariaPromptDocumentos;
        private readonly IOllamaServico _ollamaServico;
        private readonly IDocumentoCommandRepositorio _documentoCommandRepositorio;
        //private readonly ISessaoMemoriaServico _sessaoMemoriaServico;

        private static readonly SemaphoreSlim _semaforoCacheDocumentos = new(1, 1);
        private static readonly TimeSpan _tempoValidadeCacheDocumentos = TimeSpan.FromMinutes(2);
        private static DateTime _ultimaAtualizacaoCacheDocumentos = DateTime.MinValue;
        private static List<DocumentoContextoDto> _cacheDocumentos = new();

        public PromptGenerativoHandler(
            IEngenhariaPromptDocumentos engenhariaPromptDocumentos,
            IDocumentoCommandRepositorio documentoCommandRepositorio,
            IOllamaServico ollamaServico)
        {
            _engenhariaPromptDocumentos = engenhariaPromptDocumentos;
            _ollamaServico = ollamaServico;
            _documentoCommandRepositorio = documentoCommandRepositorio;
           // _sessaoMemoriaServico = sessaoMemoriaServico;
        }

        public async Task<ResultadoOperacao> Executar(PromptGenerativoRequest request, CancellationToken cancellationToken = default)
        {
            Stopwatch tempo = Stopwatch.StartNew();

            if (string.IsNullOrWhiteSpace(request.Pergunta))
            {
                tempo.Stop();
                return ResultadoOperacao.GerarErro("Campos devem ser informados!", 400);
            }

            //Carrega os documentos
            List<DocumentoContextoDto> documentos = await CarregarDocumentos(cancellationToken);

            //Valida se tem documentos para processar
            string promptGenerativo = await _engenhariaPromptDocumentos.GerarPrompt(request.Pergunta, documentos, cancellationToken);
            if (string.IsNullOrEmpty(promptGenerativo))
            {
                tempo.Stop();
                return ResultadoOperacao.GerarErro("Desculpe, nao encontrei informacoes sobre isso na minha base de dados.", 500);
            }

            //Chama o serviço do Ollama para processar o prompt
            string resposta = await _ollamaServico.ProcessaPromptDocumentosAsync(request.Pergunta, promptGenerativo, "Sistema", cancellationToken);

            tempo.Stop();
            if (!string.IsNullOrEmpty(resposta))
            {
                PromptGenerativoResponse response = PromptGenerativoResponse.Criar(request.Pergunta, resposta, tempo.ElapsedMilliseconds);
                return ResultadoOperacao.GerarSucesso(response);
            }
            return ResultadoOperacao.GerarErro("Nao foi possivel gerar resposta.", 500);
        }

        private async Task<List<DocumentoContextoDto>> CarregarDocumentos(CancellationToken cancellationToken)
        {
            await _semaforoCacheDocumentos.WaitAsync(cancellationToken);
            try
            {
                DateTime agora = DateTime.Now;
                if (_cacheDocumentos.Count > 0 && (agora - _ultimaAtualizacaoCacheDocumentos) <= _tempoValidadeCacheDocumentos)
                    return _cacheDocumentos;

                var documentosDb = await _documentoCommandRepositorio.ObterTodosAsync(cancellationToken);
                _cacheDocumentos = documentosDb.Select(d => new DocumentoContextoDto(d.Id, d.Titulo, d.Texto)).ToList();
                _ultimaAtualizacaoCacheDocumentos = DateTime.Now;
                return _cacheDocumentos;
            }
            finally
            {
                _semaforoCacheDocumentos.Release();
            }
        }
    }
}
