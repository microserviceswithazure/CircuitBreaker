namespace CompositeWeb.Controllers
{
    using System;
    using System.Fabric;
    using System.Threading.Tasks;

    using Communication;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class CounterViewComponent : ViewComponent
    {
        private static readonly Uri CounterServiceUri =
   new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/CounterService");

        private readonly ICounterService counterServiceClient =
            ServiceProxy.Create<ICounterService>(CounterServiceUri, new ServicePartitionKey("basic"));

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result =  await this.counterServiceClient.GetValue();
            return this.View(result);
        }


    }
}