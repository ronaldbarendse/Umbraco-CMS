using System;
using System.Collections.Generic;
using System.Web.Configuration;

namespace Umbraco.Web.HealthCheck.Checks.Security
{
    public abstract class BaseProtectSectionCheck : HealthCheck
    {
        private const string protectSection = "protectSection";
        private const string unprotectSection = "unprotectSection";
        private readonly string sectionName;
        private readonly string configPath;
        private readonly string provider;

        protected BaseProtectSectionCheck(HealthCheckContext healthCheckContext, string sectionName, string configPath = "/", string provider = "DataProtectionConfigurationProvider")
            : base(healthCheckContext)
        {
            this.sectionName = sectionName;
            this.configPath = configPath;
            this.provider = provider;
        }

        public override IEnumerable<HealthCheckStatus> GetStatus()
        {
            return new[] { this.CheckSection() };
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
        {
            switch (action.Alias)
            {
                case protectSection:
                    return this.UpdateSection(true);
                case unprotectSection:
                    return this.UpdateSection(false);
                default:
                    throw new InvalidOperationException();
            }
        }

        protected HealthCheckStatus CheckSection()
        {
            var config = WebConfigurationManager.OpenWebConfiguration(this.configPath);
            var section = config.GetSection(this.sectionName);
            if (section == null)
            {
                return new HealthCheckStatus("Section not found.")
                {
                    ResultType = StatusResultType.Info
                };
            }

            if (section.SectionInformation.IsProtected)
            {
                return new HealthCheckStatus("Section is protected.")
                {
                    ResultType = StatusResultType.Success,
                    Description = "The section is protected using provider: " + section.SectionInformation.ProtectionProvider.Name,
                    Actions = new[]
                    {
                        new HealthCheckAction(unprotectSection, this.Id)
                        {
                            Name = "Unprotect section"
                            //Description = _textService.Localize(string.Format("healthcheck/{0}SetHeaderInConfigDescription", _localizedTextPrefix))
                        }
                    }
                };
            }

            return new HealthCheckStatus("Section is not protected!")
            {
                ResultType = StatusResultType.Error,
                Actions = new[]
                {
                    new HealthCheckAction(protectSection, this.Id)
                    {
                        Name = "Protect section"
                        //Name = _textService.Localize("healthcheck/setHeaderInConfig"),
                        //Description = _textService.Localize(string.Format("healthcheck/{0}SetHeaderInConfigDescription", _localizedTextPrefix))
                    }
                }
            };
        }

        protected HealthCheckStatus UpdateSection(bool protect)
        {
            var config = WebConfigurationManager.OpenWebConfiguration(this.configPath);
            var section = config.GetSection(this.sectionName);

            if (protect)
            {
                section.SectionInformation.ProtectSection(this.provider);
            }
            else
            {
                section.SectionInformation.UnprotectSection();
            }

            config.Save();

            return this.CheckSection();
        }
    }
}
