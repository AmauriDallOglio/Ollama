namespace Ollama.Aplicacao.Dto
{
    public class OllamaResponseDto
    {
        public string Pergunta { get; set; } = string.Empty;
        public string Resposta { get; set; } = string.Empty;
        public string TempoMs { get; set; } = string.Empty;
        public bool Sucesso { get; set; }

        public OllamaResponseDto()
        {

        }
        public OllamaResponseDto GeraSucesso(string pergunta, string resposta, long tempoMs)
        {
            this.Pergunta = pergunta;
            this.Resposta = resposta;
            this.TempoMs = $"{tempoMs} ms";
            this.Sucesso = true;
            return this;
        }

        public OllamaResponseDto GeraErro(string pergunta, string resposta, long tempoMs)
        {
            
            this.Pergunta = pergunta;
            this.Resposta = resposta;
            this.TempoMs = $"{tempoMs} ms";
            this.Sucesso = false;
            return this;
            
        }

    }
}
