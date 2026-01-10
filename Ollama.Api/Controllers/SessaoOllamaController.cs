using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using System.Text.Json;

namespace Ollama.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessaoOllamaController : ControllerBase
    {
        private readonly OllamaServico _OllamaServico;
        private readonly HelperConsoleColor _HelperConsoleColor;
        private readonly EngenhariaPromptServico _EngenhariaPromptServico;
        private readonly HttpClient _http;

        public SessaoOllamaController(IHttpClientFactory httpClientFactory, OllamaServico ollamaServico, EngenhariaPromptServico engenhariaPromptServico, HelperConsoleColor helperConsoleColor)
        {
            _OllamaServico = ollamaServico;
            _HelperConsoleColor = helperConsoleColor;
            _EngenhariaPromptServico = engenhariaPromptServico;

            _http = httpClientFactory.CreateClient("ollama");
        }



        [HttpGet("SessaoGravada")]
        public async Task<IActionResult> SessaoGravada()
        {
            var request = new
            {
                model = "sessao_amauri",
                prompt = "Retorne o conteúdo gravado desta sessão.",
                stream = false
            };

            var response = await _http.PostAsJsonAsync("http://localhost:11434/api/generate", request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var conteudo = json.GetProperty("response").GetString();

            return Ok(new { Sucesso = true, Sessao = "sessao_amauri", Conteudo = conteudo });
        }


    }
}
