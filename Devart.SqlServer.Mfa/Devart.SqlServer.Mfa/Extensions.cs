using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Devart.SqlServer.Mfa {

  internal static class StringExtensions {

    public static SecureString CharArrayToSecureString(char[] charArray) {

      if (charArray == null)
        return null;

      SecureString str = new SecureString();
      foreach (char ch in charArray)
        str.AppendChar(ch);
      str.MakeReadOnly();
      return str;
    }

    public static char[] SecureStringToCharArray(SecureString secureString) {

      if (secureString == null)
        return null;

      char[] destination = new char[secureString.Length];
      IntPtr source = Marshal.SecureStringToGlobalAllocUnicode(secureString);
      try {
        Marshal.Copy(source, destination, 0, secureString.Length);
      }
      finally {
        Marshal.ZeroFreeGlobalAllocUnicode(source);
      }
      return destination;
    }

    public static string SecureStringToString(this SecureString secureString) => new string(SecureStringToCharArray(secureString));

    public static SecureString StringToSecureString(this string unsecureString) => CharArrayToSecureString(unsecureString.ToCharArray());
  }

  internal static class TokenExtensions {

    public static string FormatCacheKey(string resource, string tenant, string userId, string claims) =>
        $"{resource.TrimEnd(new char[] { '/' })}{tenant}{userId}{claims}";

    public static string GetCacheKey(this ADALAuthenticationCredentials token, string claims) =>
        FormatCacheKey(token.Token.Resource, token.Token.TenantId, token.Token.UserId, claims);

    public static string GetCacheKey(this AzureADALAuthenticationConfiguration config, string userName) {
      string tenant = ActiveDirectoryUniversalAuthenticationManager.TenantIdMap.ContainsKey(config.Tenant) ? ActiveDirectoryUniversalAuthenticationManager.TenantIdMap[config.Tenant] : config.Tenant;
      return FormatCacheKey(config.Resource, tenant, userName, config.Claims);
    }
  }
}
