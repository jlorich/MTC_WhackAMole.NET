using System;
using System.Collections.Generic;
using System.Text;

namespace WhackAMole.KubeServices
{
    public class KubeSettings
    {
        public string BaseApiUrl { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenPath { get; set; } = "/var/run/secrets/kubernetes.io/serviceaccount/token";

    }
}
