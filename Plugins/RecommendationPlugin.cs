using System.ComponentModel;
using AgentsDemo.Interfaces;
using Microsoft.SemanticKernel;

namespace AgentsDemo.Plugins;

public class RecommendationPlugin
{
    private readonly IMovieRecommender _reco;
    public RecommendationPlugin(IMovieRecommender reco) => _reco = reco;

    [KernelFunction, Description("Suggest up to five top‑rated movies the user has not watched.")]
    public async Task<string> RecommendMoviesAsync(int top = 20)
    {
        var list = await _reco.RecommendAsync(top);
        return list.Any()
            ? string.Join("\n", list)
            : "I don't have new suggestions – you've watched everything in the top list!";
    }
}