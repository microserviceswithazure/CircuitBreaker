namespace CompositeWeb.Utils
{
    using System;

    using Polly;
    using Polly.CircuitBreaker;

    public class CircuitBreaker
    {
        #region Constructors and Destructors
        CircuitBreakerPolicy breaker;

        public CircuitBreaker()
        {
            breaker = Policy.Handle<Exception>()
                .CircuitBreaker(exceptionsAllowedBeforeBreaking: 2, durationOfBreak: TimeSpan.FromMinutes(1));
        }

        public T Execute<T>(Func<T> functionToExecute)
        {
            this.breaker.Execute()
        }

        #endregion
    }
}