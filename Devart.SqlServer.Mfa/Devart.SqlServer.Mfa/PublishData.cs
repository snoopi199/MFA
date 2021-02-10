using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Devart.SqlServer.Mfa {

  [Serializable, XmlType(AnonymousType = true), XmlRoot(Namespace = "", IsNullable = false), GeneratedCode("xsd", "4.0.30319.17914"), DebuggerStepThrough, DesignerCategory("code")]
  internal class PublishData {

    private PublishDataPublishProfile[] itemsField;

    [XmlElement("PublishProfile", Form = XmlSchemaForm.Unqualified)]
    public PublishDataPublishProfile[] Items {
      get => this.itemsField;
      set => this.itemsField = value;
    }
  }

  [Serializable, GeneratedCode("xsd", "4.0.30319.17914"), DesignerCategory("code"), XmlType(AnonymousType = true), DebuggerStepThrough]
  internal class PublishDataPublishProfile {

    [XmlAttribute]
    public string PublishMethod {
      get;
      set;
    }

    [XmlAttribute]
    public string SchemaVersion {
      get;
      set;
    }

    [XmlElement("Subscription", Form = XmlSchemaForm.Unqualified)]
    public PublishDataPublishProfileSubscription[] Subscription {
      get;
      set;
    }
  }

  [Serializable, DesignerCategory("code"), XmlType(AnonymousType = true), GeneratedCode("xsd", "4.0.30319.17914"), DebuggerStepThrough]
  internal class PublishDataPublishProfileSubscription {

    [XmlAttribute]
    public string Id {
      get;
      set;
    }

    [XmlAttribute]
    public string ManagementCertificate {
      get;
      set;
    }

    [XmlAttribute]
    public string Name {
      get;
      set;
    }

    [XmlAttribute]
    public string ServiceManagementUrl {
      get;
      set;
    }
  }
}