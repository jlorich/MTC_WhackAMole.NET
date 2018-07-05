using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WhackAMole.KubeServices.Providers;
using WhackAMole.KubeServices.Interfaces;

namespace WhackAMole.KubeServices
{
    public class KubeRequestBuilder
    {
        private IAuthenticationProvider _auth;
        KubeSettings _settings;

        public KubeRequestBuilder(IAuthenticationProvider auth, KubeSettings settings)
        {
            _auth = auth;
            _settings = settings;
        }

        public T Create<T>() where T : IKubeRequest
        {
            var table = (IKubeRequest)Activator.CreateInstance(
                            typeof(T),
                            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null,
                            new object[] { _auth , _settings}, null);

            return (T) table;
        }
    }
}
