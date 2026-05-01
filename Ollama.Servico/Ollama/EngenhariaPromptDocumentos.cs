using Ollama.Servico.Ollama.Dto;
using Ollama.Servico.Ollama.Interface;
using System.Text;
using System.Text.RegularExpressions;

namespace Ollama.Servico.Ollama
{
    public class EngenhariaPromptDocumentos : IEngenhariaPromptDocumentos
    {
 
        public Task<string> GerarPrompt( string pergunta, List<DocumentoContextoDto> documentos, CancellationToken cancellationToken)
        {
            List<string> trechosLocalizados = FiltroPalavraChave(pergunta, documentos);
            if (trechosLocalizados.Count == 0)
                return Task.FromResult(string.Empty);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Você é um assistente especializado em análise de documentos e respostas contextuais. \n " +
                "Responda exclusivamente com base nas informações fornecidas no CONTEXTO abaixo. \n" +
                "Não utilize conhecimento externo ou suposições.");
            sb.AppendLine();

            sb.AppendLine($"Pergunta: " +   pergunta );
            sb.AppendLine();

            sb.AppendLine("--- CONTEXTO RELEVANTE ---");
            foreach (string trecho in trechosLocalizados)
            {
                sb.AppendLine(trecho);
                sb.AppendLine();
            }
            sb.AppendLine("--- FIM DO CONTEXTO ---");
            sb.AppendLine();
            sb.AppendLine("Instrucao:");
            sb.AppendLine("- Inicialize a resposta contextualizando o assunto da sessao.");
            sb.AppendLine("- Utilize o conteudo da sessao como base principal para responder.");
            sb.AppendLine("- Seja direto, objetivo e preciso; evite respostas longas ou especulativas.");
            sb.AppendLine("- Caso a pergunta não tenha relação com o CONTEXTO, responda exatamente: Desculpe, não encontrei informações sobre isso na minha base de dados.");

            Console.WriteLine(sb);

            return Task.FromResult(sb.ToString());
        }



        private static List<string> FiltroPalavraChave(string pergunta, List<DocumentoContextoDto> documentos)
        {
            List<string> trechosCapturados = new List<string>();

            // Valida entrada
            if (string.IsNullOrWhiteSpace(pergunta) || documentos == null || documentos.Count == 0)
                return trechosCapturados; //Enumerable.Empty<DocumentoContextoDto>();

            //Tokenizar a pergunta para obter os tokens de busca
            List<string> listaTokensPergunta = Tokenizar(pergunta);
            if (listaTokensPergunta.Count == 0)
                return trechosCapturados; //Enumerable.Empty<DocumentoContextoDto>();

            // Para cada documento, contar quantos tokens da pergunta aparecem no título e texto
            List<(DocumentoContextoDto Documento, int QuantidadeTokens)> documentosFiltrados = new List<(DocumentoContextoDto, int)>();
            foreach (DocumentoContextoDto documento in documentos)
            {
                // Dividir o texto do documento em frases e verificar se alguma frase contém os tokens da pergunta
                string[] frases = documento.Texto.Split(new[] { '.', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                // Verificar se alguma frase contém os tokens da pergunta
                foreach (var frase in frases)
                {
                    int tokensEncontrados = 0;
                    // Verificar se a frase contém algum dos tokens da pergunta
                    foreach (var token in listaTokensPergunta)
                    {
                        // Verificar se a frase contém o token (ignorar maiúsculas/minúsculas)
                        if (frase.Contains(token, StringComparison.OrdinalIgnoreCase))
                        {
                            tokensEncontrados +=1; // Incrementar a contagem de tokens encontrados
                            break; // já achou um token nessa frase
                        }
                    }
                    if (tokensEncontrados > 0)
                    {
                        var quantidadeTokensFrase = frase.Count();
                        var quantidadeTokensPerunta = listaTokensPergunta.Count;
                        Console.WriteLine($"Frase: {frase.Trim()} - Tokens encontrados: {tokensEncontrados} - Tokens na frase: {quantidadeTokensFrase} - Tokens na pergunta: {quantidadeTokensPerunta}");
                        trechosCapturados.Add(frase.Trim());
                    }
                }

                //List<string> listaTokensDocumento = Tokenizar($"{documento.Titulo} {documento.Texto}");
                //int tokensEncontrados = 0;
                //foreach (string tokenPergunta in listaTokensPergunta)
                //{
                //    int ocorrencias = listaTokensDocumento.Count(tokenDocumento => tokenDocumento == tokenPergunta);
                //    tokensEncontrados += ocorrencias;
                //}

                //if (tokensEncontrados > 0)
                //    documentosFiltrados.Add((documento, tokensEncontrados));
            }

            //int limite = quantidadeMaximaDocumentos <= 0 ? 8 : quantidadeMaximaDocumentos;
            //return documentosFiltrados
            //    .OrderByDescending(x => x.QuantidadeTokens)
            //    .Take(limite)
            //    .Select(x => x.Documento)
            //    .ToList();

            return trechosCapturados;
        }

        //private static string LimitarTamanhoTexto(string texto, int quantidadeMaximaCaracteres)
        //{
        //    if (string.IsNullOrWhiteSpace(texto))
        //        return string.Empty;

        //    int limite = quantidadeMaximaCaracteres <= 0 ? 1200 : quantidadeMaximaCaracteres;
        //    if (texto.Length <= limite)
        //        return texto;

        //    return texto[..limite] + "...";
        //}

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
