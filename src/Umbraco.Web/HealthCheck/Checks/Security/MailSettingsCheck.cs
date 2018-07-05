namespace Umbraco.Web.HealthCheck.Checks.Security
{
    [HealthCheck(
        "FBF9CEE6-84CB-46AD-A004-E0423EF1C6A9",
        "Protect mail settings SMTP",
        Description = "Checks if the mailSettings smtp section is protected (encrypted).",
        Group = "Security")]
    public class MailSettingsCheck : BaseProtectSectionCheck
    {
        public MailSettingsCheck(HealthCheckContext healthCheckContext)
            : base(healthCheckContext, "system.net/mailSettings/smtp")
        { }
    }
}
