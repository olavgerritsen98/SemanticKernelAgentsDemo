namespace AgentsDemo.Interfaces;
public interface IMovieMemory
{
    Task<bool> AddAsync(string title);
    Task<bool> RemoveAsync(string title);
    Task<IReadOnlyCollection<string>> AllAsync();
    bool Contains(string title);
}