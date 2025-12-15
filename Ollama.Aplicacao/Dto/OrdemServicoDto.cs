namespace Ollama.Aplicacao.Dto
{
    public class OrdemServicoDto
    {
        public string Codigo { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public StatusOrdemServico Status { get; set; }
        public int TempoEstimadoHoras { get; set; }
        public string Manutentor { get; set; } = string.Empty;
    }

    public enum StatusOrdemServico
    {
        Aberta,
        Agendada,
        EmExecucao,
        Parada,
        Finalizada
    }
}
