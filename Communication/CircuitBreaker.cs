namespace Communication
{
    using System;
    using System.Threading;

    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    public class CircuitBreaker
    {
        private IReliableStateManager stateManager;

        private readonly int resetTimeoutInMilliseconds;

        private DateTime? errorTime;

        public CircuitBreaker(IReliableStateManager stateManager, int resetTimeoutInMilliseconds)
        {
            this.stateManager = stateManager;
            this.resetTimeoutInMilliseconds = resetTimeoutInMilliseconds;
            this.errorTime = null;
        }

        public async void Invoke(Action func, Action failAction)
        {
            var errorHistory = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, DateTime>>("errorHistory");
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            token.ThrowIfCancellationRequested();

            using (var tx = this.stateManager.CreateTransaction())
            {
                if (this.errorTime.HasValue)
                {
                    if ((DateTime.Now - this.errorTime.Value).TotalMilliseconds < this.resetTimeoutInMilliseconds)
                    {
                        failAction.Invoke();
                    }
                }
                try
                {
                    var result = await errorHistory.TryGetValueAsync(tx, "errorTime");
                    if (result.HasValue)
                    {
                        this.errorTime = result.Value;
                    }

                    func.Invoke();
                    this.errorTime = null;
                }
                catch (Exception)
                {
                    this.errorTime = DateTime.Now;
                    failAction.Invoke();
                }

                await tx.CommitAsync();
            }
        }
    }
}