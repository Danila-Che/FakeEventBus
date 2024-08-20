using System;

namespace FakeEventBus
{
    public interface IEventBus
    {
        int GetActiveObserverCount<T>()
            where T : EventArgs;

		void Clear(bool includeCache = false);

		void Register(object observer);
        
        void Unregister(object observer);

        void Notify<T>(T eventArgs)
            where T : EventArgs;
    }
}
