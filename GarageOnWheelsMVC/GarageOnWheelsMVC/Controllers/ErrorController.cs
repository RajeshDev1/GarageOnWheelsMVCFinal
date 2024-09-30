using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GarageOnWheelsMVC.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/AccessDenied")]
        public IActionResult AccessDenied()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return View();
        }
    }

}
