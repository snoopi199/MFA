using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security;

namespace Devart.SqlServer.Mfa {

  internal class AzureAuthenticationManagementToken {

    internal AzureAuthenticationManagementToken(AuthenticationResult result, string authority, string resource) {

      this.Authority = authority;
      this.ExpiresOn = result.ExpiresOn;
      this.Resource = resource;
      this.TenantId = result.TenantId;
      this.TenantToken = result.AccessToken.StringToSecureString();
      this.UserId = result.UserInfo?.DisplayableId;
    }

    public string Authority { get; private set; }

    public DateTimeOffset ExpiresOn { get; private set; }

    public string Resource { get; private set; }

    public string TenantId { get; private set; }

    public SecureString TenantToken { get; private set; }

    public string UserId { get; private set; }
  }
}
