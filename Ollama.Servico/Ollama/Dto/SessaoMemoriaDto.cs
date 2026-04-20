namespace Ollama.Servico.Ollama.Dto
{
    /// <summary>
    /// DTO para registrar logs de interação para aprendizado de máquina.
    /// </summary>
    public class SessaoMemoriaDto
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
       // public string IdConversa { get; set; } = string.Empty;
        public string Pergunta { get; set; } = string.Empty;
        public string PromptMontado { get; set; } = string.Empty;
        public string RespostaModelo { get; set; } = string.Empty;
        public string[] ContextosUtilizados { get; set; } = new string[0];
        public string Usuario { get; set; } = string.Empty;
        public bool RespostaCorreta { get; set; } // Para feedback supervisionado
        public string FeedbackUsuario { get; set; } = string.Empty; // Comentário opcional do usuário
        public bool Remover { get; set; }
    }
}
