// -----------------------------------------------------------------------
// <copyright file="AttributesBinding.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace XmlSorter.DataObjects
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AttributesBinding : ObservableCollection<KeyValuePairEx<string, bool>>
    {
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        public void Add(string Key)
        {
            Add(Key, false);
        }
        public void Add(string Key, bool Value)
        {
            AddEx(new KeyValuePairEx<string, bool>(Key, Value));
        }
        public void AddEx(KeyValuePairEx<string, bool> KeyValuePairExInstance)
        {
            if(SynchronizationContext.Current == _synchronizationContext)
            {
                Add(KeyValuePairExInstance);
            }
            else
            {
                _synchronizationContext.Post(AddCore, KeyValuePairExInstance);
            }
        }
        private void AddCore(object param)
        {
            KeyValuePairEx<string, bool> KeyValuePairExInstance = (KeyValuePairEx<string, bool>)param;
            if(!ContainsKey(KeyValuePairExInstance.Key))
            {
                Add(KeyValuePairExInstance);
            }
        }
        public bool ContainsKey(string Key)
        {
            return this.Where(P => P.Key == Key).Count() > 0;
        }
        public IEnumerable<string> GetSelected()
        {
            return this.Where(P => P.Value).Select(K => K.Key);
        }
    }
}
