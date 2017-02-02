namespace WeatherService
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    using Communication;

    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Newtonsoft.Json;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class WeatherService : StatefulService, IWeatherService
    {
        private CircuitBreaker circuitBreaker;

        public WeatherService(StatefulServiceContext context)
            : base(context)
        {
            this.circuitBreaker = new CircuitBreaker(this.StateManager, 30000);
        }

        public async Task<WeatherReport> GetReport(string postCode)
        {
            var random = new Random().Next(25, 35);
            var counterState = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(
                "weatherState");
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            token.ThrowIfCancellationRequested();
            WeatherReport result = null;
            await this.circuitBreaker.Invoke(
                async () =>
                {
                    // mocking service result. randomly failing the service call.
                    var failureSeed = new Random().Next(1, 20);
                    if (failureSeed % 3 == 0)
                    {
                        throw new ApplicationException();
                    }
                    result = new WeatherReport
                    {
                        Temperature = random,
                        WeatherConditions = random < 30 ? "Cloudy" : "Sunny",
                        ReportTime = DateTime.UtcNow,
                        CircuitState = "Open"
                    };
                    using (var tx = this.StateManager.CreateTransaction())
                    {
                        await counterState.AddOrUpdateAsync(
                            tx,
                            "savedWeather",
                            key => JsonConvert.SerializeObject(result),
                            (key, val) => JsonConvert.SerializeObject(result));
                        await tx.CommitAsync();
                    }
                },
                async () =>
                {
                    using (var tx = this.StateManager.CreateTransaction())
                    {
                        // service faulted. read old value and populate.
                        var value = await counterState.TryGetValueAsync(tx, "savedWeather");
                        if (value.HasValue)
                        {
                            result = JsonConvert.DeserializeObject<WeatherReport>(value.Value);
                        }
                        else
                        {
                            result = new WeatherReport { ReportTime = DateTime.UtcNow, Temperature = 0, WeatherConditions = "Unknown" };
                            await counterState.AddOrUpdateAsync(
                                tx,
                                "savedWeather",
                                key => JsonConvert.SerializeObject(result),
                                (key, val) => JsonConvert.SerializeObject(result));
                            await tx.CommitAsync();
                        }

                        result.CircuitState = "Closed";
                    }
                });

            return result;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener) };
        }
    }
}