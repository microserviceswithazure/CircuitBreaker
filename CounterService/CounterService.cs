namespace CounterService
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

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CounterService : StatefulService, ICounterService
    {
        readonly CircuitBreaker circuitBreaker;

        public CounterService(StatefulServiceContext context)
            : base(context)
        {
            this.circuitBreaker = new CircuitBreaker(this.StateManager, 1000);
        }

        public async Task<CounterResult> GetValue()
        {
            var random = new Random().Next(1, 1000);
            var counterState = await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("counterState");
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            token.ThrowIfCancellationRequested();
            using (var tx = StateManager.CreateTransaction())
            {
                var savedCount = await counterState.TryGetValueAsync(tx, "savedCount");

            }

            CounterResult result = null;
            this.circuitBreaker.Invoke(
                () =>
                    {
                        // mocking service result.
                        var failureSeed = new Random().Next(1, 20);
                        if (failureSeed % 3 == 0)
                        {
                            throw new ApplicationException();
                        }

                        result = new CounterResult
                        {
                            Value = random,
                            ReportTime = DateTime.UtcNow,
                            CircuitState = "Open"
                        };
                    },
                () =>
                    {

                        result = new CounterResult { Value = 10, CircuitState = "Closed" };
                    });

            return result;
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener) };
        }
    }
}