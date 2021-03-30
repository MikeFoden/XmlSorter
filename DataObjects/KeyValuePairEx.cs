using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace XmlSorter.DataObjects
{
  /// <author>Originally created by Abdulhamed Shalaby</author>
  [Serializable, StructLayout(LayoutKind.Sequential)]
    public class KeyValuePairEx<TKey, TValue> : INotifyPropertyChanged, ICloneable
    {
      private TValue value;
        public KeyValuePairEx(TKey key, TValue value)
        {
            Key = key;
            this.value = value;
        }

        public TKey Key { get; }

        public TValue Value
        {
            get => value;
            set
            {
              if (this.value.Equals(value)) { return; }
              this.value = value;
                NotifyPropertyChanged("Value");
            }
        }
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append('[');
            if (Key != null)
            {
                builder.Append(Key);
            }
            builder.Append(", ");
            if (Value != null)
            {
                builder.Append(Value);
            }
            builder.Append(']');
            return builder.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone() => new KeyValuePairEx<TKey, TValue>(Key, Value);
    }
}
