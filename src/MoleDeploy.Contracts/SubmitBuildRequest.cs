namespace MoleDeploy.Contracts
{
    public class SubmitBuildRequest
    {
        public string ServiceName { get; set; }

        public string PodColor { get; set; }

        public int PodReplicaCount { get; set; }
    }
}
