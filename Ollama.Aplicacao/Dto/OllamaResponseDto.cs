namespace Ollama.Aplicacao.Dto
{
    public class OllamaResponseDto
    {
        public string Resposta = string.Empty;
        public long TempoMs;

        public OllamaResponseDto(string resposta, long tempoMs)
        {
            Resposta = resposta;
            TempoMs = tempoMs;
        }
    }
}
