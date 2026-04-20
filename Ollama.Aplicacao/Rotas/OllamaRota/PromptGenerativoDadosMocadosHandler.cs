using Ollama.Aplicacao.Util;
using Ollama.Servico.Ollama.Interface;
using System.Diagnostics;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptGenerativoDadosMocadosHandler : IContratoBaseHandler<PromptGenerativoDadosMocadosRequest, ResultadoOperacao>
    {
        private readonly IEngenhariaPromptDadosMocados _engenhariaPromptDadosMocados;
        private readonly IOllamaServico _ollamaServico;

        public PromptGenerativoDadosMocadosHandler(
            IEngenhariaPromptDadosMocados engenhariaPromptDadosMocados, 
            IOllamaServico ollamaServico)
        {
            _engenhariaPromptDadosMocados = engenhariaPromptDadosMocados;
            _ollamaServico = ollamaServico;
        }

        public async Task<ResultadoOperacao> Executar(PromptGenerativoDadosMocadosRequest request, CancellationToken cancellationToken = default)
        {
            var tempo = Stopwatch.StartNew();

            // 1. Monta o prompt com dados mocados
            string prompt = _engenhariaPromptDadosMocados.PromptOrdemServicoHtml(request.Manutentor);

            // 2. Processa no Ollama
            string resposta = await _ollamaServico.ProcessaPromptAsync(prompt, cancellationToken);

            tempo.Stop();

            if (!string.IsNullOrEmpty(resposta))
            {
                var response = PromptGenerativoDadosMocadosResponse.Criar(prompt, resposta, tempo.ElapsedMilliseconds);
                return ResultadoOperacao.GerarSucesso(response);
            }
            else
            {
                return ResultadoOperacao.GerarErro("Não foi possível gerar resposta.", 500);
            }
        }
    }
}
