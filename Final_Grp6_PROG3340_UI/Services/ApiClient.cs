using System.Net.Http.Headers;

public class ApiClient
{
	private readonly HttpClient _client;
	private readonly IHttpContextAccessor _context;

	public ApiClient(HttpClient client, IHttpContextAccessor context)
	{
		_client = client;
		_context = context;
		_client.BaseAddress = new Uri("https://localhost:7111/api/");
	}

	private void InjectToken()
	{
		// Read JWT from cookie (correct for your AuthController)
		var token = _context.HttpContext?.Request.Cookies["AuthToken"];

		if (!string.IsNullOrEmpty(token))
		{
			_client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", token);
		}
		else
		{
			_client.DefaultRequestHeaders.Authorization = null;
		}
	}

	public async Task<HttpResponseMessage> PostAsync(string url, object data)
	{
		InjectToken();
		return await _client.PostAsJsonAsync(url, data);
	}

	public async Task<HttpResponseMessage> GetAsync(string url)
	{
		InjectToken();
		return await _client.GetAsync(url);
	}

	public async Task<HttpResponseMessage> PutAsync(string url, object data)
	{
		InjectToken();
		return await _client.PutAsJsonAsync(url, data);
	}

	public async Task<HttpResponseMessage> DeleteAsync(string url)
	{
		InjectToken();
		return await _client.DeleteAsync(url);
	}
}
