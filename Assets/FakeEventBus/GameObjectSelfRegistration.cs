using System;
using UnityEngine;

namespace FakeEventBus
{
    public class GameObjectSelfRegistration : MonoBehaviour
    {
        private enum RegistrationStrategy
        {
            Single,
            Object,
            Recursive
        }

        [SerializeField] private RegistrationStrategy m_RegistrationStrategy = RegistrationStrategy.Recursive;

        private RegistrationStrategy m_LastRegistrationStrategy;
        private bool m_WasRegistered = false;

        private void OnEnable()
        {
            if (m_WasRegistered) { return; }

            switch (m_RegistrationStrategy)
            {
                case RegistrationStrategy.Single:
                    EventBusProxy.RegisterSingle(gameObject);
                    break;
                case RegistrationStrategy.Object:
                    EventBusProxy.RegisterObject(gameObject);
                    break;
                case RegistrationStrategy.Recursive:
                    EventBusProxy.RegisterRecursive(gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(m_RegistrationStrategy.ToString());
            }

            m_LastRegistrationStrategy = m_RegistrationStrategy;
            m_WasRegistered = true;
        }

        private void OnDisable()
        {
            switch (m_LastRegistrationStrategy)
            {
                case RegistrationStrategy.Single:
                    EventBusProxy.UnregisterSingle(gameObject);
                    break;
                case RegistrationStrategy.Object:
                    EventBusProxy.UnregisterObject(gameObject);
                    break;
                case RegistrationStrategy.Recursive:
                    EventBusProxy.UnregisterRecursive(gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(m_RegistrationStrategy.ToString());
            }

            m_WasRegistered = false;
        }
    }
}
