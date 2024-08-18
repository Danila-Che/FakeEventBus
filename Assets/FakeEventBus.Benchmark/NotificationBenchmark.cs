using System;
using FakeEventBus.Benchmark.Utilities;

namespace FakeEventBus.Benchmark
{
    public class NotificationBenchmark : MonoProfiler
    {
        private class EventArgsStub : EventArgs { }

        private class Observer
        {
            [ObserveEvent]
            public void On(EventArgsStub args) {}
        }

        private EventBus m_EventBus;
        private EventArgsStub m_EventArgs;
        private Observer m_Observer;

        protected override int Order => 1;

        private void Start()
        {
            m_EventBus = new EventBus();
            m_EventArgs = new EventArgsStub();
            m_Observer = new Observer();

            m_EventBus.Register(m_Observer);
        }

        protected override void OnBeginSample() { }

        protected override void Sample(int i)
        {
            m_EventBus.Notify(m_EventArgs);
        }
    }
}
