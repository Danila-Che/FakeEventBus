using System;
using UnityEngine;
using UnityEngine.Pool;

namespace FakeEventBus
{
    public sealed class EventBusProxy
    {
        private static readonly Lazy<EventBus> m_Instance = new(() => new EventBus());

        public static EventBus EventBus => m_Instance.Value;

        public static void Clear()
        {
            m_Instance.Value.Clear();
        }

        public static void Notify<T>(T eventArgs)
            where T : EventArgs
        {
            m_Instance.Value.Notify(eventArgs);
        }

        public static void RegisterSingle(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
            {
                m_Instance.Value.Register(monoBehaviour);
            }
        }

        public static void RegisterObject(GameObject gameObject)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetComponents(monoBehaviours);

            for (int i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null)
                {
                    m_Instance.Value.Register(monoBehaviour);
                }
            }
        }

        public static void RegisterRecursive(GameObject gameObject)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetComponentsInChildren(includeInactive: true, monoBehaviours);

            for (int i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null)
                {
                    m_Instance.Value.Register(monoBehaviour);
                }
            }
        }

        public static void UnregisterSingle(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
            {
                m_Instance.Value.Unregister(monoBehaviour);
            }
        }

        public static void UnregisterObject(GameObject gameObject)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetComponents(monoBehaviours);

            for (int i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null)
                {
                    m_Instance.Value.Unregister(monoBehaviour);
                }
            }
        }

        public static void UnregisterRecursive(GameObject gameObject)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetComponentsInChildren(includeInactive: true, monoBehaviours);

            for (int i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];

                if (monoBehaviour != null)
                {
                    m_Instance.Value.Unregister(monoBehaviour);
                }
            }
        }
    }
}
