namespace WeatherService
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading.Tasks;

    using Communication;

    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class WeatherService : StatefulService, IWeatherService
    {
        #region Fields

        private CircuitBreaker circuitBreaker;

        #endregion

        #region Constructors and Destructors

        public WeatherService(StatefulServiceContext context)
            : base(context)
        {
            this.circuitBreaker = new CircuitBreaker(this.StateManager, 1000);
        }

        #endregion

        #region Public Methods and Operators

        public async Task<WeatherReport> GetReport(string postCode)
        {
            return new WeatherReport();
        }

        #endregion

        #region Methods


        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(this.CreateServiceRemotingListener) };
        }

        #endregion
    }
}