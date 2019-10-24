using System.Runtime.Serialization;

namespace Umbraco.Core.Dashboards
{
    /// <summary>
    /// Represents a dashboard with only minimal data.
    /// </summary>
    public interface IDashboardSlim
    {
        /// <summary>
        /// Gets the alias of the dashboard.
        /// </summary>
        [DataMember(Name = "alias")]
        string Alias { get; }

        /// <summary>
        /// Gets the view used to render the dashboard.
        /// </summary>
        [DataMember(Name = "view")]
        string View { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this tab requires a header, even when it's the only tab in the section.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a header is required; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "requireHeader")]
        bool RequireHeader { get; }
    }
}
