using System;
using System.Collections.Generic;
using System.Reflection;

namespace FakeEventBus
{
    public class EventBus : IEventBus
    {
        private class ObserverBindings
        {
            private readonly Type m_EventArgsType;
            private readonly List<Delegate> m_Callbacks;

            public ObserverBindings(Type eventArgsType)
            {
                m_EventArgsType = eventArgsType;
                m_Callbacks = new List<Delegate>();
            }

            public int CallbackCount => m_Callbacks.Count;

            public void Add(Delegate callback)
            {
                m_Callbacks.Add(callback);
            }

            public void Remove(object observer)
            {
                m_Callbacks.RemoveAll(callback => callback.Target == observer);
            }

            public void Invoke<T>(T args)
                where T : EventArgs
            {
                m_Callbacks.ForEach(callback => ((Action<T>)callback).Invoke(args));
            }
        }

        private const BindingFlags k_Binding = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        private readonly Dictionary<Type, ObserverBindings> m_ObserverBindings; // Type is event args type

        public EventBus()
        {
            m_ObserverBindings = new Dictionary<Type, ObserverBindings>();
        }

        public int GetActiveObserverCount<T>()
            where T : EventArgs
        {
            if (m_ObserverBindings.TryGetValue(typeof(T), out var bindings))
            {
                return bindings.CallbackCount;
            }

            return 0;
        }

        public void Register(object observer)
        {
            var methods = observer.GetType().GetMethods(k_Binding);

            foreach (var methodInfo in methods)
            {
                if (TryGetEventArgsType(methodInfo, out Type eventArgsType))
                {
                    AddCallback(methodInfo, eventArgsType, observer);
                }
            }
        }

        public void Unregister(object observer)
        {
            foreach (var binding in m_ObserverBindings)
            {
                binding.Value.Remove(observer);
            }
        }

        public void Notify<T>(T eventArgs)
            where T : EventArgs
        {
            if (m_ObserverBindings.TryGetValue(typeof(T), out var bindings))
            {
                bindings.Invoke(eventArgs);
            }
        }

        private bool TryGetEventArgsType(MethodInfo methodInfo, out Type eventArgsType)
        {
            if (HasValidAttribute(methodInfo))
            {
                var parameters = methodInfo.GetParameters();

                if (parameters.Length == 1 && typeof(EventArgs).IsAssignableFrom(parameters[0].ParameterType))
                {
                    eventArgsType = parameters[0].ParameterType;
                    return true;
                }
                else
                {
                    throw new InvalidCallbackException($"The callback {methodInfo.Name} of {methodInfo.DeclaringType.Name} must contain only one parameter inheriting from the {typeof(EventArgs).Name} type");
                }
            }

            eventArgsType = default;
            return false;
        }

        private bool HasValidAttribute(MethodInfo methodInfo)
        {
            var attributes = methodInfo.GetCustomAttribute<ObserveEventAttribute>();

            return attributes is not null;
        }

        private void AddCallback(MethodInfo methodInfo, Type eventArgsType, object observer)
        {
            var callback = CreateDelegate(methodInfo, eventArgsType, observer);

            if (m_ObserverBindings.TryGetValue(eventArgsType, out var observerBindings))
            {
                observerBindings.Add(callback);
            }
            else
            {
                var observerBinding = new ObserverBindings(eventArgsType);
                observerBinding.Add(callback);
                m_ObserverBindings[eventArgsType] = observerBinding;
            }
        }

        private Delegate CreateDelegate(MethodInfo methodInfo, Type genericType, object target)
        {
            var delegateType = typeof(Action<>).MakeGenericType(genericType);

            return methodInfo.CreateDelegate(delegateType, target);
        }
    }
}
