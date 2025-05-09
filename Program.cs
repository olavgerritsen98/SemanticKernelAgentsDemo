using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Text.RegularExpressions;
using AgentsDemo.Interfaces;
using AgentsDemo.Plugins;
using Azure.Identity;
using Microsoft.SemanticKernel.Planning; 


namespace AgentsDemo;

public class Program
{
    [Experimental("SKEXP0060")]
    public static async Task Main()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("C:\\dev\\Latest\\kernelConfig.json", optional: false)
            .Build();

        var builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
            deploymentName: config["AzureOpenAI:DeploymentName"]!,
            endpoint      : config["AzureOpenAI:Endpoint"]!,
            credentials: new DefaultAzureCredential());

        builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Warning));
        builder.AddInMemoryVectorStore();                               // not used yet, future RAG
        builder.Services.AddSingleton<IMovieMemory, InMemoryMovieMemory>();

        builder.Plugins.AddFromObject(
            new TmdbPlugin(
                baseUrl: config["TMDB:BaseUrl"]!,
                token  : config["TMDB:ReadAccessToken"]!),
            "tmdb");

        Kernel kernel = builder.Build();

        var memory       = kernel.Services.GetRequiredService<IMovieMemory>();
        var recommender  = new MovieRecommender(kernel, memory);
        kernel.Plugins.AddFromObject(new MovieMemoryPlugin(memory),          "memory");
        kernel.Plugins.AddFromObject(new RecommendationPlugin(recommender),  "reco");

        var planner = new FunctionCallingStepwisePlanner();

        Console.WriteLine("Movie‑Agent (agentic).  Ask me anything about movies — type 'exit' to quit.\n");

        while (true)
        {
            Console.Write(">> ");
            string? userGoal = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userGoal)) continue;
            if (userGoal.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            try
            {
                var result = await planner.ExecuteAsync(kernel, userGoal);
                Console.WriteLine(result.FinalAnswer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {ex.Message}");
            }
        }
    }
}