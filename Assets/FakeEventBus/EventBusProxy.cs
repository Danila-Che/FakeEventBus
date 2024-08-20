using System;
using UnityEngine;

namespace FakeEventBus
{
	public sealed class EventBusProxy
	{
		private static readonly Lazy<EventBus> m_Instance = new(() => new EventBus());

		public static IEventBus EventBus => m_Instance.Value;

		public static int GetActiveObserverCount<T>()
			where T : EventArgs
		{
			return m_Instance.Value.GetActiveObserverCount<T>();
		}

		public static void Clear()
		{
			m_Instance.Value.Clear();
		}

		public static void Register(object observer)
		{
			m_Instance.Value.Register(observer);

		}

		public static void Unregister(object observer)
		{
			m_Instance.Value.Unregister(observer);
		}

		public static void Notify<T>(T eventArgs)
			where T : EventArgs
		{
			m_Instance.Value.Notify(eventArgs);
		}

		public static void RegisterSingle(GameObject gameObject)
		{
			EventBusUtilities.RegisterSingle(gameObject, m_Instance.Value);
		}

		public static void RegisterObject(GameObject gameObject)
		{
			EventBusUtilities.RegisterObject(gameObject, m_Instance.Value);
		}

		public static void RegisterRecursive(GameObject gameObject)
		{
			EventBusUtilities.RegisterRecursive(gameObject, m_Instance.Value);
		}

		public static void UnregisterSingle(GameObject gameObject)
		{
			EventBusUtilities.UnregisterSingle(gameObject, m_Instance.Value);
		}

		public static void UnregisterObject(GameObject gameObject)
		{
			EventBusUtilities.UnregisterObject(gameObject, m_Instance.Value);
		}

		public static void UnregisterRecursive(GameObject gameObject)
		{
			EventBusUtilities.UnregisterRecursive(gameObject, m_Instance.Value);
		}
	}
}
