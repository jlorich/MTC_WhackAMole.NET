using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WhackAMole.KubeServices.Providers
{
    public interface IAuthenticationProvider
    {
        HttpClient GetConnection();
    }
}
