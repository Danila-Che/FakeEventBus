using System;

namespace FakeEventBus
{
    public interface IEventBus
    {
        void Register(object observer);
        
        void Unregister(object observer);
        void Notify<T>(T eventArgs)
            where T : EventArgs;
    }
}
