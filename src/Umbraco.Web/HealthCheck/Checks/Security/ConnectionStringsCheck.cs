namespace Umbraco.Web.HealthCheck.Checks.Security
{
    [HealthCheck(
        "02AF8996-8044-4E79-AA10-D616EA5E1D3A",
        "Protect connection strings",
        Description = "Checks if the connectionStrings section is protected (encrypted).",
        Group = "Security")]
    public class ConnectionStringsCheck : BaseProtectSectionCheck
    {
        public ConnectionStringsCheck(HealthCheckContext healthCheckContext)
            : base(healthCheckContext, "connectionStrings")
        { }
    }
}
