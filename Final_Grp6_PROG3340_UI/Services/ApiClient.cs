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

			var user = _httpContextAccessor.HttpContext?.User;
			if (user?.Identity?.IsAuthenticated == true)
			{
				try
				{
					var token = _jwtService.GenerateToken(user);
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to generate JWT for API call.");
				}
			}
			return client;
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync<T>(string uri, T payload)
		{
			var client = CreateClient();
			var resp = await client.PostAsJsonAsync(uri, payload);
			return await LogAndBufferAsync(resp, uri, "POST");
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> PutAsync<T>(string uri, T payload)
		{
			var client = CreateClient();
			var resp = await client.PutAsJsonAsync(uri, payload);
			return await LogAndBufferAsync(resp, uri, "PUT");
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> PatchAsync<T>(string uri, T payload)
		{
			var client = CreateClient();
			var req = new HttpRequestMessage(new HttpMethod("PATCH"), uri)
			{
				Content = JsonContent.Create(payload)
			};
			var resp = await client.SendAsync(req);
			return await LogAndBufferAsync(resp, uri, "PATCH");
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> GetAsync(string uri)
		{
			var client = CreateClient();
			var resp = await client.GetAsync(uri);
			return await LogAndBufferAsync(resp, uri, "GET");
		}

		public async System.Threading.Tasks.Task<HttpResponseMessage> DeleteAsync(string uri)
		{
			var client = CreateClient();
			var resp = await client.DeleteAsync(uri);
			return await LogAndBufferAsync(resp, uri, "DELETE");
		}

		private async System.Threading.Tasks.Task<HttpResponseMessage> LogAndBufferAsync(HttpResponseMessage response, string uri, string method)
		{
			string body = string.Empty;
			try
			{
				body = response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error reading response body for {Method} {Uri}", method, uri);
			}

			_logger.LogInformation("API {Method} {Uri} => {(Status)} {Reason}\n{Body}", method, uri, (int)response.StatusCode, response.ReasonPhrase, body);
			Debug.WriteLine($"API {method} {uri} => {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
			Console.WriteLine($"API {method} {uri} => {(int)response.StatusCode} {response.ReasonPhrase}");

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
