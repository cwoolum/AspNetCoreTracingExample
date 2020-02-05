using Serilog;
using Serilog.Events;

using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tracing.Console
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            System.Console.WriteLine("Hello World!");

            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
              .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
              .Enrich.FromLogContext()
              .WriteTo.Console()
              .CreateLogger();

            HttpClient client = new HttpClient();
            int i = 0;

            while (true)
            {
                var activity = new Activity("Test Run")
                    .AddBaggage("RunNumber", i++.ToString())
                    .Start();

                Thread.Sleep(10000);
                var request = await client.GetAsync("https://localhost:5001/WeatherForecast");
                Log.Logger.Information("TraceId: " + activity.TraceId);

                Log.Logger.Information("Request Headers");
                foreach (var header in request.RequestMessage.Headers)
                {
                    Log.Logger.Information(header.Key + ":" + string.Join(';', header.Value));
                }

                activity.Stop();
            }
        }
    }
}