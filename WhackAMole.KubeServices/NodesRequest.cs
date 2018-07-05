using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhackAMole.KubeServices.Providers;
using WhackAMole.KubeServices.Interfaces;
using WhackAMole.KubeServices.Models;

namespace WhackAMole.KubeServices
{
    public class NodesRequest : KubeBaseRequest<KubeNode>, INodesRequest
    {
        public NodesRequest(IAuthenticationProvider auth, KubeSettings settings) : base(auth, settings)
        {

        }

        public async Task<KubeNode[]> GetAllAsync()
        {
            var list = await GetAsync<KubeNodeList>("nodes");

            return list.Items;
        }

        public async Task<bool> DeleteAsync(string name)
        {
            return await base.DeleteAsync($"nodes/{name}");
        }
    }
}
