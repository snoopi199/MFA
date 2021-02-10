using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security;

namespace Devart.SqlServer.Mfa {

  internal class ADALAuthenticationCredentials {

    public ADALAuthenticationCredentials(string managementUri, AzureAuthenticationManagementToken token) {

      if (string.IsNullOrEmpty(managementUri))
        throw new ArgumentNullException("managementUri");
      if (token == null)
        throw new ArgumentNullException("token");

      this.Token = token;
      this.ManagementUri = managementUri;
    }

    public AzureAuthenticationType AuthenticationType => AzureAuthenticationType.ActiveDirectoryAuthentication;

    public string ManagementUri { get; set; }

    public string SubscriptionId { get; set; }

    public AzureAuthenticationManagementToken Token { get; private set; }
  }
}
