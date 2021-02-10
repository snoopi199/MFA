using System;

namespace Devart.SqlServer.Mfa {

  internal class CertificateAuthenticationCredentials {

    public CertificateAuthenticationCredentials(string managementUri, string subscriptionId, string certificateThumbprint, StoreLocation storeLocation = StoreLocation.CurrentUser, StoreName storeName = StoreName.My) {

      if (string.IsNullOrEmpty(managementUri))
        throw new ArgumentNullException("managementUri");

      if (string.IsNullOrEmpty(subscriptionId))
        throw new ArgumentNullException("subscription");

      if (string.IsNullOrEmpty(certificateThumbprint))
        throw new ArgumentNullException("certificateThumbprint");

      this.ManagementUri = managementUri;
      this.SubscriptionId = subscriptionId;
      this.CertificateThumbprint = certificateThumbprint;
      this.CertificateStoreName = storeName;
      this.CertificateStoreLocation = storeLocation;
    }

    public AzureAuthenticationType AuthenticationType =>
        AzureAuthenticationType.CertificateBasedAuthentication;

    public StoreLocation CertificateStoreLocation { get; set; }

    public StoreName CertificateStoreName { get; set; }

    public string CertificateThumbprint { get; private set; }

    public string ManagementUri { get; set; }

    public string SubscriptionId { get; set; }
  }
}
