using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TvMazeScraper.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TvMazeScraper.Domain.ApiModel;

namespace TvMazeScraper.API.RecurrentTask
{
    public class BackgroundHostedService : IHostedService, IDisposable
    {
        private Task executingTask;
        private readonly CancellationTokenSource stoppingCts = new CancellationTokenSource();
        private const string baseAddress = "http://api.tvmaze.com/shows";
        private int page = 0;

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var shows = new List<ShowDto>();

                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await httpClient.GetAsync($"{baseAddress}?page={page}");
                        string jsonResult = await response.Content.ReadAsStringAsync();

                        shows.AddRange(JsonConvert.DeserializeObject<ShowDto[]>(jsonResult));

                        if (shows.Count == 0)
                        {
                            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                            continue;
                        }
                        page++;

                        foreach (var show in shows)
                        {
                            var responseCast = await httpClient.GetAsync($"{baseAddress}/{show.Id}/cast");
                            string jsonCastResult = await responseCast.Content.ReadAsStringAsync();

                            CreateCasts(show, jsonCastResult);
                        }

                        var content = new StringContent(JsonConvert.SerializeObject(shows), UnicodeEncoding.UTF8, "application/json");
                        await httpClient.PostAsync("https://localhost:5001/api/tvshows", content);
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private static void CreateCasts(ShowDto show, string jsonCastResult)
        {
            try
            {
                var castsObject = JsonConvert.DeserializeObject<CastApi[]>(jsonCastResult);
                var casts = castsObject.Select(o => new CastDto
                {
                    Id = o.Person.Id,
                    Name = o.Person?.Name,
                    Birthday = Convert.ToDateTime(o.Person?.Birthday)

                }).ToList();
                show.CastsDto = casts;
            }
            catch (Exception)
            {
                //if error to deserialize do no stop, just implement a log here
                return;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            executingTask = ExecuteAsync(stoppingCts.Token);

            if (executingTask.IsCompleted)
            {
                return executingTask;
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (executingTask == null)
            {
                return;
            }

            try
            {
                stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            stoppingCts.Cancel();
        }
    }
}
