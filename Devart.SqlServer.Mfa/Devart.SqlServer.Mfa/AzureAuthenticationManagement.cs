using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Devart.SqlServer.Mfa {

  internal class AzureAuthenticationManagement {

    public static ADALAuthenticationCredentials CreateAdalAuthentication(AzureADALAuthenticationConfiguration profile, ClientCredential clientCredential, bool validateAuthority = false) {

      if (profile == null)
        throw new ArgumentNullException("profile");

      AzureAuthenticationManagementToken token = new AzureADALAuthenticationManager(profile).AcquireUserAuthenticationToken(clientCredential, validateAuthority);
      if (token == null)
        throw new InvalidOperationException("Microsoft.SqlServer.Management.AzureAuthenticationManagement.Strings.UnableToAcquireToken");
      return new ADALAuthenticationCredentials(profile.Resource, token);
    }

    public static ADALAuthenticationCredentials CreateAdalAuthentication(AzureADALAuthenticationConfiguration profile, IClientAssertionCertificate clientAssertionCertificate, bool validateAuthority = false) {

      if (profile == null)
        throw new ArgumentNullException("profile");

      AzureAuthenticationManagementToken token = new AzureADALAuthenticationManager(profile).AcquireUserAuthenticationToken(clientAssertionCertificate, validateAuthority);
      if (token == null)
        throw new InvalidOperationException("Microsoft.SqlServer.Management.AzureAuthenticationManagement.Strings.UnableToAcquireToken");
      return new ADALAuthenticationCredentials(profile.Resource, token);
    }

    public static ADALAuthenticationCredentials CreateAdalAuthentication(AzureADALAuthenticationConfiguration profile, LoginPromptBehavior options = LoginPromptBehavior.Default, bool validateAuthority = false, object ownerWindow = null) {

      if (profile == null)
        throw new ArgumentNullException("profile");

      bool flag = false;
      AzureADALAuthenticationManager manager = new AzureADALAuthenticationManager(profile);
      AzureAuthenticationManagementToken token = null;
      do {
        try {
          token = manager.AcquireUserAuthenticationToken(flag ? LoginPromptBehavior.Auto : options, validateAuthority, ownerWindow);
          flag = false;
        }
        catch (AggregateException exception) {
          if (exception.InnerExceptions[0] is AdalUserMismatchException)
            throw new InvalidOperationException("Microsoft.SqlServer.Management.AzureAuthenticationManagement.Strings.UnableToAcquireToken", exception.InnerExceptions[0]);

          AdalException exception2 = exception.InnerExceptions[0] as AdalException;
          if ((flag || (exception2 == null)) || (exception2.ErrorCode != "interaction_required"))
            throw new InvalidOperationException("Microsoft.SqlServer.Management.AzureAuthenticationManagement.Strings.UnableToAcquireToken", exception);
          flag = true;
        }
        catch (AdalUserMismatchException exception3) {
          throw new InvalidOperationException("Microsoft.SqlServer.Management.AzureAuthenticationManagement.Strings.UnableToAcquireToken", exception3);
        }
        catch (AdalException exception4) {
          if (flag || (exception4.ErrorCode != "interaction_required"))
            throw new InvalidOperationException("Microsoft.SqlServer.Management.AzureAuthenticationManagement.Strings.UnableToAcquireToken", exception4);
          flag = true;
        }
      }
      while (flag);
      if (token == null) {
        throw new InvalidOperationException("Microsoft.SqlServer.Management.AzureAuthenticationManagement.Strings.UnableToAcquireToken");
      }
      return new ADALAuthenticationCredentials(profile.Resource, token);
    }

    public static List<CertificateAuthenticationCredentials> CreateCertificateAuthentication(PublishData profiles) {

      List<CertificateAuthenticationCredentials> list = new List<CertificateAuthenticationCredentials>();

      if (((profiles == null) || (profiles.Items == null)) || (profiles.Items.Count<PublishDataPublishProfile>() < 1))
        throw new ArgumentNullException("profile");

      foreach (PublishDataPublishProfile profile in profiles.Items)
        if ((profile.Subscription != null) && (profile.Subscription.Count<PublishDataPublishProfileSubscription>() > 1))
          foreach (PublishDataPublishProfileSubscription subscription in profile.Subscription)
            list.Add(new CertificateAuthenticationCredentials(subscription.ServiceManagementUrl, subscription.Id, subscription.ManagementCertificate, StoreLocation.CurrentUser, StoreName.My));

      return list;
    }

    public static CertificateAuthenticationCredentials CreateCertificateAuthentication(PublishDataPublishProfileSubscription profile) {

      if (((profile == null) || string.IsNullOrEmpty(profile.ManagementCertificate)) || (string.IsNullOrEmpty(profile.Id) || string.IsNullOrEmpty(profile.ServiceManagementUrl)))
        throw new ArgumentNullException("profile");
      return new CertificateAuthenticationCredentials(profile.ServiceManagementUrl, profile.Id, profile.ManagementCertificate, StoreLocation.CurrentUser, StoreName.My);
    }
  }
}
