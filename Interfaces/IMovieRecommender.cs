namespace AgentsDemo.Interfaces;
public interface IMovieRecommender
{
    Task<IReadOnlyList<string>> RecommendAsync(int top = 20);
}