using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    using Cache.Redis;

    interface ICircuitBreakerStateStore
    {
        CircuitBreakerStateEnum State { get; }

        Exception LastException { get; }

        DateTime LastStateChangedDateUtc { get; }

        void Trip(Exception ex);

        void Reset();

        void HalfOpen();

        bool IsClosed { get; }
    }

    public enum CircuitBreakerStateEnum
    {
        Open,
        HalfOpen,
        Closed
    }

    public class CircuitBreaker
    {
        private readonly ICircuitBreakerStateStore stateStore =
          CircuitBreakerStateStoreFactory.GetCircuitBreakerStateStore();

        private readonly object halfOpenSyncObject = new object();
        public bool IsClosed { get { return stateStore.IsClosed; } }

        public bool IsOpen { get { return !IsClosed; } }

        public void ExecuteAction(Action action)
        {

            if (IsOpen)
            {

            }

            // The circuit breaker is Closed, execute the action.
            try
            {
                action();
            }
            catch (Exception ex)
            {
                // If an exception still occurs here, simply 
                // re-trip the breaker immediately.
                this.TrackException(ex);

                // Throw the exception so that the caller can tell
                // the type of exception that was thrown.
                throw;
            }
        }

        private void TrackException(Exception ex)
        {
            // For simplicity in this example, open the circuit breaker on the first exception.
            // In reality this would be more complex. A certain type of exception, such as one
            // that indicates a service is offline, might trip the circuit breaker immediately. 
            // Alternatively it may count exceptions locally or across multiple instances and
            // use this value over time, or the exception/success ratio based on the exception
            // types, to open the circuit breaker.
            this.stateStore.Trip(ex);
        }
    }

    internal class CircuitBreakerStateStoreFactory
    {
        private CacheProxy cacheProxy;
        public CircuitBreakerStateStoreFactory(string connectionstring)
        {
            this.cacheProxy = new CacheProxy(connectionstring);
        }

        public ICircuitBreakerStateStore GetCircuitBreakerStateStore()
        {
            return this.cacheProxy.Get<ICircuitBreakerStateStore>("circuitbreakerstate");
        }
    }
}
