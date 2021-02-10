using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Devart.SqlServer.Mfa {

  [Serializable]
  public class CommonGroupBase : INotifyPropertyChanged, IEditableObject, ICloneable {

    [OptionalField]
    private Dictionary<string, object> valueStorage = new Dictionary<string, object>();
    [OptionalField]
    private byte[] valueStorageBackup;
    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    public void BeginEdit() {

      if (this.valueStorageBackup == null) {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream serializationStream = new MemoryStream();
        formatter.Serialize(serializationStream, this.valueStorage);
        serializationStream.Close();
        this.valueStorageBackup = serializationStream.GetBuffer();
      }
    }

    public void CancelEdit() {

      if (this.valueStorageBackup != null) {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream serializationStream = new MemoryStream(this.valueStorageBackup);
        Dictionary<string, object> dictionary = formatter.Deserialize(serializationStream) as Dictionary<string, object>;
        serializationStream.Close();
        if (dictionary != null) {
          this.valueStorage = dictionary;
        }
        else {
          this.valueStorage = new Dictionary<string, object>();
        }
        this.valueStorageBackup = null;
      }
    }

    public object Clone() {

      BinaryFormatter formatter = new BinaryFormatter();
      MemoryStream serializationStream = new MemoryStream();
      formatter.Serialize(serializationStream, this.valueStorage);
      serializationStream.Close();
      MemoryStream stream2 = new MemoryStream(serializationStream.GetBuffer());
      Dictionary<string, object> dictionary = formatter.Deserialize(stream2) as Dictionary<string, object>;
      stream2.Close();
      CommonGroupBase base2 = Activator.CreateInstance(base.GetType()) as CommonGroupBase;
      base2.valueStorage = dictionary;
      return base2;
    }

    public void EndEdit() => this.valueStorageBackup = null;

    public bool HasProperty<T>(string propertyName) {

      object obj2 = null;
      return (this.valueStorage.TryGetValue(propertyName, out obj2) && (obj2 is T));
    }

    public void ResetAll() => this.valueStorage = new Dictionary<string, object>();

    public void ResetProperty(string propertyName) => this.valueStorage.Remove(propertyName);

    protected T GetValue<T>(string key, T defaultValue) {

      object obj2 = null;
      if (this.valueStorage.TryGetValue(key, out obj2) && (obj2 is T))
        return (T)obj2;
      return defaultValue;
    }

    protected void Initialize(CommonGroupBase newValues) {

      if (newValues == null)
        throw new ArgumentNullException("values");

      BinaryFormatter formatter = new BinaryFormatter();
      MemoryStream serializationStream = new MemoryStream();
      formatter.Serialize(serializationStream, newValues.valueStorage);
      serializationStream.Close();
      MemoryStream stream2 = new MemoryStream(serializationStream.GetBuffer());
      this.valueStorage = formatter.Deserialize(stream2) as Dictionary<string, object>;
      stream2.Close();
    }

    protected void InitializeValue<T>(string key, T value) => this.valueStorage[key] = value;

    protected virtual bool OnBeforePropertyChanged(string parameterName, object value) => true;

    protected virtual void OnPropertyChanged(string parameterName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parameterName));

    protected void SetValue<T>(string key, T value) {

      if (this.OnBeforePropertyChanged(key, value)) {
        this.valueStorage[key] = value;
        this.OnPropertyChanged(key);
      }
    }
  }
}
