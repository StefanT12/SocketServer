using Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Dispatcher : IDispatcher, IObjectListener
    {
        private class ListenerMap
        {
            public IList<object> Listeners = new List<object>();
            public MethodInfo Method { get; set; }
        }

        private readonly IDictionary<Type, ListenerMap> _listenerMaps = new Dictionary<Type, ListenerMap>();

        public Dispatcher(params IMarshaller[] marshallers)
        {
            foreach (var m in marshallers)
            {
                m.AddListener(this);
            }
        }

        public void AddListener<T>(IGenericListener<T> listener)
        {
            var genericType = typeof(T);
            if (!_listenerMaps.ContainsKey(genericType))
            {
                _listenerMaps.Add(genericType, new ListenerMap
                {
                    Method = typeof(IGenericListener<T>).GetMethod(nameof(IGenericListener<T>.ReceiveObject))
                });
            }
            _listenerMaps[genericType].Listeners.Add(listener);
        }

        public void ReceiveObject(object obj)
        {
            var t = obj.GetType();

            ListenerMap listenerMap;

            if (_listenerMaps.TryGetValue(t, out listenerMap))
            {
                foreach (var listener in listenerMap.Listeners)
                {
                    listenerMap.Method.Invoke(listener, new object[] { obj });
                }
            }
        }
    }
}
