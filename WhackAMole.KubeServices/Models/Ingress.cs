using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhackAMole.KubeServices.Models
{
    public class Ingress
    {
        public string BackendServiceName { get; set; }

        public string Uid { get; set; }
    }
}
