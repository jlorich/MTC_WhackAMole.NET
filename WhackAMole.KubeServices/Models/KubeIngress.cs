using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.KubeServices.Interfaces;

namespace WhackAMole.KubeServices.Models
{
   

    public class KubeIngress : IKubeResource
    {
        public KubeMetaData MetaData { get; set; }
        public KubeIngressSpec Spec { get; set; }
    
    }
    public class KubeIngressList : IKubeResource
    {
        public KubeMetaData MetaData { get; set; }
        
        [JsonProperty("items")]
        public KubeIngress[] Items;
    }

}
