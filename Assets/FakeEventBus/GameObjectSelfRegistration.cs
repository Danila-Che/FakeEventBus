using System;
using UnityEngine;

namespace FakeEventBus
{
    public enum RegistrationStrategy
    {
        Single,
        Object,
        Recursive
    }

    public class GameObjectSelfRegistration : MonoBehaviour
    {
        [SerializeField] private RegistrationStrategy m_RegistrationStrategy = RegistrationStrategy.Recursive;

        private RegistrationStrategy m_LastRegistrationStrategy;
        private bool m_WasRegistered = false;

        private void OnEnable()
        {
            if (m_WasRegistered) { return; }

            Register(m_RegistrationStrategy);

			m_LastRegistrationStrategy = m_RegistrationStrategy;
            m_WasRegistered = true;
        }

        private void OnDisable()
        {
			Unregister(m_RegistrationStrategy);

			m_WasRegistered = false;
        }

        protected virtual void Register(RegistrationStrategy registrationStrategy)
        {
			switch (registrationStrategy)
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
					throw new ArgumentOutOfRangeException(registrationStrategy.ToString());
			}
		}

        protected virtual void Unregister(RegistrationStrategy registrationStrategy)
        {
			switch (registrationStrategy)
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
					throw new ArgumentOutOfRangeException(registrationStrategy.ToString());
			}
		}
    }
}
