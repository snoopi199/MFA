namespace Devart.SqlServer.Mfa {

  internal enum AzureCloudName {
    AzureChinaCloud = 3,
    AzureCloud = 0,
    AzureGermanCloud = 1,
    AzureUsGovernment = 2,
    Custom = 4
  }

  internal enum AdalLogLevelValues {
    Error = 1,
    Information = 3,
    None = 0,
    Verbose = 4,
    Warning = 2
  }

  internal enum AzureAuthenticationType {
    CertificateBasedAuthentication,
    ActiveDirectoryAuthentication
  }

  internal enum LoginPromptBehavior {
    Auto,
    Always,
    Never,
    RefreshSession,
    Default
  }

  internal enum StoreLocation {
    CurrentUser = 1,
    LocalMachine = 2
  }

  internal enum StoreName {
    AddressBook = 1,
    AuthRoot = 2,
    CertificateAuthority = 3,
    Disallowed = 4,
    My = 5,
    Root = 6,
    TrustedPeople = 7,
    TrustedPublisher = 8
  }

  internal interface IAzureCloudConfiguration {

    string ActiveDirectoryAuthority { get; }

    string ActiveDirectoryServiceEndpointResourceId { get; }

    string AdTenant { get; }

    string AzureDataFactoryPortalUrl { get; }

    string AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix { get; }

    string AzureDataLakeStoreFileSystemEndpointSuffix { get; }

    string AzureKeyVaultDnsSuffix { get; }

    string AzureKeyVaultServiceEndpointResourceId { get; }

    string GalleryUrl { get; }

    string GraphEndpointResourceId { get; }

    string GraphUrl { get; }

    string ManagementPortalUrl { get; }

    string PublishSettingsFileUrl { get; }

    string ResourceManagerUrl { get; }

    string ServiceManagementUrl { get; }

    string SqlDatabaseDnsSuffix { get; }

    string SqlDatabaseServicePrincipalName { get; }

    string StorageEndpointSuffix { get; }

    string TrafficManagerDnsSuffix { get; }
  }
}
