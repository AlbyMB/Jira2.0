using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Final_Grp6_PROG3340_UI.Services
{
	public class ApiClient
	{
		private readonly IHttpClientFactory _factory;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ILogger<ApiClient> _logger;
		private readonly JwtService _jwtService;

		public ApiClient(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, ILogger<ApiClient> logger, JwtService jwtService)
		{
			_factory = factory;
			_httpContextAccessor = httpContextAccessor;
			_logger = logger;
			_jwtService = jwtService;
		}

		private HttpClient CreateClient()
		{
			var client = _factory.CreateClient("API");

			// Log the base address for debugging
			_logger.LogInformation("API Client Base Address: {BaseAddress}", client.BaseAddress);

			var user = _httpContextAccessor.HttpContext?.User;
			if (user?.Identity?.IsAuthenticated == true)
			{
				try
				{
					var token = _jwtService.GenerateToken(user);
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
					_logger.LogDebug("JWT token added to request header");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to generate JWT for API call.");
				}
			}
			else
			{
				_logger.LogDebug("User not authenticated, no JWT token added");
			}
			return client;
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync<T>(string uri, T payload)
		{
			var client = CreateClient();
			var fullUrl = new Uri(client.BaseAddress!, uri).ToString();
			_logger.LogInformation("POST Request to: {FullUrl}", fullUrl);

			try
			{
				var resp = await client.PostAsJsonAsync(uri, payload);
				return await LogAndBufferAsync(resp, fullUrl, "POST");
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "HTTP Request Exception for POST {FullUrl}", fullUrl);
				throw;
			}
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> PutAsync<T>(string uri, T payload)
		{
			var client = CreateClient();
			var fullUrl = new Uri(client.BaseAddress!, uri).ToString();
			_logger.LogInformation("PUT Request to: {FullUrl}", fullUrl);

			try
			{
				var resp = await client.PutAsJsonAsync(uri, payload);
				return await LogAndBufferAsync(resp, fullUrl, "PUT");
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "HTTP Request Exception for PUT {FullUrl}", fullUrl);
				throw;
			}
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> PatchAsync<T>(string uri, T payload)
		{
			var client = CreateClient();
			var fullUrl = new Uri(client.BaseAddress!, uri).ToString();
			_logger.LogInformation("PATCH Request to: {FullUrl}", fullUrl);

			try
			{
				var req = new HttpRequestMessage(new HttpMethod("PATCH"), uri)
				{
					Content = JsonContent.Create(payload)
				};
				var resp = await client.SendAsync(req);
				return await LogAndBufferAsync(resp, fullUrl, "PATCH");
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "HTTP Request Exception for PATCH {FullUrl}", fullUrl);
				throw;
			}
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> GetAsync(string uri)
		{
			var client = CreateClient();
			var fullUrl = new Uri(client.BaseAddress!, uri).ToString();
			_logger.LogInformation("GET Request to: {FullUrl}", fullUrl);

			try
			{
				var resp = await client.GetAsync(uri);
				return await LogAndBufferAsync(resp, fullUrl, "GET");
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "HTTP Request Exception for GET {FullUrl}", fullUrl);
				throw;
			}
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> DeleteAsync(string uri)
		{
			var client = CreateClient();
			var fullUrl = new Uri(client.BaseAddress!, uri).ToString();
			_logger.LogInformation("DELETE Request to: {FullUrl}", fullUrl);

			try
			{
				var resp = await client.DeleteAsync(uri);
				return await LogAndBufferAsync(resp, fullUrl, "DELETE");
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "HTTP Request Exception for DELETE {FullUrl}", fullUrl);
				throw;
			}
		}

		private async System.Threading.Tasks.Task<HttpResponseMessage> LogAndBufferAsync(HttpResponseMessage response, string fullUrl, string method)
		{
			string body = string.Empty;
			try
			{
				body = response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error reading response body for {Method} {FullUrl}", method, fullUrl);
			}

			var statusCode = (int)response.StatusCode;

			if (response.IsSuccessStatusCode)
			{
				_logger.LogInformation("? API {Method} {FullUrl} => {StatusCode} {Reason}", method, fullUrl, statusCode, response.ReasonPhrase);
			}
			else
			{
				_logger.LogWarning("? API {Method} {FullUrl} => {StatusCode} {Reason}\nResponse Body: {Body}", method, fullUrl, statusCode, response.ReasonPhrase, body);
			}

			Debug.WriteLine($"API {method} {fullUrl} => {statusCode} {response.ReasonPhrase}\n{body}");
			Console.WriteLine($"API {method} {fullUrl} => {statusCode} {response.ReasonPhrase}");

			// Re-buffer content so callers can read it again
			if (response.Content != null)
			{
				var mediaType = response.Content.Headers.ContentType?.MediaType ?? "application/json";
				response.Content = new StringContent(body ?? string.Empty, System.Text.Encoding.UTF8, mediaType);
			}

			return response;
		}
	}
}
