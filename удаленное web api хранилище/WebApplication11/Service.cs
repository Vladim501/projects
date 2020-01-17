using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication11
{
    public class KeyValueClass
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    public interface IKeyValueService
    {
        void AddEntry(string key, string value);

        KeyValueClass GetEntry(string key);

        void RemoveEntry(string key);
    }

    public class KeyValueService : IKeyValueService
    {
        private ConcurrentDictionary<string, string> entries = new ConcurrentDictionary<string, string>();
        public ConcurrentDictionary<string, string> StorageEntries
        {
            get => entries;
            private set { }
        }


        public void AddEntry(string key, string value)
        {
            entries.AddOrUpdate(key, value, (thisKey, oldValue) => oldValue != value ? value : oldValue);
        }

        public KeyValueClass GetEntry(string key)
        {
            return entries.TryGetValue(key, out var value) ? new KeyValueClass { Key = key, Value = value } : null;
        }

        public void RemoveEntry(string key)
        {
            entries.TryRemove(key, out var value);
        }

    }

}
