using System;
using System.Collections.Generic;
using Jellyfin.Plugin.JWT_Auth.Config;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.JWT_Auth
{
    /// <summary>
    /// JWT Plugin.
    /// </summary>
    public class JWTPlugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JWTPlugin"/> class.
        /// </summary>
        /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
        /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/>interface.</param>
        public JWTPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        /// <summary>
        /// Gets the plugin instance.
        /// </summary>
        public static JWTPlugin Instance { get; private set; }

        /// <inheritdoc />
        public override string Name => "JWT-Auth";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("8aec4e04-77ea-4c5f-bd7f-64d178324cd5");

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = Name,
                    EmbeddedResourcePath = $"{GetType().Namespace}.Config.configPage.html"
                }
            };
        }
    }
}
