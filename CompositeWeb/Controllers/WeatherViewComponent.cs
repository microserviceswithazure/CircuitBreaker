namespace CompositeWeb.Controllers
{
    using System;
    using System.Fabric;
    using System.Threading.Tasks;

    using Communication;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class WeatherViewComponent : ViewComponent
    {
        private static readonly Uri WeatherServiceUri =
            new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/WeatherService");

        private readonly IWeatherService weatherServiceClient =
            ServiceProxy.Create<IWeatherService>(WeatherServiceUri, new ServicePartitionKey("basic"));
    
        public async Task<IViewComponentResult> InvokeAsync()
        {
           // var result =  await this.weatherServiceClient.GetReport("2010");
            return this.View(new WeatherReport());
        }


    }
}