using Ollama.Dominio.Entidade;

namespace Ollama.Aplicacao.Rotas.SessaoRota
{
    public class ObterTodosSessaoResponse
    {
        public int Id { get; set; }
        public string Pergunta { get; set; } = string.Empty;
        public string PromptMontado { get; set; } = string.Empty;
        public string RespostaModelo { get; set; } = string.Empty;
        public string? ContextosUtilizados { get; set; }
        public bool RespostaCorreta { get; set; } = false;
        public string? FeedbackUsuario { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }

        public static ObterTodosSessaoResponse Criar(Sessao entidade)
        {
            return new ObterTodosSessaoResponse
            {
                Id = entidade.Id,
                Pergunta = entidade.Pergunta,
                PromptMontado = entidade.PromptMontado,
                RespostaModelo = entidade.RespostaModelo,
                ContextosUtilizados = entidade.ContextosUtilizados,
                RespostaCorreta = entidade.RespostaCorreta,
                FeedbackUsuario = entidade.FeedbackUsuario,
                DataCriacao = entidade.DataCriacao,
                DataAtualizacao = entidade.DataAtualizacao
            };
        }

        public static List<ObterTodosSessaoResponse> CriarLista(IEnumerable<Sessao> sessoes)
        {
            return sessoes.Select(Criar).ToList();
        }
    }
}
