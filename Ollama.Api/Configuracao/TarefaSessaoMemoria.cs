using Ollama.Aplicacao.Util;
using Ollama.Dominio.Entidade;
using Ollama.Infraestrutura.Contexto;
using Ollama.Infraestrutura.Repositorio;
using Ollama.Servico.Ollama.Dto;
using Ollama.Servico.Ollama.Interface;

namespace Ollama.Api.Configuracao
{
    public class TarefaSessaoMemoria : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly TimeSpan _intervaloSucesso = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan _intervaloErro = TimeSpan.FromMinutes(2);

        public TarefaSessaoMemoria(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Aguarda a aplicação inicializar completamente
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                try
                {

                    PrintaConsole.Info($"Iniciando tarefa Sessão memória");
                    ISessaoMemoriaServico iSessaoMemoriaServico = scope.ServiceProvider.GetRequiredService<ISessaoMemoriaServico>();

                    CommandContexto commandContexto = scope.ServiceProvider.GetRequiredService<CommandContexto>();
                    SessaoCommandRepositorio sessaoCommandRepositorio = new(commandContexto);

                    List<SessaoMemoriaDto> memorias = await iSessaoMemoriaServico.ObterTodosAsync(cancellationToken);
                    foreach (SessaoMemoriaDto memoria in memorias)
                    {
                        PrintaConsole.Info($"Carregando sessão: {memoria.Pergunta}");
                        Sessao entidade = new Sessao
                        {
                            Pergunta = memoria.Pergunta,
                            PromptMontado = memoria.PromptMontado,
                            RespostaModelo = memoria.RespostaModelo,
                            ContextosUtilizados = string.Join(",", memoria.ContextosUtilizados),
                            RespostaCorreta = memoria.RespostaCorreta,
                            FeedbackUsuario = memoria.FeedbackUsuario,
                            DataCriacao = memoria.DataHora,
                            DataAtualizacao = DateTime.Now
                        };

                        await sessaoCommandRepositorio.IncluirAsync(entidade, cancellationToken);
                        await iSessaoMemoriaServico.ProcessadoAsync(memoria.Id, cancellationToken);
                    }


                    PrintaConsole.Info($"Finalizando tarefa sessão memória");
                    await Task.Delay(_intervaloSucesso, cancellationToken);
                }
                catch (OperationCanceledException exception)
                {
                    PrintaConsole.Error($"Erro ao executar TarefaSessaoMemoria: {exception.Message}");
                    break;
                }
                catch (Exception exception)
                {
                    PrintaConsole.Error($"Erro ao executar TarefaSessaoMemoria: {exception.Message}");

                    // Aguarda antes de tentar novamente
                    await Task.Delay(_intervaloErro, cancellationToken);
                }
            }
        }
    }
}
