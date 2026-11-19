using DTOs;

namespace SDKInterface;

public interface IHeartbeatService
{
    /// <summary>Returns false when the API key is missing or invalid.</summary>
    Task<bool> ReceiveHeartbeat(string apiKey, HeartbeatRequest request);

    Task<HeartbeatStatusResponse> GetHeartbeatStatus(int projectId);
}