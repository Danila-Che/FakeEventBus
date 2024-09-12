using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FakeEventBus
{
	public class EventBus : IEventBus
	{
		private class ObserverBindings
		{
			private readonly List<Delegate> m_Callbacks;

			public ObserverBindings()
			{
				m_Callbacks = new List<Delegate>();
			}

			public int CallbackCount => m_Callbacks.Count;

			public void Add(Delegate callback)
			{
				m_Callbacks.Add(callback);
			}

			public void Add(ObserverBindings bindings)
			{
				m_Callbacks.AddRange(bindings.m_Callbacks);
			}

			public void Remove<T>(T observer)
				where T : class
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
			}

			public bool Contains<T>(MethodInfo methodInfo, T observer)
				where T : class
			{
				for (int i = 0; i < m_Callbacks.Count; i++)
				{
					if (m_Callbacks[i].Method == methodInfo && m_Callbacks[i].Target == observer)
					{
						return true;
					}
				}

				return false;
			}

			public void Invoke<T>(T eventArgs)
				where T : EventArgs
			{
				for (int i = 0; i < m_Callbacks.Count; i++)
				{
					((Action<T>)m_Callbacks[i]).Invoke(eventArgs);
				}
			}

			public async Task InvokeAsync<T>(T eventArgs)
				where T : EventArgs
			{
				for (int i = 0; i < m_Callbacks.Count; i++)
				{
					((Action<T>)m_Callbacks[i]).Invoke(eventArgs);

					await Task.Yield();
				}
			}
		}

		private struct CallbackCache
		{
			public Type EventArgsType;
			public MethodInfo MethodInfo;
			public Type DelegateType;
		}

		private const BindingFlags k_Scope =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Instance;

		private static readonly object s_Lock = new();

		private readonly Dictionary<Type, ObserverBindings> m_ObserverBindings; // Type is event args type
		private readonly Dictionary<Type, List<CallbackCache>> m_ObserverCallbackCache; // Type is observer type

		public EventBus()
		{
			m_ObserverBindings = new Dictionary<Type, ObserverBindings>();
			m_ObserverCallbackCache = new Dictionary<Type, List<CallbackCache>>();
		}

		public int GetActiveObserverCount<T>()
			where T : EventArgs
		{
			Monitor.Enter(s_Lock);

			try
			{
				if (m_ObserverBindings.TryGetValue(typeof(T), out var bindings))
				{
					return bindings.CallbackCount;
				}

				return 0;
			}
			finally
			{
				Monitor.Exit(s_Lock);
			}
		}

		public void Clear(bool includeCache = false)
		{
			Monitor.Enter(s_Lock);

			m_ObserverBindings.Clear();

			if (includeCache)
			{
				m_ObserverCallbackCache.Clear();
			}

			Monitor.Exit(s_Lock);
		}

		public void Register(object observer)
		{
			Monitor.Enter(s_Lock);

			CacheIfNecessary(observer);
			AddObserver(observer);

			Monitor.Exit(s_Lock);
		}

		public async Task RegisterAsync(object observer)
		{
			Monitor.Enter(s_Lock);

			await Task.Run(() => CacheIfNecessary(observer));
			await Task.Run(() => AddObserver(observer));

			Monitor.Exit(s_Lock);
		}

		public void Unregister(object observer)
		{
			Monitor.Enter(s_Lock);

			foreach (var binding in m_ObserverBindings)
			{
				binding.Value.Remove(observer);
			}

			Monitor.Exit(s_Lock);
		}

		public async Task UnregisterAsync(object observer)
		{
			Monitor.Enter(s_Lock);

			await Task.Run(() =>
			{
				foreach (var binding in m_ObserverBindings)
				{
					binding.Value.Remove(observer);
				}
			});

			Monitor.Exit(s_Lock);
		}

		public void Notify<T>(T eventArgs)
			where T : EventArgs
		{
			Monitor.Enter(s_Lock);

			if (m_ObserverBindings.TryGetValue(typeof(T), out var bindings))
			{
				bindings.Invoke(eventArgs);
			}

			Monitor.Exit(s_Lock);
		}

		public async Task NotifyAsync<T>(T eventArgs)
			where T : EventArgs
		{
			Monitor.Enter(s_Lock);

			if (m_ObserverBindings.TryGetValue(typeof(T), out var bindings))
			{
				await bindings.InvokeAsync(eventArgs);
			}

			Monitor.Exit(s_Lock);
		}

		private void CacheIfNecessary(object observer)
		{
			if (m_ObserverCallbackCache.ContainsKey(observer.GetType()) is false)
			{
				var cache = new List<CallbackCache>();

				Cache(observer.GetType(), cache);
				m_ObserverCallbackCache.Add(observer.GetType(), cache);
			}
		}

		private void Cache(Type type, List<CallbackCache> eventArgsTypes)
		{
			var methods = type.GetMethods(k_Scope);

			foreach (var methodInfo in methods)
			{
				if (TryGetEventArgsType(methodInfo, out Type eventArgsType))
				{
					eventArgsTypes.Add(new CallbackCache
					{
						EventArgsType = eventArgsType,
						MethodInfo = methodInfo,
						DelegateType = typeof(Action<>).MakeGenericType(eventArgsType),
					});
				}
			}

			if (type.BaseType != null)
			{
				Cache(type.BaseType, eventArgsTypes);
			}
		}

		private void AddObserver(object observer)
		{
			foreach (var eventCache in m_ObserverCallbackCache[observer.GetType()])
			{
				if (m_ObserverBindings.TryGetValue(eventCache.EventArgsType, out var bindings) is false)
				{
					bindings = new ObserverBindings();
					m_ObserverBindings.Add(eventCache.EventArgsType, bindings);
				}

				if (bindings.Contains(eventCache.MethodInfo, observer) is false)
				{
					var callback = CreateDelegate(eventCache, observer);
					bindings.Add(callback);
				}
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

			eventArgsType = null;
			return false;
		}

		private bool HasValidAttribute(MethodInfo methodInfo)
		{
			return methodInfo.IsDefined(typeof(ObserveEventAttribute), inherit: false);
		}

		private bool HasValidParameter(MethodInfo methodInfo, out Type eventArgsType)
		{
			var parameters = methodInfo.GetParameters();

			if (parameters.Length == 1)
			{
				eventArgsType = parameters[0].ParameterType;
				return typeof(EventArgs).IsAssignableFrom(eventArgsType);
			}

			eventArgsType = default;
			return false;
		}

		private Delegate CreateDelegate(CallbackCache cache, object target)
		{
			return cache.MethodInfo.CreateDelegate(cache.DelegateType, target);
		}
	}
}
