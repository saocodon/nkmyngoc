using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc
{
    public static class Messenger
    {
        private static readonly Dictionary<string, Action<object>> _subscriptions = new();

        public static void Subscribe(string key, Action<object> callback) => _subscriptions[key] = callback;

        public static void Publish(string key, object data)
        {
            if (_subscriptions.TryGetValue(key, out var callback))
                callback?.Invoke(data);
        }
    }

}
