using AgentsDemo.Interfaces;

namespace AgentsDemo;

public class InMemoryMovieMemory : IMovieMemory
{
    private readonly HashSet<string> _watched = new(StringComparer.OrdinalIgnoreCase);

    public Task<bool> AddAsync(string title)    => Task.FromResult(_watched.Add(title));
    public Task<bool> RemoveAsync(string title) => Task.FromResult(_watched.Remove(title));
    public Task<IReadOnlyCollection<string>> AllAsync() => Task.FromResult((IReadOnlyCollection<string>)_watched.ToList());
    public bool Contains(string title) => _watched.Contains(title);
}