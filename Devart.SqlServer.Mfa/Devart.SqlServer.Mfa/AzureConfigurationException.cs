using System;

namespace Devart.SqlServer.Mfa {

  internal class AzureConfigurationException : Exception {

    public AzureConfigurationException(string message) : base(message) {
    }
  }
}
