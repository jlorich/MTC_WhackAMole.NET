namespace MoleDeploy.Contracts
{
    public enum VstsBuildState
    {
        Unknown = 0,
        BuildingApplication,
        PublishingContainer,
        UpgradingCluster,
        DeployComplete,
        Failed
    }
}
