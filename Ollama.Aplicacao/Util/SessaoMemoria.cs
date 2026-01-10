using System.Collections.Concurrent;

namespace Ollama.Aplicacao.Util
{
    public class SessaoMemoria
    {
        private readonly ConcurrentDictionary<string, string> _sessoes = new();

        public void Gravar(string nomeSessao, string texto) 
        { 
            _sessoes[nomeSessao] = texto; 
        }
        public string? Obter(string nomeSessao) 
        { 
            _sessoes.TryGetValue(nomeSessao, out var texto); 
            return texto; 
        }
        public void Atualizar(string nomeSessao, string novoTexto) 
        { 
            _sessoes[nomeSessao] = novoTexto; 
        }
        public void Excluir(string nomeSessao) 
        { 
            _sessoes.TryRemove(nomeSessao, out _); 
        }
    }
}
