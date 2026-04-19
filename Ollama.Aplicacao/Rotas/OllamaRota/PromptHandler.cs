using Ollama.Aplicacao.Util;
using Ollama.Servico.Ollama;
using System.Diagnostics;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptHandler : IContratoBaseHandler<PromptRequest, ResultadoOperacao>
    {
        private readonly IOllamaServico _ollamaServico;

        public PromptHandler(IOllamaServico ollamaServico)
        {
            _ollamaServico = ollamaServico;
        }

        public async Task<ResultadoOperacao> Executar(PromptRequest request, CancellationToken cancellationToken = default)
        {
            var tempo = Stopwatch.StartNew();

            if (string.IsNullOrWhiteSpace(request.Pergunta))
            {
                tempo.Stop();
                return ResultadoOperacao.GerarErro("Campos devem ser informados!", 400);
            }

            var resposta = await _ollamaServico.ProcessaPerguntaRagAsync(request.Pergunta, "Sistema", cancellationToken);

            tempo.Stop();

            if (!string.IsNullOrEmpty(resposta))
            {
                var response = PromptResponse.Criar(request.Pergunta, resposta, tempo.ElapsedMilliseconds);
                return ResultadoOperacao.GerarSucesso(response);
            }
            else
            {
                return ResultadoOperacao.GerarErro("Não foi possível gerar resposta.", 500);
            }
        }
    }
}
