using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhackAMole.KubeServices.Providers;
using WhackAMole.KubeServices.Interfaces;
using WhackAMole.KubeServices.Models;

namespace WhackAMole.KubeServices
{
    public class IngressRequest : KubeBaseRequest<KubeNode>, IIngressRequest
    {
        public IngressRequest(IAuthenticationProvider auth, KubeOptions settings) : base(auth, settings)
        {

        }

        public async Task<KubeIngress[]> GetAllAsync()
        {
            var list = await GetAsync<KubeIngressList>("extensions/v1beta1/ingresses");

            return list.Items;
        }
    }
}
