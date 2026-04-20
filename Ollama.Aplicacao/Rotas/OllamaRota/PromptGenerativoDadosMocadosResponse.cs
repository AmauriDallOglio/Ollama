using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ollama.Aplicacao.Rotas.OllamaRota
{
    public class PromptGenerativoDadosMocadosResponse
    {
        public string Pergunta { get; set; } = string.Empty;
        public string Resposta { get; set; } = string.Empty;
        public long TempoExecucaoMs { get; set; }

        public static PromptGenerativoDadosMocadosResponse Criar(string pergunta, string resposta, long tempoExecucaoMs)
        {
            return new PromptGenerativoDadosMocadosResponse
            {
                Pergunta = pergunta,
                Resposta = resposta,
                TempoExecucaoMs = tempoExecucaoMs
            };
        }
    }
}
