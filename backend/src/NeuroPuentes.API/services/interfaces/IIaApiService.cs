using Microsoft.AspNetCore.Http;
using NeuroPuentesAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuroPuentesAPI.services
{
    public interface IIaApiService
    {
        Task<IAResponse> ProcessAudioAsync(IFormFile audioFile, string contextTraits, string? sessionId);
    }
}
