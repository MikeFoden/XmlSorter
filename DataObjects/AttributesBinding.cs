using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace XmlSorter.DataObjects
{
  public class AttributesBinding : ObservableCollection<KeyValuePairEx<string, bool>>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        public void Add(string key)
        {
            Add(key, false);
        }
        public void Add(string key, bool value)
        {
            AddEx(new KeyValuePairEx<string, bool>(key, value));
        }
        public void AddEx(KeyValuePairEx<string, bool> keyValuePairExInstance)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                Add(keyValuePairExInstance);
            }
            else
            {
                _synchronizationContext.Post(AddCore, keyValuePairExInstance);
            }
        }
        private void AddCore(object param)
        {
            var keyValuePairExInstance = (KeyValuePairEx<string, bool>)param;
            if (!ContainsKey(keyValuePairExInstance.Key))
            {
                Add(keyValuePairExInstance);
            }
        }
        public bool ContainsKey(string key)
        {
            return this.Any(p => p.Key == key);
        }
        public IEnumerable<string> GetSelected()
        {
            return this.Where(p => p.Value).Select(k => k.Key);
        }
    }
}
