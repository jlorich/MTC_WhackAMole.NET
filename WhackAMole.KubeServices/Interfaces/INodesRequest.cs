using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhackAMole.KubeServices.Models;

namespace WhackAMole.KubeServices.Interfaces
{
    interface INodesRequest
    {
        Task<KubeNode[]> GetAllAsync();
    }
}
