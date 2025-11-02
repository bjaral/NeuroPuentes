using System.Text.Json.Serialization;

namespace NeuroPuentesAPI.DTOs 
{
    public class IAResponse
    {
        [JsonPropertyName("transcription")]
        public string Transcription { get; set; } = string.Empty; 

        [JsonPropertyName("response_text")]
        public string ResponseText { get; set; } = string.Empty; 

        [JsonPropertyName("audio_base64")]
        public string AudioBase64 { get; set; } = string.Empty; 

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = string.Empty; 

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; } 
    }
}