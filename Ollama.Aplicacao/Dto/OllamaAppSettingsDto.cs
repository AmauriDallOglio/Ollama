namespace Ollama.Aplicacao.Dto
{
    public class OllamaAppSettingsDto
    {
        public string UrlBase { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int TempoLimite { get; set; } = 0;
        public string Tipo { get; set; } = string.Empty;

        public string Idioma { get; set; } = string.Empty;


    }
}
 