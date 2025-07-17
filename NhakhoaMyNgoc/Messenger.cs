using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc
{
    public static class Messenger
    {
        private static readonly Dictionary<string, List<Action<object>>> _subscriptions = new();

        public static void Subscribe(string key, Action<object> callback)
        {
            if (!_subscriptions.ContainsKey(key))
                _subscriptions[key] = [];

            _subscriptions[key].Add(callback);
        }

        public static void Unsubscribe(string key, Action<object> callback)
        {
            if (_subscriptions.TryGetValue(key, out var list))
            {
                list.Remove(callback);
                if (list.Count == 0)
                    _subscriptions.Remove(key);
            }
        }

        public static void Publish(string key, params object[] data)
        {
            if (_subscriptions.TryGetValue(key, out var callbacks))
            {
                foreach (var callback in callbacks.ToList()) // tránh lỗi collection bị sửa trong khi duyệt
                {
                    callback?.Invoke(data);
                }
            }
        }
    }
}
