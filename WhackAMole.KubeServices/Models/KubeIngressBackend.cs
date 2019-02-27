using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WhackAMole.KubeServices.Models
{
    
    public class KubeIngressBackend
    {
     
        [JsonProperty("serviceName")]
        public string ServiceName { get; set; }
    
    }
}
