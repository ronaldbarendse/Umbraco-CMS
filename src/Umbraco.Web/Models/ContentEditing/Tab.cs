using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Umbraco.Web.Models.ContentEditing
{
    /// <summary>
    /// Represents a tab in the UI.
    /// </summary>
    /// <typeparam name="T">The type of properties within the tab.</typeparam>
    [DataContract(Name = "tab", Namespace = "")]
    public class Tab<T>
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "alias")]
        public string Alias { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "properties")]
        public IEnumerable<T> Properties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this tab requires a header, even when it's the only tab in the section.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a header is required; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "requireHeader")]
        public bool RequireHeader { get; set; }
    }
}
