using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security;

namespace Devart.SqlServer.Mfa {

  using System;
  using System.Data.SqlClient;
  using System.Diagnostics;
  using System.Threading;
  using System.Threading.Tasks;

  [DebuggerDisplay("{Resource} {UserId} {Tenant} {TokenExpiry} - {lastTokenHash}")]
  internal class RenewableToken {

    private readonly ActiveDirectoryUniversalAuthenticationManager authManager;
    private int lastTokenHash;
    private static readonly object refreshSyncObj = new object();

    public RenewableToken(SqlAuthenticationParameters parameters) {

      this.authManager = new ActiveDirectoryUniversalAuthenticationManager(
        parameters.ServerName,
        parameters.UserId,
        parameters.Resource,
        parameters.Authority);
    }

    public static string GetTokenKey(string server, string user) => $"{server}:{user}";

    public string Resource => this.authManager.Resource;

    public string Tenant => this.authManager.TenantId;

    public DateTimeOffset TokenExpiry => this.authManager.TokenExpiry;

    public string Server => this.authManager.Server;

    public string UserId => this.authManager.UserId;

    public string GetAccessToken() => this.GetUniversalAccessToken(this.authManager);

    private string GetUniversalAccessToken(ActiveDirectoryUniversalAuthenticationManager manager) {

      string cachedAccessToken = manager.GetCachedAccessToken();
      Exception exception = null;
      if (cachedAccessToken == null) {
        lock (refreshSyncObj) {
          cachedAccessToken = manager.GetCachedAccessToken();
          if (cachedAccessToken == null) {
            using (CancellationTokenSource source = new CancellationTokenSource()) {
              using (StaTaskScheduler scheduler = new StaTaskScheduler(3)) {
                Task task = Task.Factory.StartNew(new Action(manager.Run), source.Token, TaskCreationOptions.None, scheduler);
                task.Wait(source.Token);
                if (task.IsFaulted) {
                  exception = task.Exception;
                }
              }
            }
            cachedAccessToken = manager.AccessToken;
            this.lastTokenHash = (cachedAccessToken == null) ? 0 : cachedAccessToken.GetHashCode();
          }
        }
      }
      if (exception != null) {
        throw exception;
      }
      return cachedAccessToken;
    }
  }
}
