using Ollama.Aplicacao.Dto;
using System.Text;
using System.Text.RegularExpressions;

namespace Ollama.Aplicacao.Servico
{
    public class EngenhariaPromptDocumentos
    {
        private static readonly Random _random = new Random();

        private readonly List<DocumentoContextoDto> _documentos;
    
        public EngenhariaPromptDocumentos(List<DocumentoContextoDto> documentos)
        {
            _documentos = documentos ?? new List<DocumentoContextoDto>();


        }


        public async Task<string> ObterPromptComBaseDocumentos(string pergunta, CancellationToken cancellationToken)
        {
            // Recupera os trechos de contexto
            var documentos = ObterDocumentosComBaseNaPergunta(pergunta).ToList();
            if (documentos is null || documentos.Count == 0)
            {
                return string.Empty;
            }
            // Monta o prompt com instruções + contexto relevante
            var sb = new StringBuilder();
            sb.AppendLine("Você é um assistente especializado. Use estritamente o contexto abaixo para responder.");
            sb.AppendLine();
            sb.AppendLine("--- CONTEXTO RELEVANTE ---");
            int i = 1;
            foreach (var d in documentos)
            {
                sb.AppendLine($"[{i}] {d.Titulo}: {d.Texto}");
                sb.AppendLine();
                i++;
            }
            sb.AppendLine("--- FIM DO CONTEXTO ---");
            sb.AppendLine();
            sb.AppendLine("Pergunta:");
            sb.AppendLine(pergunta);
            sb.AppendLine();
            sb.AppendLine("Instrução: Seja objetivo, indique a fonte [n] quando usar um dos trechos acima. Se não houver informação suficiente, admita que não sabe.");

            string promptCompleto = sb.ToString();

            return promptCompleto;
        }

        // Busca simples por similaridade: pontua documentos pela frequência de termos (TF simples)
        private IEnumerable<DocumentoContextoDto> ObterDocumentosComBaseNaPergunta(string pergunta)
        {
            if (string.IsNullOrWhiteSpace(pergunta))
                return Enumerable.Empty<DocumentoContextoDto>();

            // Quebra o prompt em tokens
            List<string> tokensPergunta = Tokenizar(pergunta);

            List<(DocumentoContextoDto Documento, int Score)> documentosComScore = new List<(DocumentoContextoDto, int)>();
            foreach (var documento in _documentos)
            {
                var tokensDocumento = Tokenizar($"{documento.Titulo} {documento.Texto}");

                // Conta quantas vezes cada termo aparece nos tokens
                int score = 0;
                foreach (var token in tokensPergunta)
                {
                    int achou = tokensDocumento.Count(tokenDocumento => tokenDocumento == token);
                    score += achou;
                }
                if (score > 0)
                {
                    documentosComScore.Add((documento, score));
                }

            }

            //Filtra apenas os assuntos relevantes
            var documentosSelecionados = documentosComScore.Where(x => x.Score > 0).OrderByDescending(x => x.Score).Select(x => x.Documento);

            return documentosSelecionados;
        }




        private static List<string> Tokenizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return new();

            texto = texto.ToLowerInvariant();

            // remove pontuação e divide por espaço
            var ignorarPalavras = new HashSet<string> {     
                "a", "o", "os", "as",
                "um", "uma", "uns", "umas",
                "de", "do", "da", "dos", "das",
                "em", "no", "na", "nos", "nas",
                "para", "por", "com", "sem",
                "e", "ou", "mas",
                "que", "se", "sua", "seu", "suas", "seus",
                "ao", "aos", "à", "às",
                "sobre", "entre", "até", "após",
                "como", "quando", "onde",
                "já", "não", "sim"};
            var palavras = Regex.Split(texto, @"\W+")     // remove pontuação e divide por espaço
                .Where(w => w.Length > 1 && !ignorarPalavras.Contains(w))
                .ToList();

            return palavras;
        }
    }
}
