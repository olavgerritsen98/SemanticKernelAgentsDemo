# Movie-Agent &nbsp;🎬🤖

> A function-calling LLM agent that remembers what you’ve watched and recommends what to watch next.

---

## ✨ Features

| | Capability | Behind the scenes |
|-|------------|-------------------|
| 🌟 | **Top-rated list** | `tmdb.GetTopRatedMoviesAsync` |
| 🔎 | **Free-text search** | `tmdb.SearchMoviesAsync` |
| ✅ | **Mark as watched** | `memory.AddWatchedAsync` |
| 🗑️ | **Remove watched title** | `memory.RemoveWatchedAsync` |
| 📜 | **List everything watched** | `memory.ListWatchedAsync` |
| 🤝 | **Similar-movie recommendations** | `tmdb.SearchMovieIdAsync` → `tmdb.GetMovieRecommendationsAsync` |
| 🎯 | **Personalised top-picks** (excludes watched) | `reco.RecommendMoviesAsync` |
| 🪄 | **Stepwise planning** | OpenAI Function-Calling Planner chains calls automatically |

---

## 📋 Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/)
* Azure OpenAI Resource
* **TMDB API key** – set `TMDB_API_KEY` (get one at [themoviedb.org](https://www.themoviedb.org/))

---

## 🚀 Quick-start

```bash
# clone & restore packages
git clone https://github.com/olavgerritsen98/SemanticKernelAgents.git

# configure secrets
Go to kernelConfig.json
Add the TMDB api key

# run the console chat
dotnet run
