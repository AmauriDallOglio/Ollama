using Microsoft.AspNetCore.Http;
using Ollama.Aplicacao.Util;
using Ollama.Dominio.Entidade;
using Ollama.Dominio.InterfaceRepositorio;

namespace Ollama.Aplicacao.Rotas.DocumentoRota
{
    public class ImportarDocumentoHandler : IContratoBaseHandler<ImportarDocumentoRequest, ResultadoOperacao>
    {
        private readonly IDocumentoCommandRepositorio _documentoRepositorio;

        public ImportarDocumentoHandler(IDocumentoCommandRepositorio documentoRepositorio)
        {
            _documentoRepositorio = documentoRepositorio;
        }

        public async Task<ResultadoOperacao> Executar(ImportarDocumentoRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                IFormFile arquivo = request.Arquivo;

                if (arquivo == null || arquivo.Length == 0)
                    return ResultadoOperacao.GerarErro("Nenhum arquivo enviado.", 400);

                // Extrair texto do arquivo (exemplo simples para TXT)
                string texto;
                using (var reader = new StreamReader(arquivo.OpenReadStream()))
                {
                    texto = await reader.ReadToEndAsync();
                }

            
                var documento = Documento.Criar(
                    titulo: Path.GetFileNameWithoutExtension(arquivo.FileName),
                    texto: texto,
                    tipoArquivo: Path.GetExtension(arquivo.FileName).Trim('.'),
                    tamanhoArquivo: arquivo.Length
                );

                await _documentoRepositorio.IncluirAsync(documento, cancellationToken);

                var response = ImportarDocumentoResponse.Criar(documento);
                return ResultadoOperacao.GerarSucesso(response);
            }
            catch (Exception ex)
            {
                return ResultadoOperacao.GerarErro($"Erro interno: {ex.Message}", 500);
            }
        }
    }
}
