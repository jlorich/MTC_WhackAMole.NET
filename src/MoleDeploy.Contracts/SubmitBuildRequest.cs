using Newtonsoft.Json;

namespace MoleDeploy.Contracts
{
    public class SubmitBuildRequest
    {
        [JsonProperty(PropertyName = "service_name")]
        public string ServiceName { get; set; }

        [JsonProperty(PropertyName = "pod_color")]
        public string PodColor { get; set; }

        [JsonProperty(PropertyName = "pod_replica_count")]
        public string PodReplicaCount { get; set; }
    }
}
