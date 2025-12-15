using Ollama.Aplicacao.Dto;
using System.Text;
using System.Text.RegularExpressions;

namespace Ollama.Aplicacao.Servico
{
    public class PromptDocumentoServico
    {
        private readonly List<DocumentoContextoDto> _documentos = [];

        public PromptDocumentoServico()
        {
            // Exemplo: inicializa alguns documentos. Em produção carregue de BD/arquivo.
            _documentos.Add(new DocumentoContextoDto("1", "História da API", "APIs são contratos entre sistemas..."));
            _documentos.Add(new DocumentoContextoDto("2", "O que é Ollama", "Ollama é um servidor de modelos LLM que permite..."));
            _documentos.Add(new DocumentoContextoDto("3", "Uso em C#", "Para usar Ollama em C#, faça HTTP POST para /api/generate..."));
            _documentos.Add(new DocumentoContextoDto("4", "Batman", "É um super herói da DC..."));
            _documentos.Add(new DocumentoContextoDto("5", "Batman e robin", "É um super herói da DC amigo do Robin..."));
        }


        public string ObterPromptComBaseDocumentos(string assunto, CancellationToken cancellationToken)
        {
            // 1) Recupera os trechos de contexto
            var docs = ObterDocumentos(assunto).ToList();
            if (docs is null || docs.Count == 0)
            {
                return string.Empty;
            }
            // 2) Monta o prompt com instruções + contexto relevante
            var sb = new StringBuilder();
            sb.AppendLine("Você é um assistente especializado. Use estritamente o contexto abaixo para responder.");
            sb.AppendLine();
            sb.AppendLine("--- CONTEXTO RELEVANTE ---");
            int i = 1;
            foreach (var d in docs)
            {
                sb.AppendLine($"[{i}] {d.Titulo}: {d.Texto}");
                sb.AppendLine();
                i++;
            }
            sb.AppendLine("--- FIM DO CONTEXTO ---");
            sb.AppendLine();
            sb.AppendLine("Pergunta:");
            sb.AppendLine(assunto);
            sb.AppendLine();
            sb.AppendLine("Instrução: Seja objetivo, indique a fonte [n] quando usar um dos trechos acima. Se não houver informação suficiente, admita que não sabe.");

            string promptCompleto = sb.ToString();
            return promptCompleto;
        }

        // Busca simples por similaridade: pontua documentos pela frequência de termos (TF simples)
        private IEnumerable<DocumentoContextoDto> ObterDocumentos(string assunto)
        {
            if (string.IsNullOrWhiteSpace(assunto)) 
                return Enumerable.Empty<DocumentoContextoDto>();

            List<string> termosQuebraToken = Tokenizar(assunto);
            IEnumerable<DocumentoContextoDto> assuntosAssociados = _documentos.Select(doc =>
                {
                    var tokensDoc = Tokenizar(doc.Texto + " " + doc.Titulo);
                    // score: soma de ocorrências de cada termo
                    int score = termosQuebraToken.Sum(t => tokensDoc.Count(td => td == t));
                    return new { Doc = doc, Score = score };
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Doc);

            return assuntosAssociados;
        }

        private static List<string> Tokenizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return new();
            texto = texto.ToLowerInvariant();
            // remove pontuação e divide por espaço
            var words = Regex.Split(texto, @"\W+").Where(w => w.Length > 0).ToList();
            return words;
        }

  

            
    
       
    }
}
