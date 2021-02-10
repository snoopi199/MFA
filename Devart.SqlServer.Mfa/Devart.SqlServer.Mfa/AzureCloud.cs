using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Devart.SqlServer.Mfa {

  internal class AzureCloud : CommonGroupBase, IAzureCloudConfiguration {

    private const string ACTIVEDIRECTORYAUTHORITY_PROPERTY_KEY = "ActiveDirectoryAuthority";
    private const string ACTIVEDIRECTORYSERVICEENDPOINTRESOURCEID_PROPERTY_KEY = "ActiveDirectoryServiceEndpointResourceId";
    private const string ADTENANT_PROPERTY_KEY = "AdTenant";
    private const string AZUREDATAFACTORYPORTALURL_PROPERTY_KEY = "AzureDataFactoryPortalUrl";
    private const string AZUREDATALAKEANALYTICSCATALOGANDJOBENDPOINTSUFFIX_PROPERTY_KEY = "AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix";
    private const string AZUREDATALAKESTOREFILESYSTEMENDPOINTSUFFIX_PROPERTY_KEY = "AzureDataLakeStoreFileSystemEndpointSuffix";
    private const string AZUREKEYVAULTDNSSUFFIX_PROPERTY_KEY = "AzureKeyVaultDnsSuffix";
    private const string AZUREKEYVAULTSERVICEENDPOINTRESOURCEID_PROPERTY_KEY = "AzureKeyVaultServiceEndpointResourceId";
    private Dictionary<string, object> customValues;
    private const string GALLERYURL_PROPERTY_KEY = "GalleryUrl";
    private const string GRAPHENDPOINTRESOURCEID_PROPERTY_KEY = "GraphEndpointResourceId";
    private const string GRAPHURL_PROPERTY_KEY = "GraphUrl";
    private AzureCloudName? lastName;
    private const string MANAGEMENTPORTALURL_PROPERTY_KEY = "ManagementPortalUrl";
    private AzureCloudName name;
    private static IList<string> nonCustomProperties = new string[] { "AdTenant", "Name", "AdalLogLevel", "EnableConditionalAccess" };
    private const string PUBLISHSETTINGSFILEURL_PROPERTY_KEY = "PublishSettingsFileUrl";
    private static readonly System.Resources.ResourceManager ResourceManager = new System.Resources.ResourceManager(typeof(AzureCloud));
    private const string RESOURCEMANAGERURL_PROPERTY_KEY = "ResourceManagerUrl";
    private const string SERVICEMANAGEMENTURL_PROPERTY_KEY = "ServiceManagementUrl";
    private const string SQLDATABASEDNSSUFFIX_PROPERTY_KEY = "SqlDatabaseDnsSuffix";
    private const string SQLDATABASESERVICEPRINCIPALNAME_PROPERTY_KEY = "SqlDatabaseServicePrincipalName";
    private const string STORAGEENDPOINTSUFFIX_PROPERTY_KEY = "StorageEndpointSuffix";
    private const string TRAFFICMANAGERDNSSUFFIX_PROPERTY_KEY = "TrafficManagerDnsSuffix";

    private class NationalCloud : IAzureCloudConfiguration {

      public string ActiveDirectoryAuthority { get; set; }

      public string ActiveDirectoryServiceEndpointResourceId { get; set; }

      public string AdTenant { get; set; }

      public string AzureDataFactoryPortalUrl { get; set; }

      public string AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix { get; set; }

      public string AzureDataLakeStoreFileSystemEndpointSuffix { get; set; }

      public string AzureKeyVaultDnsSuffix { get; set; }

      public string AzureKeyVaultServiceEndpointResourceId { get; set; }

      public string GalleryUrl { get; set; }

      public string GraphEndpointResourceId { get; set; }

      public string GraphUrl { get; set; }

      public string ManagementPortalUrl { get; set; }

      public string PublishSettingsFileUrl { get; set; }

      public string ResourceManagerUrl { get; set; }

      public string ServiceManagementUrl { get; set; }

      public string SqlDatabaseDnsSuffix { get; set; }

      public string SqlDatabaseServicePrincipalName { get; set; }

      public string StorageEndpointSuffix { get; set; }

      public string TrafficManagerDnsSuffix { get; set; }
    }

    public static IAzureCloudConfiguration GetAzureCloudConfiguration(string azureDatabaseAddress, bool defaultToCurrentSettings) {

      string azureDatabaseAddressLowerCase = azureDatabaseAddress.ToLower(CultureInfo.InvariantCulture);
      if(azureDatabaseAddressLowerCase.EndsWith("database.windows.net"))
        return LoadNationalCloudValues(AzureCloudName.AzureCloud);
      else if(azureDatabaseAddressLowerCase.EndsWith("database.chinacloudapi.cn"))
        return LoadNationalCloudValues(AzureCloudName.AzureChinaCloud);
      else if (azureDatabaseAddressLowerCase.EndsWith("database.usgovcloudapi.net"))
        return LoadNationalCloudValues(AzureCloudName.AzureUsGovernment);
      else if (azureDatabaseAddressLowerCase.EndsWith("database.cloudapi.de"))
        return LoadNationalCloudValues(AzureCloudName.AzureGermanCloud);

      throw new AzureConfigurationException($"Unknown Azure cloud server {azureDatabaseAddress}.");
    }

    public TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {

      if (context.PropertyDescriptor.Name == "Name")
        return new TypeConverter.StandardValuesCollection(Enum.GetValues(typeof(AzureCloudName)));
      if (context.PropertyDescriptor.Name == "AdalLogLevel")
        return new TypeConverter.StandardValuesCollection(Enum.GetValues(typeof(AdalLogLevelValues)));
      return null;
    }

    private void LoadCloudValues() {

      IAzureCloudConfiguration configuration = LoadNationalCloudValues(this.Name);
      this.ActiveDirectoryServiceEndpointResourceId = configuration.ActiveDirectoryServiceEndpointResourceId;
      this.GalleryUrl = configuration.GalleryUrl;
      this.ManagementPortalUrl = configuration.ManagementPortalUrl;
      this.ServiceManagementUrl = configuration.ServiceManagementUrl;
      this.PublishSettingsFileUrl = configuration.PublishSettingsFileUrl;
      this.ResourceManagerUrl = configuration.ResourceManagerUrl;
      this.SqlDatabaseDnsSuffix = configuration.SqlDatabaseDnsSuffix;
      this.StorageEndpointSuffix = configuration.StorageEndpointSuffix;
      this.ActiveDirectoryAuthority = configuration.ActiveDirectoryAuthority;
      this.GraphUrl = configuration.GraphUrl;
      this.GraphEndpointResourceId = configuration.GraphEndpointResourceId;
      this.TrafficManagerDnsSuffix = configuration.TrafficManagerDnsSuffix;
      this.AzureKeyVaultDnsSuffix = configuration.AzureKeyVaultDnsSuffix;
      this.AzureDataLakeStoreFileSystemEndpointSuffix = configuration.AzureDataLakeStoreFileSystemEndpointSuffix;
      this.AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix = configuration.AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix;
      this.AzureKeyVaultServiceEndpointResourceId = configuration.AzureKeyVaultServiceEndpointResourceId;
      this.SqlDatabaseServicePrincipalName = configuration.SqlDatabaseServicePrincipalName;
      this.AzureDataFactoryPortalUrl = configuration.AzureDataFactoryPortalUrl;
    }

    private static IAzureCloudConfiguration LoadNationalCloudValues(AzureCloudName name) {

      NationalCloud cloud = new NationalCloud();
      switch (name) {
        case AzureCloudName.AzureCloud:
          cloud.ActiveDirectoryServiceEndpointResourceId = "https://management.core.windows.net/";
          cloud.GalleryUrl = "https://gallery.azure.com/";
          cloud.ManagementPortalUrl = "http://portal.azure.com";
          cloud.ServiceManagementUrl = "https://management.core.windows.net/";
          cloud.PublishSettingsFileUrl = "http://go.microsoft.com/fwlink/?LinkID=335839";
          cloud.ResourceManagerUrl = "https://management.azure.com/";
          cloud.SqlDatabaseDnsSuffix = ".database.windows.net";
          cloud.StorageEndpointSuffix = "core.windows.net";
          cloud.ActiveDirectoryAuthority = "https://login.microsoftonline.com/";
          cloud.GraphUrl = "https://graph.windows.net/";
          cloud.GraphEndpointResourceId = "https://graph.windows.net/";
          cloud.TrafficManagerDnsSuffix = "trafficmanager.net";
          cloud.AzureKeyVaultDnsSuffix = "vault.azure.net";
          cloud.AzureDataLakeStoreFileSystemEndpointSuffix = "azuredatalakestore.net";
          cloud.AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix = "azuredatalakeanalytics.net";
          cloud.AzureKeyVaultServiceEndpointResourceId = "https://vault.azure.net";
          cloud.SqlDatabaseServicePrincipalName = "https://database.windows.net/";
          cloud.AzureDataFactoryPortalUrl = "https://adf.azure.com/";
          return cloud;

        case AzureCloudName.AzureGermanCloud:
          cloud.ActiveDirectoryServiceEndpointResourceId = "https://management.core.cloudapi.de/";
          cloud.GalleryUrl = "https://gallery.cloudapi.de/";
          cloud.ManagementPortalUrl = "http://portal.microsoftazure.de";
          cloud.ServiceManagementUrl = "https://management.core.cloudapi.de/";
          cloud.PublishSettingsFileUrl = "https://manage.microsoftazure.de/publishsettings/index";
          cloud.ResourceManagerUrl = "https://management.microsoftazure.de/";
          cloud.SqlDatabaseDnsSuffix = ".database.cloudapi.de";
          cloud.StorageEndpointSuffix = "core.cloudapi.de";
          cloud.ActiveDirectoryAuthority = "https://login.microsoftonline.de/";
          cloud.GraphUrl = "https://graph.cloudapi.de/";
          cloud.GraphEndpointResourceId = "https://graph.cloudapi.de/";
          cloud.TrafficManagerDnsSuffix = "azuretrafficmanager.de";
          cloud.AzureKeyVaultDnsSuffix = "vault.microsoftazure.de";
          cloud.AzureDataLakeStoreFileSystemEndpointSuffix = string.Empty;
          cloud.AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix = string.Empty;
          cloud.AzureKeyVaultServiceEndpointResourceId = "https://vault.microsoftazure.de";
          cloud.SqlDatabaseServicePrincipalName = "https://database.cloudapi.de/";
          return cloud;

        case AzureCloudName.AzureUsGovernment:
          cloud.ActiveDirectoryServiceEndpointResourceId = "https://management.core.usgovcloudapi.net/";
          cloud.GalleryUrl = "https://gallery.usgovcloudapi.net/";
          cloud.ManagementPortalUrl = "https://portal.azure.us";
          cloud.ServiceManagementUrl = "https://management.core.usgovcloudapi.net/";
          cloud.PublishSettingsFileUrl = "https://manage.windowsazure.us/publishsettings/index";
          cloud.ResourceManagerUrl = "https://management.usgovcloudapi.net/";
          cloud.SqlDatabaseDnsSuffix = ".database.usgovcloudapi.net";
          cloud.StorageEndpointSuffix = "core.usgovcloudapi.net";
          cloud.ActiveDirectoryAuthority = "https://login.microsoftonline.us/";
          cloud.GraphUrl = "https://graph.windows.net/";
          cloud.GraphEndpointResourceId = "https://graph.windows.net/";
          cloud.TrafficManagerDnsSuffix = "usgovtrafficmanager.net";
          cloud.AzureKeyVaultDnsSuffix = "vault.usgovcloudapi.net";
          cloud.AzureDataLakeStoreFileSystemEndpointSuffix = string.Empty;
          cloud.AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix = string.Empty;
          cloud.AzureKeyVaultServiceEndpointResourceId = "https://vault.usgovcloudapi.net";
          cloud.SqlDatabaseServicePrincipalName = "https://database.usgovcloudapi.net/";
          return cloud;

        case AzureCloudName.AzureChinaCloud:
          cloud.ActiveDirectoryServiceEndpointResourceId = "https://management.core.chinacloudapi.cn/";
          cloud.GalleryUrl = "https://gallery.chinacloudapi.cn/";
          cloud.ManagementPortalUrl = "http://portal.azure.cn";
          cloud.ServiceManagementUrl = "https://management.core.chinacloudapi.cn/";
          cloud.PublishSettingsFileUrl = "http://go.microsoft.com/fwlink/?LinkID=301776";
          cloud.ResourceManagerUrl = "https://management.chinacloudapi.cn/";
          cloud.SqlDatabaseDnsSuffix = ".database.chinacloudapi.cn";
          cloud.StorageEndpointSuffix = "core.chinacloudapi.cn";
          cloud.ActiveDirectoryAuthority = "https://login.chinacloudapi.cn/";
          cloud.GraphUrl = "https://graph.chinacloudapi.cn/";
          cloud.GraphEndpointResourceId = "https://graph.chinacloudapi.cn/";
          cloud.TrafficManagerDnsSuffix = "trafficmanager.cn";
          cloud.AzureKeyVaultDnsSuffix = "vault.azure.cn";
          cloud.AzureDataLakeStoreFileSystemEndpointSuffix = string.Empty;
          cloud.AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix = string.Empty;
          cloud.AzureKeyVaultServiceEndpointResourceId = "https://vault.azure.cn";
          cloud.SqlDatabaseServicePrincipalName = "https://database.chinacloudapi.cn/";
          return cloud;
      }
      throw new InvalidOperationException(ResourceManager.GetString("error.UnknownCloud", CultureInfo.CurrentUICulture));
    }

    public virtual string ActiveDirectoryAuthority {
      get => base.GetValue<string>("ActiveDirectoryAuthority", "https://login.microsoftonline.com/");
      set => base.SetValue<string>("ActiveDirectoryAuthority", value);
    }

    public virtual string ActiveDirectoryServiceEndpointResourceId {
      get => base.GetValue<string>("ActiveDirectoryServiceEndpointResourceId", "https://management.core.windows.net/");
      set => base.SetValue<string>("ActiveDirectoryServiceEndpointResourceId", value);
    }

    public AdalLogLevelValues AdalLogLevel { get; set; }

    public virtual string AdTenant {
      get => base.GetValue<string>("AdTenant", "");
      set => base.SetValue<string>("AdTenant", value);
    }

    public virtual string AzureDataFactoryPortalUrl {
      get => base.GetValue<string>("AzureDataFactoryPortalUrl", "https://adf.azure.com/");
      set => base.SetValue<string>("AzureDataFactoryPortalUrl", value);
    }

    public virtual string AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix {
      get => base.GetValue<string>("AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix", "azuredatalakeanalytics.net");
      set => base.SetValue<string>("AzureDataLakeAnalyticsCatalogAndJobEndpointSuffix", value);
    }

    public virtual string AzureDataLakeStoreFileSystemEndpointSuffix {
      get => base.GetValue<string>("AzureDataLakeStoreFileSystemEndpointSuffix", "azuredatalakestore.net");
      set => base.SetValue<string>("AzureDataLakeStoreFileSystemEndpointSuffix", value);
    }

    public virtual string AzureKeyVaultDnsSuffix {
      get => base.GetValue<string>("AzureKeyVaultDnsSuffix", "vault.azure.net");
      set => base.SetValue<string>("AzureKeyVaultDnsSuffix", value);
    }

    public virtual string AzureKeyVaultServiceEndpointResourceId {
      get => base.GetValue<string>("AzureKeyVaultServiceEndpointResourceId", "https://vault.azure.net");
      set => base.SetValue<string>("AzureKeyVaultServiceEndpointResourceId", value);
    }

    private IDictionary<string, object> CustomValues => (this.customValues ?? (this.customValues = new Dictionary<string, object>()));

    public bool EnableConditionalAccess { get; set; }

    public virtual string GalleryUrl {
      get => base.GetValue<string>("GalleryUrl", "https://gallery.azure.com/");
      set => base.SetValue<string>("GalleryUrl", value);
    }

    public virtual string GraphEndpointResourceId {
      get => base.GetValue<string>("GraphEndpointResourceId", "https://graph.windows.net/");
      set => base.SetValue<string>("GraphEndpointResourceId", value);
    }

    public virtual string GraphUrl {
      get => base.GetValue<string>("GraphUrl", "https://graph.windows.net/");
      set => base.SetValue<string>("GraphUrl", value);
    }

    public virtual string ManagementPortalUrl {
      get => base.GetValue<string>("ManagementPortalUrl", "http://portal.azure.com");
      set => base.SetValue<string>("ManagementPortalUrl", value);
    }

    public AzureCloudName Name {
      get => this.name;
      set {
        bool flag = (!this.lastName.HasValue || ((this.name == AzureCloudName.Custom) && (value != AzureCloudName.Custom))) || ((this.name != AzureCloudName.Custom) && (value == AzureCloudName.Custom));
        bool flag2 = false;
        if (this.name != value) {
          this.lastName = new AzureCloudName?(this.name);
          this.name = value;
          flag2 = this.name != AzureCloudName.Custom;
        }
        //if (flag && (this.ReadOnlyPropertyChanged != null)) {
        //  this.ReadOnlyPropertyChanged(this, new ReadOnlyPropertyChangedEventArgs(string.Empty));
        //}
        if (flag2) {
          this.LoadCloudValues();
        }
      }
    }

    public virtual string PublishSettingsFileUrl {
      get => base.GetValue<string>("PublishSettingsFileUrl", "http://go.microsoft.com/fwlink/?LinkID=335839");
      set => base.SetValue<string>("PublishSettingsFileUrl", value);
    }

    public virtual string ResourceManagerUrl {
      get => base.GetValue<string>("ResourceManagerUrl", "https://management.azure.com/");
      set => base.SetValue<string>("ResourceManagerUrl", value);
    }

    public virtual string ServiceManagementUrl {
      get => base.GetValue<string>("ServiceManagementUrl", "https://management.core.windows.net/");
      set => base.SetValue<string>("ServiceManagementUrl", value);
    }

    public virtual string SqlDatabaseDnsSuffix {
      get => base.GetValue<string>("SqlDatabaseDnsSuffix", ".database.windows.net");
      set => base.SetValue<string>("SqlDatabaseDnsSuffix", value);
    }

    public virtual string SqlDatabaseServicePrincipalName {
      get => base.GetValue<string>("SqlDatabaseServicePrincipalName", "https://database.windows.net/");
      set => base.SetValue<string>("SqlDatabaseServicePrincipalName", value);
    }

    public virtual string StorageEndpointSuffix {
      get => base.GetValue<string>("StorageEndpointSuffix", "core.windows.net");
      set => base.SetValue<string>("StorageEndpointSuffix", value);
    }

    public virtual string TrafficManagerDnsSuffix {
      get => base.GetValue<string>("TrafficManagerDnsSuffix", "trafficmanager.net");
      set => base.SetValue<string>("TrafficManagerDnsSuffix", value);
    }
  }
}
