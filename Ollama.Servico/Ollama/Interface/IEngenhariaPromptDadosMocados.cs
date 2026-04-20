namespace Ollama.Servico.Ollama.Interface
{
    public interface IEngenhariaPromptDadosMocados
    {
        public string PromptOrdemServico(string manutentor);
        public string PromptOrdemServicoHtml(string manutentor);
    }
}
