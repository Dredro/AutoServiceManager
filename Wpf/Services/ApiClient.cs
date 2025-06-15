using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Wpf.Services;

public class ApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://localhost:5214/api"; 

    public string? CurrentJwtToken { get; private set; }

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void SetJwtToken(string? token)
    {
        CurrentJwtToken = token;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
    
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
        Console.WriteLine($"Error GET {endpoint}: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        return default;
    }

    public async Task<(TResponse? Result, string? ErrorMessage)> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{endpoint}", data);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            return (result, null);
        }
        
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error POST {endpoint}: {response.StatusCode} - {errorContent}");
        if (response.Content.Headers.ContentType?.MediaType == "application/problem+json")
        {
             try 
             {
                var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (problemDetails != null && problemDetails.Errors.Any())
                {
                    var sb = new StringBuilder();
                    foreach(var err in problemDetails.Errors)
                    {
                        sb.AppendLine($"{err.Key}: {string.Join(", ", err.Value)}");
                    }
                    return (default, sb.ToString());
                }
             } catch {} 
        }
        return (default, errorContent);
    }
   
    public async Task<TResponse?> PostAsync<TResponse>(string endpoint, object data)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{endpoint}", data);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        Console.WriteLine($"Error POST {endpoint}: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        return default;
    }
    public async Task<(TResponse? Result, string? ErrorMessage)> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var requestUri = $"{_baseUrl}/{endpoint}";

        // Create JsonContent from the request data
        var content = JsonContent.Create(data);

        // Create an HttpRequestMessage specifically for the PATCH method
        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri);
        request.Content = content;

        // Send the request
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            // PATCH success can return 200 OK (with content) or 204 No Content.
            if (response.Content.Headers.ContentLength > 0)
            {
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<TResponse>();
                    return (result, null); // Success with result
                }
                 catch (JsonException ex)
                {
                    // Handle cases where success status is returned but content is not valid TResponse JSON
                    Console.WriteLine($"Error deserializing PATCH {endpoint} success response: {ex.Message}");
                    return (default, $"Failed to deserialize success response: {ex.Message}");
                }
            }
            else
            {
                // Success with No Content (e.g., 204)
                return (default, null); // Success, but no result body
            }
        }
        else // Not successful status code
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error PATCH {endpoint}: {response.StatusCode} - {errorContent}");

            // Attempt to parse ValidationProblemDetails, similar to PostAsync
            if (response.Content.Headers.ContentType?.MediaType == "application/problem+json")
            {
                 try
                 {
                    var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (problemDetails != null && problemDetails.Errors.Any())
                    {
                        var sb = new StringBuilder();
                        foreach(var err in problemDetails.Errors)
                        {
                            sb.AppendLine($"{err.Key}: {string.Join(", ", err.Value)}");
                        }
                        // Return default TResponse and the formatted error message
                        return (default, sb.ToString());
                    }
                 }
                 catch (JsonException ex)
                 {
                     // Log deserialization error but return the raw error content
                     Console.WriteLine($"Error deserializing ValidationProblemDetails for PATCH {endpoint}: {ex.Message}");
                     // Fall through to returning raw errorContent
                 }
                 catch (Exception ex) // Catch other potential exceptions during processing problemDetails
                 {
                      Console.WriteLine($"Unexpected error processing ValidationProblemDetails for PATCH {endpoint}: {ex.Message}");
                     // Fall through to returning raw errorContent
                 }
            }
            // If not problem+json or deserialization failed, return default TResponse and the raw error content
            return (default, errorContent);
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> PutAsync<TRequest>(string endpoint, TRequest data)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{endpoint}", data);
        if (response.IsSuccessStatusCode)
        {
            return (true, null);
        }
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error PUT {endpoint}: {response.StatusCode} - {errorContent}");
        return (false, errorContent);
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
         if (response.IsSuccessStatusCode)
        {
            return (true, null);
        }
        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error DELETE {endpoint}: {response.StatusCode} - {errorContent}");
        return (false, errorContent);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
    
    private class ValidationProblemDetails
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}