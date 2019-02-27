using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WhackAMole.KubeServices;
using WhackAMole.KubeServices.Models;
using WhackAMole.KubeServices.Providers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhackAMole.KubeAdmin.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class IngressController : Controller
    {
        private readonly IAuthenticationProvider _auth;
        private readonly IngressRequest _ingressRequest;

        public IngressController(IAuthenticationProvider authProvider, IOptions<KubeOptions> options)
        {
            _auth = authProvider;
            var k8s = new KubeRequestBuilder(_auth, options.Value);
            _ingressRequest = k8s.Create<IngressRequest>();
        }

        // GET: api/ingress
        [HttpGet()]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var list = await _ingressRequest.GetAllAsync();

                if (list == null || list.Length == 0)
                {
                    return new NotFoundObjectResult(null);
                }

                var nodes = new List<Ingress>();

                foreach (var node in list) {
                    nodes.Add(new Ingress { BackendServiceName = node.Spec.Backend.ServiceName, Uid = node.MetaData.Uid });
                }

                return new OkObjectResult(nodes);
            }
            catch (Exception)
            {
                return new NotFoundResult();
            }
        }
    }
}
