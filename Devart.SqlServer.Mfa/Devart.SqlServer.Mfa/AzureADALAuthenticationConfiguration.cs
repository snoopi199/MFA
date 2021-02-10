using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security;

namespace Devart.SqlServer.Mfa {

  internal class AzureADALAuthenticationConfiguration {

    public const string TenantWildcard = "common";

    public AzureADALAuthenticationConfiguration() {

      this.Resource = "https://management.core.windows.net/";
      this.Authority = "https://login.windows.net";
      this.Tenant = "common";
      this.ClientID = "aba285d5-d9f3-427b-a994-e9deb4567639";
      this.RedirectUri = new Uri(@"urn:ietf:wg:oauth:2.0:oob");
    }

    public string Authority { get; set; }

    public string Claims { get; set; }

    public string ClientID { get; set; }

    public Uri RedirectUri { get; set; }

    public string Resource { get; set; }

    public string Tenant { get; set; }

    public string UserName { get; set; }
  }
}
