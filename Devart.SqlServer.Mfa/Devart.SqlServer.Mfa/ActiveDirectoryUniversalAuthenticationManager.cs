using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security;

namespace Devart.SqlServer.Mfa {

  internal class ActiveDirectoryUniversalAuthenticationManager {

    private const string SQL_AZURE_CLIENTID = "a94f9c62-97fe-4d19-b06d-472bed8d2bcf";

    internal static readonly IDictionary<string, string> TenantIdMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private static readonly IDictionary<string, ADALAuthenticationCredentials> TokenCache = new Dictionary<string, ADALAuthenticationCredentials>(StringComparer.OrdinalIgnoreCase);

    public ActiveDirectoryUniversalAuthenticationManager(
      string server,
      string userName,
      string resource,
      string authority) {

      this.Server = server;
      this.UserId = userName;
      this.Resource = resource;
      this.Authority = authority;
    }

    public string Server {
      get;
      private set;
    }

    public string AccessToken { get; private set; }

    public string Resource { get; private set; }

    public string Authority { get; private set; }

    public string TenantId { get; private set; }

    public DateTimeOffset TokenExpiry { get; private set; }

    public string UserId { get; private set; }

    public static void ClearTokenCache() => TokenCache.Clear();

    public string GetCachedAccessToken() {

      string tenantId = AzureADALAuthenticationConfiguration.TenantWildcard;
      string token = null;

      var azureCloud = new AzureCloud();

      if (TenantIdMap.ContainsKey(tenantId)) {

        AzureADALAuthenticationConfiguration config = new AzureADALAuthenticationConfiguration {
          Resource = azureCloud.SqlDatabaseServicePrincipalName,
          Tenant = tenantId,
          Authority = azureCloud.ActiveDirectoryAuthority,
          ClientID = SQL_AZURE_CLIENTID,
          UserName = UserId,
          Claims = this.GetConditionalAccessClaim()
        };
        this.SetAuthorityAndResourceUrls(config);
        ADALAuthenticationCredentials azureCredential = this.GetAzureCredential(config, false, true);
        if (azureCredential != null) {
          this.UpdateFromCredential(azureCredential);
          token = StringExtensions.SecureStringToString(azureCredential.Token.TenantToken);
        }
      }
      return token;
    }

    public void Run() {

      string adTenant = "common";
      IAzureCloudConfiguration azureCloud = new AzureCloud();

      AzureADALAuthenticationConfiguration config = new AzureADALAuthenticationConfiguration {
        Resource = azureCloud.SqlDatabaseServicePrincipalName,
        Tenant = adTenant,
        Authority = this.Authority ?? azureCloud.ActiveDirectoryAuthority,
        ClientID = SQL_AZURE_CLIENTID,
        UserName = this.UserId,
        Claims = this.GetConditionalAccessClaim()
      };
      this.SetAuthorityAndResourceUrls(config);
      ADALAuthenticationCredentials azureCredential = this.GetAzureCredential(config, false, false);
      this.UpdateFromCredential(azureCredential);
    }

    private ADALAuthenticationCredentials GetAzureCredential(AzureADALAuthenticationConfiguration config, bool validateAuthority, bool cacheOnly = false) {

      ADALAuthenticationCredentials token = null;
      ADALAuthenticationCredentials credentials2 = null;
      if (TokenCache.Count > 0) {
        token = this.LookupForCachedToken(config);
      }
      if (token != null) {
        if (token.Token.ExpiresOn >= DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(5.0))) {
          return token;
        }
        if (!cacheOnly) {
          TokenCache.Remove(token.GetCacheKey(config.Claims));
          try {
            credentials2 = AzureAuthenticationManagement.CreateAdalAuthentication(config, LoginPromptBehavior.Never, validateAuthority, null);
            this.UpdateTokenCache(config, credentials2);
          }
          catch (InvalidOperationException) {
          }
        }
        return credentials2;
      }
      if (!cacheOnly) {
        credentials2 = AzureAuthenticationManagement.CreateAdalAuthentication(config, LoginPromptBehavior.Always, validateAuthority, null);
        this.UpdateTokenCache(config, credentials2);
      }
      return credentials2;
    }

    private string GetConditionalAccessClaim() {

      return null;
      //if (!Settings<SqlStudio>.Current.SSMS.AzureCloud.EnableConditionalAccess) {
      //  return null;
      //}
      //return $"{{"access_token":{{"acrs":{{"essential":true,"value":"urn: sql: { this._connectionInfo.ServerName}}}}}}}";
    }

    private ADALAuthenticationCredentials LookupForCachedToken(AzureADALAuthenticationConfiguration config) {

      string cacheKey = config.GetCacheKey(config.UserName ?? (this.UserId ?? this.UserId));
      if (TokenCache.ContainsKey(cacheKey)) {
        return TokenCache[cacheKey];
      }
      return null;
    }

    private void SetAuthorityAndResourceUrls(AzureADALAuthenticationConfiguration config) {

      IAzureCloudConfiguration azureCloudConfiguration = AzureCloud.GetAzureCloudConfiguration(this.Server, true);
      config.Authority = azureCloudConfiguration.ActiveDirectoryAuthority;
      config.Resource = azureCloudConfiguration.SqlDatabaseServicePrincipalName;
    }

    private void UpdateFromCredential(ADALAuthenticationCredentials azureCredential) {

      this.UserId = azureCredential.Token.UserId;
      this.AccessToken = StringExtensions.SecureStringToString(azureCredential.Token.TenantToken);
      this.Resource = azureCredential.Token.Resource;
      this.TokenExpiry = azureCredential.Token.ExpiresOn;
      this.TenantId = azureCredential.Token.TenantId;
    }

    private void UpdateTokenCache(AzureADALAuthenticationConfiguration config, ADALAuthenticationCredentials token) {

      TenantIdMap[config.Tenant] = token.Token.TenantId;
      TokenCache[token.GetCacheKey(config.Claims)] = token;
      TokenCache[TokenExtensions.FormatCacheKey(config.Resource, token.Token.TenantId, config.UserName ?? this.UserId, config.Claims)] = token;
    }
  }
}
