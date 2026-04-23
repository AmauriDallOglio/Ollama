namespace Ollama.Dominio.Entidade
{
    public class Sessao
    {
        public int Id { get; set; }
        public string Pergunta { get; set; } = string.Empty;
        public string PromptMontado { get; set; } = string.Empty;
        public string RespostaModelo { get; set; } = string.Empty;

        public string? ContextosUtilizados { get; set; } // lista de IDs ou títulos
        public bool RespostaCorreta { get; set; } = false;
        public string? FeedbackUsuario { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }
    }
}
