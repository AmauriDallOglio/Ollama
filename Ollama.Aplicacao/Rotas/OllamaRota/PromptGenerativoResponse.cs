using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptGenerativoResponse
    {
        public string Pergunta { get; set; } = string.Empty;
        public string Resposta { get; set; } = string.Empty;
        public long TempoExecucaoMs { get; set; }

        public static PromptGenerativoResponse Criar(string pergunta, string resposta, long tempoExecucaoMs)
        {
            return new PromptGenerativoResponse
            {
                Pergunta = pergunta,
                Resposta = resposta,
                TempoExecucaoMs = tempoExecucaoMs
            };
        }
    }

}
