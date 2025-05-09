using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.SemanticKernel;

public class TmdbPlugin
{
    private readonly HttpClient _http;

    public TmdbPlugin(string baseUrl, string token, HttpClient? client = null)
    {
        _http = client ?? new HttpClient();
        _http.BaseAddress = new Uri(baseUrl);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _http.DefaultRequestHeaders.Add("accept", "application/json");
    }

    [KernelFunction, Description("Return the top‑rated TMDB movies as a newline list.")]
    public async Task<string> GetTopRatedMoviesAsync(int take = 5)
    {
        var json = await _http.GetStringAsync("/3/movie/top_rated?language=en-US&page=1");
        using var doc = JsonDocument.Parse(json);
        var lines = doc.RootElement.GetProperty("results")
            .EnumerateArray().Take(Math.Clamp(take, 1, 20))
            .Select(m => $"- {m.GetProperty("title").GetString()} (" +
                         $"{m.GetProperty("release_date").GetString()?.Split('-')[0]}) " +
                         $"{m.GetProperty("vote_average").GetDecimal():0.0}");
        return string.Join("\n", lines);
    }

    [KernelFunction, Description("Free‑text movie search. Returns up to 10 matches.")]
    public async Task<string> SearchMoviesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return string.Empty;
        var url  = $"/3/search/movie?query={Uri.EscapeDataString(query)}&include_adult=false&language=en-US&page=1";
        var json = await _http.GetStringAsync(url);
        using var doc = JsonDocument.Parse(json);
        var lines = doc.RootElement.GetProperty("results")
            .EnumerateArray().Take(10)
            .Select(m => $"- {m.GetProperty("title").GetString()} (" +
                         $"{m.GetProperty("release_date").GetString()?.Split('-')[0]}) " +
                         $"{m.GetProperty("vote_average").GetDecimal():0.0}");
        return string.Join("\n", lines);
    }
    
    [KernelFunction, Description("Return TMDB movieId for the given title (first search result).")]
    public async Task<string> SearchMovieIdAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return "0";
        var url = $"/3/search/movie?query={Uri.EscapeDataString(query)}&include_adult=false&language=en-US&page=1";
        var json = await _http.GetStringAsync(url);
        using var doc = JsonDocument.Parse(json);
        var first = doc.RootElement.GetProperty("results").EnumerateArray().FirstOrDefault();
        return first.ValueKind == JsonValueKind.Object ? first.GetProperty("id").GetInt32().ToString() : "0";
    }
    
    [KernelFunction, Description("Get recommended movies for a TMDB movieId.")]
    public async Task<string> GetMovieRecommendationsAsync(int movieId, int take = 10)
    {
        if (movieId == 0) return string.Empty;
        var json = await _http.GetStringAsync($"/3/movie/{movieId}/recommendations?language=en-US&page=1");
        using var doc = JsonDocument.Parse(json);
        var lines = doc.RootElement.GetProperty("results")
            .EnumerateArray().Take(Math.Clamp(take, 1, 20))
            .Select(m => $"- {m.GetProperty("title").GetString()} (" +
                         $"{m.GetProperty("release_date").GetString()?.Split('-')[0]}) " +
                         $"{m.GetProperty("vote_average").GetDecimal():0.0}");
        return string.Join("\n", lines);
    }
}
