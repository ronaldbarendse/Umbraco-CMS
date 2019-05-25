using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Models;

namespace Umbraco.Web.Routing
{
    /// <summary>
    /// Provides URLs.
    /// </summary>
    public class UrlProvider
    {
        #region Fields

        /// <summary>
        /// The Umbraco context.
        /// </summary>
        private readonly UmbracoContext _umbracoContext;

        /// <summary>
        /// The URL providers.
        /// </summary>
        private readonly IEnumerable<IUrlProvider> _urlProviders;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the provider URL mode.
        /// </summary>
        /// <value>
        /// The provider URL mode.
        /// </value>
        public UrlProviderMode Mode { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlProvider" /> class with an Umbraco context and a list of URL providers.
        /// </summary>
        /// <param name="umbracoContext">The Umbraco context.</param>
        /// <param name="routingSettings">The routing settings.</param>
        /// <param name="urlProviders">The list of URL providers.</param>
        /// <exception cref="ArgumentNullException">umbracoContext
        /// or
        /// routingSettings</exception>
        public UrlProvider(UmbracoContext umbracoContext, IWebRoutingSection routingSettings, IEnumerable<IUrlProvider> urlProviders)
        {
            this._umbracoContext = umbracoContext ?? throw new ArgumentNullException("umbracoContext");
            this._urlProviders = urlProviders;

            if (routingSettings == null)
                throw new ArgumentNullException("routingSettings");

            if (!Enum<UrlProviderMode>.TryParse(routingSettings.UrlProviderMode, out var provider))
            {
                provider = UrlProviderMode.Auto;
            }

            this.Mode = provider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlProvider" /> class with an Umbraco context and a list of URL providers.
        /// </summary>
        /// <param name="umbracoContext">The Umbraco context.</param>
        /// <param name="urlProviders">The list of URL providers.</param>
        /// <param name="provider">The provider.</param>
        /// <exception cref="ArgumentNullException">umbracoContext</exception>
        public UrlProvider(UmbracoContext umbracoContext, IEnumerable<IUrlProvider> urlProviders, UrlProviderMode provider = UrlProviderMode.Auto)
        {
            this._umbracoContext = umbracoContext ?? throw new ArgumentNullException("umbracoContext");
            this._urlProviders = urlProviders;

            this.Mode = provider;
        }

        #endregion

        #region GetUrl

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>Mode</c> and on the current URL.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(Guid id)
        {
            return this.GetUrl(id, this._umbracoContext.CleanedUmbracoUrl, this.Mode);
        }

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <param name="absolute">A value indicating whether the URL should be absolute in any case.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>Mode</c> and on <c>current</c>, unless <c>absolute</c> is true, in which case the URL is always absolute.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(Guid id, bool absolute)
        {
            return this.GetUrl(id, this._umbracoContext.CleanedUmbracoUrl, absolute);
        }

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <param name="current">The current absolute URL.</param>
        /// <param name="absolute">A value indicating whether the URL should be absolute in any case.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>Mode</c> and on <c>current</c>, unless <c>absolute</c> is true, in which case the URL is always absolute.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(Guid id, Uri current, bool absolute)
        {
            var mode = absolute ? UrlProviderMode.Absolute : this.Mode;

            return this.GetUrl(id, current, mode);
        }

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <param name="mode">The URL mode.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>mode</c> and on the current URL.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(Guid id, UrlProviderMode mode)
        {
            return this.GetUrl(id, this._umbracoContext.CleanedUmbracoUrl, mode);
        }

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <param name="current">The current absolute URL.</param>
        /// <param name="mode">The URL mode.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>mode</c> and on <c>current</c>.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(Guid id, Uri current, UrlProviderMode mode)
        {
            var intId = this._umbracoContext.Application.Services.EntityService.GetIdForKey(id, UmbracoObjectTypes.Document);
            
            return this.GetUrl(intId.Success ? intId.Result : -1, current, mode);
        }

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>Mode</c> and on the current URL.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(int id)
        {
            return this.GetUrl(id, this._umbracoContext.CleanedUmbracoUrl, this.Mode);
        }

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <param name="absolute">A value indicating whether the URL should be absolute in any case.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>Mode</c> and on <c>current</c>, unless <c>absolute</c> is true, in which case the URL is always absolute.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(int id, bool absolute)
        {
            return this.GetUrl(id, this._umbracoContext.CleanedUmbracoUrl, absolute);
        }

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <param name="current">The current absolute URL.</param>
        /// <param name="absolute">A value indicating whether the URL should be absolute in any case.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>Mode</c> and on <c>current</c>, unless <c>absolute</c> is true, in which case the URL is always absolute.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(int id, Uri current, bool absolute)
        {
            var mode = absolute ? UrlProviderMode.Absolute : this.Mode;

            return this.GetUrl(id, current, mode);
        }

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content identifier.</param>
        /// <param name="mode">The URL mode.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>mode</c> and on the current URL.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(int id, UrlProviderMode mode)
        {
            return this.GetUrl(id, this._umbracoContext.CleanedUmbracoUrl, mode);
        }

        /// <summary>
        /// Gets the URL of a published content.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <param name="current">The current absolute URL.</param>
        /// <param name="mode">The URL mode.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>The URL is absolute or relative depending on <c>mode</c> and on <c>current</c>.</para>
        /// <para>If the provider is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        public string GetUrl(int id, Uri current, UrlProviderMode mode)
        {
            var url = this._urlProviders.Select(provider => provider.GetUrl(this._umbracoContext, id, current, mode))
                .FirstOrDefault(u => u != null);

            return url ?? "#"; // legacy wants this
        }

        /// <summary>
        /// Gets the URL from route using the <see cref="DefaultUrlProvider" />.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <param name="route">The route.</param>
        /// <returns>
        /// The URL for the published content.
        /// </returns>
        /// <remarks>
        /// <para>If the <see cref="DefaultUrlProvider" /> cannot be found or is unable to provide a URL, it returns "#".</para>
        /// </remarks>
        internal string GetUrlFromRoute(int id, string route)
        {
            var provider = this._urlProviders.OfType<DefaultUrlProvider>().FirstOrDefault();
            var url = provider == null
                ? route // what else?
                : provider.GetUrlFromRoute(route, UmbracoContext.Current, id, this._umbracoContext.CleanedUmbracoUrl, this.Mode);

            return url ?? "#";
        }

        #endregion

        #region GetOtherUrls

        /// <summary>
        /// Gets the other URLs of a published content.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <returns>
        /// The other URLs for the published content.
        /// </returns>
        /// <remarks>
        /// <para>Other URLs are those that <see cref="GetUrl" /> would not return in the current context,
        /// but would be valid URLs for the node in other contexts (different domain for current request, umbracoURLAlias...).</para>
        /// <para>The results depend on the current URL.</para>
        /// </remarks>
        public IEnumerable<string> GetOtherUrls(int id)
        {
            return this.GetOtherUrls(id, this._umbracoContext.CleanedUmbracoUrl);
        }

        /// <summary>
        /// Gets the other URLs of a published content.
        /// </summary>
        /// <param name="id">The published content id.</param>
        /// <param name="current">The current absolute URL.</param>
        /// <returns>
        /// The other URLs for the published content.
        /// </returns>
        /// <remarks>
        /// Other URLs are those that <see cref="GetUrl" /> would not return in the current context,
        /// but would be valid URLs for the node in other contexts (different domain for current request, umbracoURLAlias...).
        /// </remarks>
        public IEnumerable<string> GetOtherUrls(int id, Uri current)
        {
            // Providers can return null or an empty list or a non-empty list, be prepared
            var urls = this._urlProviders.SelectMany(provider => provider.GetOtherUrls(this._umbracoContext, id, current) ?? Enumerable.Empty<string>());

            return urls;
        }

        #endregion
    }
}
