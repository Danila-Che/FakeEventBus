using UnityEngine;
using UnityEngine.Pool;

namespace FakeEventBus
{
	public static class EventBusUtilities
	{
		public static void RegisterSingle(GameObject gameObject, EventBus eventBus)
		{
			if (gameObject.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
			{
				eventBus.Register(monoBehaviour);
			}
		}

		public static void RegisterObject(GameObject gameObject, EventBus eventBus)
		{
			using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
			gameObject.GetComponents(monoBehaviours);

			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				var monoBehaviour = monoBehaviours[i];

				if (monoBehaviour != null)
				{
					eventBus.Register(monoBehaviour);
				}
			}
		}

		public static void RegisterRecursive(GameObject gameObject, EventBus eventBus)
		{
			using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
			gameObject.GetComponentsInChildren(includeInactive: true, monoBehaviours);

			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				var monoBehaviour = monoBehaviours[i];

				if (monoBehaviour != null)
				{
					eventBus.Register(monoBehaviour);
				}
			}
		}

		public static void UnregisterSingle(GameObject gameObject, EventBus eventBus)
		{
			if (gameObject.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
			{
				eventBus.Unregister(monoBehaviour);
			}
		}

		public static void UnregisterObject(GameObject gameObject, EventBus eventBus)
		{
			using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
			gameObject.GetComponents(monoBehaviours);

			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				var monoBehaviour = monoBehaviours[i];

				if (monoBehaviour != null)
				{
					eventBus.Unregister(monoBehaviour);
				}
			}
		}

		public static void UnregisterRecursive(GameObject gameObject, EventBus eventBus)
		{
			using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
			gameObject.GetComponentsInChildren(includeInactive: true, monoBehaviours);

			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				var monoBehaviour = monoBehaviours[i];

				if (monoBehaviour != null)
				{
					eventBus.Unregister(monoBehaviour);
				}
			}
		}
	}
}
