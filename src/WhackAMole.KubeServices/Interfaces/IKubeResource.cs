using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.KubeServices.Models;

namespace WhackAMole.KubeServices.Interfaces
{
    public interface IKubeResource
    {
        KubeMetaData MetaData { get; set; }
    }
}
