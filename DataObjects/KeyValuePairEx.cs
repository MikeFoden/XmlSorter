// -----------------------------------------------------------------------
// <copyright file="KeyValuePairEx.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace XmlSorter.DataObjects
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public class KeyValuePairEx<TKey, TValue> : INotifyPropertyChanged, ICloneable
    {
        private TKey key;
        private TValue value;
        public KeyValuePairEx(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
        
        public TKey Key
        {
            get
            {
                return this.key;
            }
        }
        public TValue Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if(!this.value.Equals(value))
                {
                    this.value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            if (this.Key != null)
            {
                builder.Append(this.Key.ToString());
            }
            builder.Append(", ");
            if (this.Value != null)
            {
                builder.Append(this.Value.ToString());
            }
            builder.Append(']');
            return builder.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String PropertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public object Clone()
        {
            return new KeyValuePairEx<TKey, TValue>(Key, Value);
        }
    }
}
