namespace CompositeWeb.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Weather()
        {

            return this.PartialView();
        }


        public IActionResult Counter()
        {

            return this.PartialView();
        }

        public IActionResult Error()
        {
            return this.View();
        }
    }
}
