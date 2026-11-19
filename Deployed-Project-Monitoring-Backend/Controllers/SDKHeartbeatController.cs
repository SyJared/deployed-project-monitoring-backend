using DTOs;
using Microsoft.AspNetCore.Mvc;
using SDKInterface;

namespace Controllers; // <- use the same namespace as your existing controllers

[ApiController]
[Route("api/sdk/heartbeat")]
public class SdkHeartbeatController : ControllerBase
{
	private readonly IHeartbeatService _heartbeatService;

	public SdkHeartbeatController(IHeartbeatService heartbeatService)
	{
		_heartbeatService = heartbeatService;
	}

	[HttpPost]
	public async Task<IActionResult> Post(
		[FromHeader(Name = "X-API-Key")] string? apiKey,
		[FromBody] HeartbeatRequest request)
	{
		var accepted = await _heartbeatService.ReceiveHeartbeat(apiKey ?? string.Empty, request);

		// The SDK reads `message` from error responses.
		if (!accepted)
			return Unauthorized(new { message = "Invalid or missing API key." });

		return Ok(new { ok = true, server_time = DateTime.UtcNow });
	}
}