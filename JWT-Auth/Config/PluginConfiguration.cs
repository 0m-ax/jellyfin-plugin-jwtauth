using System;

namespace Jellyfin.Plugin.JWT_Auth.Config
{
    /// <summary>
    /// Plugin Configuration.
    /// </summary>
    public class PluginConfiguration : MediaBrowser.Model.Plugins.BasePluginConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
        /// </summary>
        public PluginConfiguration()
        {
            Audience = "jellyfin";
            Issuer = string.Empty;
            Key = "default_key";
            CreateUsersFromJWT = true;
            EnableAllFolders = false;
            EnabledFolders = Array.Empty<string>();
        }

        /// <summary>
        /// Gets or sets the jwt Audience.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the jwt Issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the jwt Key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create Jellyfin users from JWT.
        /// </summary>
        public bool CreateUsersFromJWT { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable access to all library folders.
        /// </summary>
        public bool EnableAllFolders { get; set; }

        /// <summary>
        /// Gets or sets a list of folder Ids which are enabled for access by default.
        /// </summary>
        public string[] EnabledFolders { get; set; }
    }
}
