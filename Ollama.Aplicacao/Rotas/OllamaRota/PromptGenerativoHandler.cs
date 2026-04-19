using Ollama.Aplicacao.Util;
using Ollama.Dominio.InterfaceRepositorio;
using Ollama.Servico.Ollama;
using System.Diagnostics;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptGenerativoHandler : IContratoBaseHandler<PromptGenerativoRequest, ResultadoOperacao>
    {
        private readonly IEngenhariaPromptDocumentos _iEngenhariaPromptDocumentos;
        private readonly IOllamaServico _ollamaServico;
        private readonly IDocumentoCommandRepositorio _IDocumentoCommandRepositorio;

        public PromptGenerativoHandler(
            IEngenhariaPromptDocumentos iEngenhariaPromptServico, 
            IDocumentoCommandRepositorio iDocumentoCommandRepositorio,
            IOllamaServico ollamaServico)
        {
            _iEngenhariaPromptDocumentos = iEngenhariaPromptServico;
            _ollamaServico = ollamaServico;
            _IDocumentoCommandRepositorio = iDocumentoCommandRepositorio;
        }

        public async Task<ResultadoOperacao> Executar(PromptGenerativoRequest request, CancellationToken cancellationToken = default)
        {
            var tempo = Stopwatch.StartNew();

            if (string.IsNullOrWhiteSpace(request.Pergunta))
            {
                tempo.Stop();
                return ResultadoOperacao.GerarErro("Campos devem ser informados!", 400);
            }

            var documentosDb = await _IDocumentoCommandRepositorio.ObterTodosAsync(cancellationToken);

            List<DocumentoContextoDto> documentos = documentosDb.Select(d => new DocumentoContextoDto
            {
                Id = d.Id,
                Titulo = d.Titulo,
                Texto = d.Texto
            }).ToList();

            // 1. Obter prompt baseado em documentos
            var promptBase = await _iEngenhariaPromptDocumentos.ObterPromptComBaseDocumentos(request.Pergunta,  documentos , cancellationToken);

            if (string.IsNullOrEmpty(promptBase))
            {
                tempo.Stop();
                return ResultadoOperacao.GerarErro("Não foi possível gerar prompt com base nos documentos.", 500);
            }

            // 2. Processar no Ollama
            var resposta = await _ollamaServico.ProcessaPerguntaRagAsync(promptBase, "Sistema", cancellationToken);

            tempo.Stop();

            if (!string.IsNullOrEmpty(resposta))
            {
                var response = PromptGenerativoResponse.Criar(request.Pergunta, resposta, tempo.ElapsedMilliseconds);
                return ResultadoOperacao.GerarSucesso(response);
            }
            else
            {
                return ResultadoOperacao.GerarErro("Não foi possível gerar resposta.", 500);
            }
        }
    }
}
