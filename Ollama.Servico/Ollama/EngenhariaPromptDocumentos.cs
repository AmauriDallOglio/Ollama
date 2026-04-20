using Ollama.Servico.Ollama.Dto;
using Ollama.Servico.Ollama.Interface;
using System.Text;
using System.Text.RegularExpressions;

namespace Ollama.Servico.Ollama
{
    public class EngenhariaPromptDocumentos : IEngenhariaPromptDocumentos
    {
        private const int QuantidadeMaximaDocumentos = 8;
        private const int QuantidadeMaximaCaracteresPorDocumento = 1200;
        private const int QuantidadeMaximaInteracoesHistorico = 4;

        public Task<string> ObterPromptComBaseDocumentos( string pergunta, List<DocumentoContextoDto> documentos, CancellationToken cancellationToken)
        {
            List<DocumentoContextoDto> documentosFiltrados = ObterTopDocumentosRelevantes(pergunta, documentos, QuantidadeMaximaDocumentos).ToList();
            if (documentosFiltrados.Count == 0)
                return Task.FromResult(string.Empty);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Voce e um assistente especializado. Use estritamente o contexto abaixo para responder.");
            sb.AppendLine();

            //string blocoHistorico = MontarBlocoHistoricoConversa(historicoConversa, QuantidadeMaximaInteracoesHistorico);
            //if (!string.IsNullOrWhiteSpace(blocoHistorico))
            //{
            //    sb.AppendLine("--- HISTORICO DA CONVERSA ---");
            //    sb.AppendLine(blocoHistorico);
            //    sb.AppendLine("--- FIM DO HISTORICO ---");
            //    sb.AppendLine();
            //}

            sb.AppendLine("--- CONTEXTO RELEVANTE ---");
            foreach (DocumentoContextoDto documento in documentosFiltrados)
            {
                string textoResumido = LimitarTamanhoTexto(documento.Texto, QuantidadeMaximaCaracteresPorDocumento);
                sb.AppendLine($"Fonte: [{documento.Id}]. Titulo: {documento.Titulo}. Texto: {textoResumido}");
                sb.AppendLine();
            }
            sb.AppendLine("--- FIM DO CONTEXTO ---");
            sb.AppendLine();
            sb.AppendLine("Pergunta:");
            sb.AppendLine(pergunta);
            sb.AppendLine();
            sb.AppendLine("Instrucao:");
            sb.AppendLine("- Inicialize a resposta contextualizando o assunto da sessao.");
            sb.AppendLine("- Utilize o conteudo da sessao como base principal para responder.");
            sb.AppendLine("- Evite respostas longas, seja direto e preciso.");
            sb.AppendLine("- Se a pergunta nao tiver relacao com o assunto da sessao, diga exatamente: Desculpe, nao encontrei informacoes sobre isso na minha base de dados.");

            return Task.FromResult(sb.ToString());
        }

        //private static string MontarBlocoHistoricoConversa(List<SessaoMemoriaDto> historicoConversa, int quantidadeMaxima)
        //{
        //    if (historicoConversa == null || historicoConversa.Count == 0)
        //        return string.Empty;

        //    List<SessaoMemoriaDto> historicoLimitado = historicoConversa
        //        .OrderByDescending(x => x.DataHora)
        //        .Take(quantidadeMaxima <= 0 ? 4 : quantidadeMaxima)
        //        .OrderBy(x => x.DataHora)
        //        .ToList();

        //    StringBuilder sb = new StringBuilder();
        //    foreach (SessaoMemoriaDto item in historicoLimitado)
        //    {
        //        sb.AppendLine($"Pergunta anterior: {item.Pergunta}");
        //        sb.AppendLine($"Resposta anterior: {item.RespostaModelo}");
        //        sb.AppendLine();
        //    }

        //    return sb.ToString();
        //}

        private static IEnumerable<DocumentoContextoDto> ObterTopDocumentosRelevantes(string pergunta, List<DocumentoContextoDto> documentos, int quantidadeMaximaDocumentos)
        {
            if (string.IsNullOrWhiteSpace(pergunta) || documentos == null || documentos.Count == 0)
                return Enumerable.Empty<DocumentoContextoDto>();

            List<string> tokensPergunta = Tokenizar(pergunta);
            if (tokensPergunta.Count == 0)
                return Enumerable.Empty<DocumentoContextoDto>();

            List<(DocumentoContextoDto Documento, int Score)> documentosComScore = new List<(DocumentoContextoDto, int)>();
            foreach (DocumentoContextoDto documento in documentos)
            {
                List<string> tokensDocumento = Tokenizar($"{documento.Titulo} {documento.Texto}");

                int score = 0;
                foreach (string token in tokensPergunta)
                {
                    int ocorrencias = tokensDocumento.Count(tokenDocumento => tokenDocumento == token);
                    score += ocorrencias;
                }

                if (score > 0)
                    documentosComScore.Add((documento, score));
            }

            int limite = quantidadeMaximaDocumentos <= 0 ? 8 : quantidadeMaximaDocumentos;
            return documentosComScore
                .OrderByDescending(x => x.Score)
                .Take(limite)
                .Select(x => x.Documento)
                .ToList();
        }

        private static string LimitarTamanhoTexto(string texto, int quantidadeMaximaCaracteres)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            int limite = quantidadeMaximaCaracteres <= 0 ? 1200 : quantidadeMaximaCaracteres;
            if (texto.Length <= limite)
                return texto;

            return texto[..limite] + "...";
        }

        private static List<string> Tokenizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return new List<string>();

            string textoNormalizado = texto.ToLowerInvariant();

            HashSet<string> ignorarPalavras = new HashSet<string>
            {
                "a", "o", "os", "as",
                "um", "uma", "uns", "umas",
                "de", "do", "da", "dos", "das",
                "em", "no", "na", "nos", "nas",
                "para", "por", "com", "sem",
                "e", "ou", "mas",
                "que", "se", "sua", "seu", "suas", "seus",
                "ao", "aos", "as", "sobre", "entre", "ate", "apos",
                "como", "quando", "onde",
                "ja", "nao", "sim"
            };

            return Regex.Split(textoNormalizado, @"\W+")
                .Where(w => w.Length > 1 && !ignorarPalavras.Contains(w))
                .Distinct()
                .ToList();
        }
    }
}
