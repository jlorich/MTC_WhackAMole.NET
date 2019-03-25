using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.KubeServices.Models;

namespace WhackAMole.KubeServices.Interfaces
{
    internal interface IPodsRequest : IKubeRequest
    {
        Task<KubePod[]> GetAllAsync(string appname = "");
    }

   
}
