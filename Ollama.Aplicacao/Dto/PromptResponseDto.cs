namespace Ollama.Aplicacao.Dto
{
    public class PromptResponseDto
    {
        public List<MensagemDto> Mensagens { get; set; }

        public PromptResponseDto(string persona, string contexto, string pergunta)
        {
            Mensagens = new List<MensagemDto>
            {
                new MensagemDto("system",  $"Persona: {persona}"),
                new MensagemDto("system", $"Contexto: {contexto}"),
                new MensagemDto("user",  $"Pergunta: {pergunta}")
            };
        }
    }


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
