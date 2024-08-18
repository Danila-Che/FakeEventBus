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
                    EventBusDecorator.RegisterSingle(gameObject);
                    break;
                case RegistrationStrategy.Object:
                    EventBusDecorator.RegisterObject(gameObject);
                    break;
                case RegistrationStrategy.Recursive:
                    EventBusDecorator.RegisterRecursive(gameObject);
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
                    EventBusDecorator.UnregisterSingle(gameObject);
                    break;
                case RegistrationStrategy.Object:
                    EventBusDecorator.UnregisterObject(gameObject);
                    break;
                case RegistrationStrategy.Recursive:
                    EventBusDecorator.UnregisterRecursive(gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(m_RegistrationStrategy.ToString());
            }

            m_WasRegistered = false;
        }
    }
}
