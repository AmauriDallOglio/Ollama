namespace Ollama.Aplicacao.Dto
{
    public class MensagemDto
    {
        public string Papel { get; set; }   // system, user, assistant
        public string Conteudo { get; set; }

        public MensagemDto(string papel, string conteudo)
        {
            Papel = papel;
            Conteudo = conteudo;
        }
    }
}
