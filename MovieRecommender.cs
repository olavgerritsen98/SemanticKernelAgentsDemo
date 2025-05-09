using AgentsDemo.Interfaces;
using Microsoft.SemanticKernel;

namespace AgentsDemo;

public class MovieRecommender : IMovieRecommender
{
    private readonly Kernel _kernel;
    private readonly IMovieMemory _memory;

    public MovieRecommender(Kernel kernel, IMovieMemory memory)
    {
        _kernel = kernel;
        _memory = memory;
    }
    
    public async Task<IReadOnlyList<string>> RecommendAsync(int top = 20)
    {
        var watched = await _memory.AllAsync();
        if (watched.Count == 0)
        {
            string list = await _kernel.InvokeAsync<string>("tmdb", "GetTopRatedMoviesAsync",
                new KernelArguments { ["take"] = top });
            return list.Split('\n', StringSplitOptions.RemoveEmptyEntries).Where(l => !string.IsNullOrWhiteSpace(l)).Take(5).ToList();
        }

        var ids = new List<int>();
        foreach (var title in watched)
        {
            string idStr = await _kernel.InvokeAsync<string>("tmdb", "SearchMovieId",
                new KernelArguments { ["query"] = title });
            if (int.TryParse(idStr, out int id) && id != 0) ids.Add(id);
        }

        if (ids.Count == 0)
        {
            return new List<string>();
        }

        var suggestions = new List<string>();
        foreach (var id in ids)
        {
            string recs = await _kernel.InvokeAsync<string>("tmdb", "GetMovieRecommendations",
                new KernelArguments { ["movieId"] = id, ["take"] = 10 });
            suggestions.AddRange(recs.Split('\n', StringSplitOptions.RemoveEmptyEntries).Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        var unseen = suggestions.Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(s => !_memory.Contains(TitleOnly(s)))
            .Select(s => (Line: s, Score: ParseScore(s)))
            .OrderByDescending(t => t.Score)
            .Select(t => t.Line)
            .Take(5)
            .ToList();
        return unseen;

        static string TitleOnly(string line) => line.StartsWith("- ") ? line[2..].Split('(')[0].Trim() : line;

        static decimal ParseScore(string line)
        {
            var parts = line.Split(' ');
            decimal.TryParse(parts[^1], out var score);
            return score;
        }
    }
}