namespace Ollama.Aplicacao.Dto
{
    public class AppSettingsDto
    {
        public TipoServidorDto TipoServidor { get; set; } = new();
        public OllamaConfigDto OllamaLocal { get; set; } = new();
        public OllamaConfigDto OllamaDocker { get; set; } = new();
    }

    public class TipoServidorDto
    {
        public int Tipo { get; set; }
    }

    public class OllamaConfigDto
    {
        public string Servidor { get; set; } = string.Empty;
        public string UrlBase { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int TempoLimiteSegundos { get; set; } = 0;
        public string Tipo { get; set; } = string.Empty;
        public string Idioma { get; set; } = string.Empty;
    }


    public class TipoServidorSelecionado
    {
        public string UrlBase { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int TempoLimiteSegundos { get; set; } = 0;
        public string Tipo { get; set; } = string.Empty;
        public string Idioma { get; set; } = string.Empty;



        public TipoServidorSelecionado CarregarLocal(AppSettingsDto _appSettings)
        {
            UrlBase = _appSettings.OllamaLocal.UrlBase;
            Modelo = _appSettings.OllamaLocal.Modelo;
            TempoLimiteSegundos = _appSettings.OllamaLocal.TempoLimiteSegundos;
            Idioma = _appSettings.OllamaLocal.Idioma;
            Tipo = _appSettings.OllamaLocal.Tipo;
            return this;
        }

        public TipoServidorSelecionado CarregarDocker(AppSettingsDto _appSettings)
        {
            UrlBase = _appSettings.OllamaDocker.UrlBase;
            Modelo = _appSettings.OllamaDocker.Modelo;
            TempoLimiteSegundos = _appSettings.OllamaDocker.TempoLimiteSegundos;
            Idioma = _appSettings.OllamaDocker.Idioma;
            Tipo = _appSettings.OllamaDocker.Tipo;
            return this;
        }
    }
}
 