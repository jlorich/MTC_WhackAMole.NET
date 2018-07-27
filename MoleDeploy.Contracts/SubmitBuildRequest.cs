namespace MoleDeploy.Vsts
{
    public class SubmitBuildRequest
    {
        /// <summary>
        /// The color the service should represent itself as
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The number of replicas the service should have
        /// </summary>
        public int ReplicaCount { get; set; }

        /// <summary>
        /// The name for the Kubernetes service to be deployed
        /// </summary>
        public string ServiceName { get; set; }
    }
}
