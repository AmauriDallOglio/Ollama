namespace Ollama.Aplicacao.Dto
{
    public class ResultadoOperacaoDto
    {
        public string Pergunta { get; set; } = string.Empty;
        public string Resposta { get; set; } = string.Empty;
        public string Tempo { get; set; } = string.Empty;
        public bool Sucesso { get; set; }

        public ResultadoOperacaoDto()
        {

        }
        public ResultadoOperacaoDto GeraSucesso(string pergunta, string resposta, long tempoMs)
        {
            this.Pergunta = pergunta;
            this.Resposta = resposta;

            double tempoSegundos = tempoMs / 1000.0; // Converte milissegundos para segundos
            this.Tempo = $"{tempoSegundos:F2} s";  // Exibe em segundos com 2 casas decimais

            this.Sucesso = true;
            return this;
        }


        public ResultadoOperacaoDto GeraErro(string pergunta, string resposta, long tempoMs)
        {

            this.Pergunta = pergunta;
            this.Resposta = resposta;

            double tempoSegundos = tempoMs / 1000.0; // Converte milissegundos para segundos
            this.Tempo = $"{tempoSegundos:F2} s";  // Exibe em segundos com 2 casas decimais

            this.Sucesso = false;
            return this;

        }

    }
}
