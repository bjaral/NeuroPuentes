using NeuroPuentesAPI.DTOs; 
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NeuroPuentesAPI.services 
{
    public class IaApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IaApiService> _logger;
        private readonly string _processAudioEndpoint = "/process_audio";

        public IaApiService(IHttpClientFactory httpClientFactory, ILogger<IaApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IAResponse> ProcessAudioAsync(IFormFile audioFile, string contextTraits, string? sessionId)
        {
            var client = _httpClientFactory.CreateClient("IaApiClient");

            try
            {
                using var formData = new MultipartFormDataContent();
                using var audioStream = audioFile.OpenReadStream();
                using var streamContent = new StreamContent(audioStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(audioFile.ContentType ?? "audio/mpeg");

                formData.Add(streamContent, "audio", audioFile.FileName);
                formData.Add(new StringContent(contextTraits ?? ""), "context_traits");
                formData.Add(new StringContent(sessionId ?? ""), "session_id");
                
                var response = await client.PostAsync(_processAudioEndpoint, formData);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error de la API de IA (HTTP {StatusCode}): {Response}", response.StatusCode, jsonResponse);
                    return new IAResponse { Success = false, Error = $"Error de la API de IA (HTTP {response.StatusCode}): {jsonResponse}" };
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var iaResponse = JsonSerializer.Deserialize<IAResponse>(jsonResponse, options);
                
                return iaResponse ?? new IAResponse { Success = false, Error = "No se pudo deserializar la respuesta de la IA." };
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "No se pudo conectar a la API de Python en {BaseAddress}", client.BaseAddress);
                return new IAResponse { Success = false, Error = $"Error de conexión: No se pudo alcanzar el servidor de IA. {e.Message}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al procesar el audio.");
                return new IAResponse { Success = false, Error = $"Error inesperado: {ex.Message}" };
            }
        }
    }
}