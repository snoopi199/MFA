using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Devart.SqlServer.Mfa {

  public class AuthenticationProvider : SqlAuthenticationProvider {

    public override bool IsSupported(SqlAuthenticationMethod authenticationMethod)
      => authenticationMethod == SqlAuthenticationMethod.ActiveDirectoryInteractive;

    public override Task<SqlAuthenticationToken> AcquireTokenAsync(SqlAuthenticationParameters parameters) {

      RenewableToken renewableToken = new RenewableToken(parameters);
      return Task.Run<SqlAuthenticationToken>(() => this.GetToken(renewableToken));
    }

    public static void Initialize() {

      var provider = SqlAuthenticationProvider.GetProvider(SqlAuthenticationMethod.ActiveDirectoryInteractive);
      if (provider == null)
        SqlAuthenticationProvider.SetProvider(SqlAuthenticationMethod.ActiveDirectoryInteractive, new AuthenticationProvider());
    }

    private SqlAuthenticationToken GetToken(RenewableToken renewableToken)
      => new SqlAuthenticationToken(renewableToken.GetAccessToken(), renewableToken.TokenExpiry);
  }
}
