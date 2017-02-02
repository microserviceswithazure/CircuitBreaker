namespace CompositeWeb.Controllers
{
    using System;
    using System.Fabric;

    using Communication;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class HomeController : Controller
    {



        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Error()
        {
            return this.View();
        }
    }
}
