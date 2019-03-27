using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WhackAMole.KubeServices;
using WhackAMole.KubeServices.Models;
using WhackAMole.KubeServices.Providers;

namespace WhackAMole.KubeAdmin.Controllers
{
    [Route("/")]
    public class HelloController : Controller
    {
        public HelloController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return new OkObjectResult("Hello world!");
        }
    }
}
