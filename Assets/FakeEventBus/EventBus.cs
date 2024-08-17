using System;
using System.Collections.Generic;
using System.Reflection;

namespace FakeEventBus
{
    public class EventBus : IEventBus
    {
        private const BindingFlags k_Binding = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        private readonly List<object> m_Observers;
        private readonly Dictionary<Type, object> m_EventBindings; // Type is event type inhereted from EventArgs
        private readonly Dictionary<Type, MethodInfo> m_ObserversCache;

        public EventBus()
        {
            m_Observers = new List<object>();
            m_EventBindings = new Dictionary<Type, object>();
            m_ObserversCache = new Dictionary<Type, MethodInfo>();
        }

        public int ActiveObserverCount => m_Observers.Count;

        public void Register(object observer)
        {
            var allMethodInfo = observer.GetType().GetMethods(k_Binding);
            var isObserver = false;

            foreach (var methodInfo in allMethodInfo)
            {
                if (IsValidCallback(methodInfo))
                {
                    isObserver = true;

                    var eventArgsType = methodInfo.GetParameters()[0].ParameterType;
                    _ = m_EventBindings.TryAdd(eventArgsType, observer);
                    _ = m_ObserversCache.TryAdd(observer.GetType(), methodInfo);
                }
            }

            if (isObserver)
            {
                m_Observers.Add(observer);
            }
        }

        public void Unregister(object observer)
        {
            m_Observers.Remove(observer);
        }

        public void Notify<T>(T eventArgs)
            where T : EventArgs
        {
            if (m_EventBindings.TryGetValue(typeof(T), out var observer))
            {
                if (m_ObserversCache.TryGetValue(observer.GetType(), out var callback))
                {
                    callback.Invoke(observer, new object[] { eventArgs });
                }
            }
        }

        private bool IsValidCallback(MethodInfo methodInfo)
        {
            var attributes = methodInfo.GetCustomAttribute<ObserveEventAttribute>();

            if (attributes is not null)
            {
                return HasValidParameter(methodInfo);
            }

            return false;
        }

        private bool HasValidParameter(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 1 && parameters[0].ParameterType.IsSubclassOf(typeof(EventArgs)))
            {
                return true;
            }

            throw new InvalidCallbackException($"The callback {methodInfo.Name} of {methodInfo.DeclaringType.Name} must contain only one parameter inheriting from the {typeof(EventArgs).Name} type");
        }
    }
}
