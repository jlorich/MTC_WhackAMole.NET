using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhackAMole.KubeServices.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhackAMole.MoleCloud.Controllers
{
    [Route("api/[controller]")]
    public class MoleController : Controller
    {
        private const int START = 65;
        private const int END = 91;
        private int _count = START;
        private string _defaultColor = "FF691E"; // Change this and queue a new build manually to update the color for a specific MTC

        [HttpGet]
        public IActionResult Get()
        {
            var pod = Environment.GetEnvironmentVariable("POD_NAME");
            var host = Environment.GetEnvironmentVariable("NODE_NAME");
            var color = Environment.GetEnvironmentVariable("POD_COLOR");

            if (String.IsNullOrEmpty(color))
            {
                color = _defaultColor;
            }

            var rnd = new Random();
            var c = (char)rnd.Next(START, END);
            var character = c.ToString();
            var mole = new MoleState() { Name = $"{pod}", CurrentChar = character, Color = color };
            
            return new OkObjectResult(mole);
        }
    }
}
