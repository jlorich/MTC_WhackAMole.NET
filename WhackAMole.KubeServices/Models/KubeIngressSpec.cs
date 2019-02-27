using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WhackAMole.KubeServices.Models
{
    
    public class KubeIngressSpec
    {
     
        [JsonProperty("backend")]
        public KubeIngressBackend Backend { get; set; }
    
    }
}
