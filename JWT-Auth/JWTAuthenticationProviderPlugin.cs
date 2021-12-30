using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Data.Entities;
using Jellyfin.Data.Enums;
using JWT;
using MediaBrowser.Common;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JWT_Auth
{
    /// <summary>
    /// JWT Authentication Provider Plugin.
    /// </summary>
    public class JWTAuthenticationProviderPlugin : IAuthenticationProvider
    {
        private readonly ILogger<JWTAuthenticationProviderPlugin> _logger;
        private readonly IApplicationHost _applicationHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="JWTAuthenticationProviderPlugin"/> class.
        /// </summary>
        /// <param name="applicationHost">Instance of the <see cref="IApplicationHost"/> interface.</param>
        /// <param name="logger">Instance of the <see cref="ILogger{JWTAuthenticationProviderPlugin}"/> interface.</param>
        public JWTAuthenticationProviderPlugin(IApplicationHost applicationHost, ILogger<JWTAuthenticationProviderPlugin> logger)
        {
            _logger = logger;
            _applicationHost = applicationHost;
        }

        /// <summary>
        /// Gets plugin name.
        /// </summary>
        public string Name => "JWT-Authentication";

        /// <summary>
        /// Gets a value indicating whether gets plugin enabled.
        /// </summary>
        public bool IsEnabled => true;

        /// <summary>
        /// Authenticate user against the JWT.
        /// </summary>
        /// <param name="username">Username to authenticate.</param>
        /// <param name="password">Password to authenticate.</param>
        /// <returns>A <see cref="ProviderAuthenticationResult"/> with the authentication result.</returns>
        /// <exception cref="AuthenticationException">Exception when failing to authenticate.</exception>
        public async Task<ProviderAuthenticationResult> Authenticate(string username, string password)
        {
            var userManager = _applicationHost.Resolve<IUserManager>();

            var payload = JWT.Builder.JwtBuilder.Create()
            .WithAlgorithm(new JWT.Algorithms.HMACSHA256Algorithm()) // symmetric
            .WithSecret(JWTPlugin.Instance.Configuration.Key)
            .MustVerifySignature()
            .Decode<IDictionary<string, object>>(password);
            if (payload.ContainsKey("iss") && payload.ContainsKey("iss").ToString() != JWTPlugin.Instance.Configuration.Issuer)
            {
                throw new AuthenticationException("Invalid iss");
            }

            if (payload.ContainsKey("aud") && payload.ContainsKey("aud").ToString() != JWTPlugin.Instance.Configuration.Audience)
            {
                throw new AuthenticationException("Invalid aud");
            }

            User user = null;

            if (!payload.ContainsKey("sub"))
            {
                throw new AuthenticationException("Missing Subject");
            }

            var name = payload["sub"].ToString();

            try
            {
                user = userManager.GetUserByName(name);
            }
            catch (Exception e)
            {
                _logger.LogInformation("User Manager could not find a user for jwt", e);
            }

            bool? isAdmin = payload?["admin"] as bool?;
            if (user == null)
            {
                _logger.LogInformation("Creating new user {name}", name);
                user = await userManager.CreateUserAsync(name).ConfigureAwait(false);
                user.AuthenticationProviderId = GetType().FullName;
                if (isAdmin != null)
                {
                    user.SetPermission(PermissionKind.IsAdministrator, isAdmin.Value);
                }

                user.SetPermission(PermissionKind.EnableAllFolders, JWTPlugin.Instance.Configuration.EnableAllFolders);
                if (!JWTPlugin.Instance.Configuration.EnableAllFolders)
                {
                    user.SetPreference(PreferenceKind.EnabledFolders, JWTPlugin.Instance.Configuration.EnabledFolders);
                }

                await userManager.UpdateUserAsync(user).ConfigureAwait(false);
            }
            else
            {
                if (isAdmin != null)
                {
                    user.SetPermission(PermissionKind.IsAdministrator, isAdmin.Value);
                }

                await userManager.UpdateUserAsync(user).ConfigureAwait(false);
            }

            return new ProviderAuthenticationResult { Username = name };
        }

        /// <inheritdoc />
        public bool HasPassword(User user)
        {
            return true;
        }

        /// <inheritdoc />
        public Task ChangePassword(User user, string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}
