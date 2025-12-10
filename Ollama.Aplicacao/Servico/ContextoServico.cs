using System.Text.RegularExpressions;

namespace Ollama.Aplicacao.Servico
{
    public class ContextoServico
    {
        private readonly List<DocumentoContexto> _documentos = new();

        public ContextoServico()
        {
            // Exemplo: inicializa alguns documentos. Em produção carregue de BD/arquivo.
            _documentos.Add(new DocumentoContexto("1", "História da API", "APIs são contratos entre sistemas..."));
            _documentos.Add(new DocumentoContexto("2", "O que é Ollama", "Ollama é um servidor de modelos LLM que permite..."));
            _documentos.Add(new DocumentoContexto("3", "Uso em C#", "Para usar Ollama em C#, faça HTTP POST para /api/generate..."));
            _documentos.Add(new DocumentoContexto("4", "Batman", "É um super herói da DC..."));
        }

        // Permite adicionar documentos dinamicamente
        public void Adicionar(DocumentoContexto doc)
        {
            _documentos.Add(doc);
        }

        // Busca simples por similaridade: pontua documentos pela frequência de termos (TF simples)
        public IEnumerable<DocumentoContexto> BuscarDocumentos(string assunto, int topK = 4)
        {
            if (string.IsNullOrWhiteSpace(assunto)) 
                return Enumerable.Empty<DocumentoContexto>();

            var termosQuebraToken = Tokenizar(assunto);

            var pontuacoes = _documentos.Select(doc =>
            {
                var tokensDoc = Tokenizar(doc.Texto + " " + doc.Titulo);
                // score: soma de ocorrências de cada termo
                int score = termosQuebraToken.Sum(t => tokensDoc.Count(td => td == t));
                return new { Doc = doc, Score = score };
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => x.Doc);

            // Se nenhum doc pontuou, devolve topK por fallback (ex.: títulos)
            if (!pontuacoes.Any())
                return _documentos.Take(topK);

            return pontuacoes;
        }

        private static List<string> Tokenizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return new();
            texto = texto.ToLowerInvariant();
            // remove pontuação e divide por espaço
            var words = Regex.Split(texto, @"\W+").Where(w => w.Length > 0).ToList();
            return words;
        }

        public record DocumentoContexto(
            string Id, 
            string Titulo, 
            string Texto
            );
        public record PerguntaRequest(
            string Assunto, 
            int TopK = 4
            );
        public record OllamaResponseDto(
            string Resposta, 
            long TempoMs
            );
    }
}
