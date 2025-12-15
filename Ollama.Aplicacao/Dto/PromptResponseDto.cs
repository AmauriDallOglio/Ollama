namespace Ollama.Aplicacao.Dto
{
    public class PromptResponseDto
    {
        public List<MensagemDto> Mensagens { get; set; }

        public PromptResponseDto(string persona, string contexto, string pergunta)
        {
            Mensagens = new List<MensagemDto>
                {
                    //A persona diz ao modelo quem ele é e como deve se comportar. O texto define papel, estilo e regras. Sempre vai no system (ou no início do prompt).
                    new MensagemDto("system",  $"Persona: {persona}"),
                    //Fornece informações que a IA pode usar, nada além disso
                    new MensagemDto("system", $"Contexto: {contexto}"),
                    //A pergunta é o que o usuário quer saber, enviada como user: 
                    new MensagemDto("user",  $"Pergunta: {pergunta}")
                };
        }

        public string FormataToString()
        {
            return string.Join("\n", Mensagens.Select(m => $"{m.Papel.ToUpper()}: {m.Conteudo}"));
        }
    }
}
