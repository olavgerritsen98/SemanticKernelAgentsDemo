using System.ComponentModel;
using AgentsDemo.Interfaces;
using Microsoft.SemanticKernel;

namespace AgentsDemo.Plugins;

public class MovieMemoryPlugin
{
    private readonly IMovieMemory _mem;
    public MovieMemoryPlugin(IMovieMemory mem) => _mem = mem;
    
    [KernelFunction, Description("Mark the specified movie as already watched by the user. Return confirmation text.")]
    public async Task<string> AddWatchedAsync(string title)
    {
        return await _mem.AddAsync(title)
            ? $"👍 Added '{title}' to your watched list."
            : $"ℹ️  '{title}' was already marked as watched.";
    }

    [KernelFunction, Description("Remove a title from the watched list.")]
    public async Task<string> RemoveWatchedAsync(string title)
    {
        return await _mem.RemoveAsync(title)
            ? $"🗑️  Removed '{title}' from watched list."
            : $"ℹ️  I didn't find '{title}' in your watched list.";
    }

    [KernelFunction, Description("Return all watched titles as a newline‑separated list.")]
    public async Task<string> ListWatchedAsync()
    {
        var all = await _mem.AllAsync();
        return all.Count == 0
            ? "You haven't marked any movies as watched yet."
            : string.Join("\n", all.OrderBy(t => t));
    }
}