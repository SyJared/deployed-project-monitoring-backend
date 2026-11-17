
using System.Security.Cryptography;
using System.Text;

namespace Security;

public static class ApiKeyGenerator
{
	public static string Generate()
	{
		var randomBytes = RandomNumberGenerator.GetBytes(32);

		var token = Convert.ToBase64String(randomBytes)
			.Replace("+", "-")
			.Replace("/", "_")
			.TrimEnd('=');

		return $"dpm_live_{token}";
	}

	public static string Hash(string apiKey)
	{
		var bytes = Encoding.UTF8.GetBytes(apiKey);
		var hash = SHA256.HashData(bytes);

		return Convert.ToHexString(hash);
	}
}
