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

            public void Add(ObserverBindings bindings)
            {
                if (m_EventArgsType == bindings.m_EventArgsType)
                {
                    m_Callbacks.AddRange(bindings.m_Callbacks);
                }
            }

            public void Remove(object observer)
            {
                int index = -1;

                for (int i = 0; i < m_Callbacks.Count; i++)
                {
                    if (m_Callbacks[i].Target == observer)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    m_Callbacks[index] = m_Callbacks[^1];
					m_Callbacks.RemoveAt(m_Callbacks.Count - 1);
                }

                //m_Callbacks.RemoveAll(callback => callback.Target == observer);
			}

            public bool Contains(MethodInfo methodInfo, object observer)
            {
                foreach (var callback in m_Callbacks)
                {
                    if (callback.Method == methodInfo && callback.Target == observer)
                    {
                        return true;
                    }
                }

                return false;
            }

            public void Invoke<T>(T arg)
                where T : EventArgs
            {
				for (int i = 0; i < m_Callbacks.Count; i++)
                {
					((Action<T>)m_Callbacks[i]).Invoke(arg);
				}
            }
        }

        private const BindingFlags k_Scope =
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Instance;

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

        public void Clear()
        {
            m_ObserverBindings.Clear();
        }

        public void Register(object observer)
        {
            var generatedBindings = new Dictionary<Type, ObserverBindings>();

            Generate(observer.GetType(), observer, generatedBindings);

            foreach (var bindings in generatedBindings)
            {
                if (m_ObserverBindings.TryGetValue(bindings.Key, out var existedBindings))
                {
                    existedBindings.Add(bindings.Value);
                }
                else
                {
                    m_ObserverBindings[bindings.Key] = bindings.Value;
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

        private void Generate(Type type, object observer, Dictionary<Type, ObserverBindings> observerBindings)
        {
            var methods = type.GetMethods(k_Scope);

            foreach (var methodInfo in methods)
            {
                if (TryGetEventArgsType(methodInfo, out Type eventArgsType))
                {
                    if (m_ObserverBindings.TryGetValue(eventArgsType, out var bindings))
                    {
                        if (bindings.Contains(methodInfo, observer) is false)
                        {
                            AddCallback(methodInfo, eventArgsType, observer, observerBindings);
                        }
                    }
                    else
                    {
                        AddCallback(methodInfo, eventArgsType, observer, observerBindings);
                    }
                }
            }

            if (type.BaseType != null)
            {
                Generate(type.BaseType, observer, observerBindings);
            }
        }

        private bool TryGetEventArgsType(MethodInfo methodInfo, out Type eventArgsType)
        {
            if (HasValidAttribute(methodInfo))
            {
                if (HasValidParameter(methodInfo, out eventArgsType))
                {
                    return true;
                }

                throw new InvalidCallbackException($"The callback {methodInfo.Name} of {methodInfo.DeclaringType.Name} must contain only one parameter inheriting from the {typeof(EventArgs).Name} type");
            }

            eventArgsType = default;
            return false;
        }

        private bool HasValidAttribute(MethodInfo methodInfo)
        {
            var observeEventAttribute = methodInfo.GetCustomAttribute<ObserveEventAttribute>();

            return observeEventAttribute is not null;
        }

        private bool HasValidParameter(MethodInfo methodInfo, out Type eventArgsType)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 1)
            {
                eventArgsType = parameters[0].ParameterType;
                return typeof(EventArgs).IsAssignableFrom(parameters[0].ParameterType);
            }

            eventArgsType = default;
            return false;
        }

        private void AddCallback(MethodInfo methodInfo, Type eventArgsType, object observer, Dictionary<Type, ObserverBindings> bindings)
        {
            var callback = CreateDelegate(methodInfo, eventArgsType, observer);

            if (bindings.TryGetValue(eventArgsType, out var observerBindings))
            {
                observerBindings.Add(callback);
            }
            else
            {
                var observerBinding = new ObserverBindings(eventArgsType);
                observerBinding.Add(callback);
                bindings[eventArgsType] = observerBinding;
            }
        }

        private Delegate CreateDelegate(MethodInfo methodInfo, Type genericType, object target)
        {
            var delegateType = typeof(Action<>).MakeGenericType(genericType);

            return methodInfo.CreateDelegate(delegateType, target);
        }
    }
}
