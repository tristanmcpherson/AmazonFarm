using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AmazonFarm
{
    class Program
    {
        static async Task Main(string[] args) {
            var configFile = await File.ReadAllTextAsync("Config.json");
            var config = JsonConvert.DeserializeObject<Config>(configFile);

            // Browser per asin
            var cancellationToken = new CancellationTokenSource();
            var tasks = new List<Task>();

            config.AsinGroups.SelectMany(asin => asin.Asins.Select(
                a => new AsinGroup {
                    Asins = new List<string> { a },
                    MinPrice = asin.MinPrice,
                    MaxPrice = asin.MaxPrice
                })).ToList().ForEach(
                    asin => {
                        tasks.Add(Task.Run(() => {
                            var selenium = new Selenium(config, asin);


                            selenium.Start(cancellationToken.Token);

                            if (!cancellationToken.IsCancellationRequested) {
                                cancellationToken.Cancel();
                            }

                            selenium.Stop();
                        }));
                    });

            Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Shutting down chrome instances.");
            Console.ForegroundColor = ConsoleColor.White;

            cancellationToken.Cancel();
            await Task.WhenAll(tasks);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Shutdown complete.");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ReadLine();
        }
    }
}
