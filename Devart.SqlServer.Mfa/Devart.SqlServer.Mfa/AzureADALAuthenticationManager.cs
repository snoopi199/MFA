using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Threading.Tasks;

namespace Devart.SqlServer.Mfa {

  internal class AzureADALAuthenticationManager {

    public AzureADALAuthenticationManager(AzureADALAuthenticationConfiguration configuration) {

      if (configuration == null)
        throw new ArgumentNullException("configuration");

      this.Configuration = configuration;
    }

    public AzureADALAuthenticationConfiguration Configuration {
      get;
      private set;
    }

    public AzureAuthenticationManagementToken AcquireUserAuthenticationToken(ClientCredential clientCredential, bool validateAuthority = false) {

      if (clientCredential == null)
        throw new ArgumentNullException("clientCredential");

      AuthenticationContext context = new AuthenticationContext(this.CreateAuthorityEndpoint(), validateAuthority);
      return CreateAzureAuthenticationToken(context.AcquireTokenAsync(this.Configuration.Resource, clientCredential).Result, this.Configuration.Authority, this.Configuration.Resource);
    }

    public AzureAuthenticationManagementToken AcquireUserAuthenticationToken(IClientAssertionCertificate clientAssertionCertificate, bool validateAuthority = false) {

      if (clientAssertionCertificate == null)
        throw new ArgumentNullException("clientAssertionCertificate");

      AuthenticationContext context = new AuthenticationContext(this.CreateAuthorityEndpoint(), validateAuthority);
      return CreateAzureAuthenticationToken(context.AcquireTokenAsync(this.Configuration.Resource, clientAssertionCertificate).Result, this.Configuration.Authority, this.Configuration.Resource);
    }

    public AzureAuthenticationManagementToken AcquireUserAuthenticationToken(LoginPromptBehavior options = LoginPromptBehavior.RefreshSession, bool validateAuthority = false, object ownerWindow = null) {

      AuthenticationContext context = new AuthenticationContext(this.CreateAuthorityEndpoint(), validateAuthority);
      var promptBehavior = PromptBehaviorFromLoginPromptBehavior(options);
      var platformParameters = ownerWindow is null ? new PlatformParameters(promptBehavior) : new PlatformParameters(promptBehavior, ownerWindow);
      Task<AuthenticationResult> task = string.IsNullOrEmpty(this.Configuration.UserName) ? context.AcquireTokenAsync(this.Configuration.Resource, this.Configuration.ClientID, this.Configuration.RedirectUri, platformParameters, UserIdentifier.AnyUser, null, this.Configuration.Claims) : context.AcquireTokenAsync(this.Configuration.Resource, this.Configuration.ClientID, this.Configuration.RedirectUri, platformParameters, new UserIdentifier(this.Configuration.UserName, UserIdentifierType.RequiredDisplayableId), null, this.Configuration.Claims);
      return CreateAzureAuthenticationToken(task.Result, this.Configuration.Authority, this.Configuration.Resource);
    }

    public AzureAuthenticationManagementToken AcquireUserAuthenticationTokenByRefreshToken(AzureAuthenticationManagementToken oldToken, bool validateAuthority = false) {

      AuthenticationContext context = new AuthenticationContext(oldToken.Authority.TrimEnd(new char[] { '/' }), validateAuthority);
      return CreateAzureAuthenticationToken(context.AcquireTokenSilentAsync(this.Configuration.Resource, this.Configuration.ClientID, new UserIdentifier(oldToken.UserId, UserIdentifierType.UniqueId)).Result, oldToken.Authority, this.Configuration.Resource);
    }

    private string CreateAuthorityEndpoint() =>
        string.Format(CultureInfo.InvariantCulture, "{0}/{1}/", new object[] { this.Configuration.Authority.TrimEnd(new char[] { '/' }), this.Configuration.Tenant });

    private static AzureAuthenticationManagementToken CreateAzureAuthenticationToken(AuthenticationResult result, string authority, string resource) =>
        new AzureAuthenticationManagementToken(result, authority, resource);

    private static PromptBehavior PromptBehaviorFromLoginPromptBehavior(LoginPromptBehavior behavior) {

      switch (behavior) {
        case LoginPromptBehavior.Always:
          return PromptBehavior.Always;

        case LoginPromptBehavior.Never:
          return PromptBehavior.Never;

        case LoginPromptBehavior.RefreshSession:
          return PromptBehavior.RefreshSession;
      }
      return PromptBehavior.Auto;
    }
  }
}
