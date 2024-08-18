using System;
using FakeEventBus.Benchmark.Utilities;

namespace FakeEventBus.Benchmark
{
    public class UnregistrationBenchmark : MonoProfiler
    {
        private class EventArgsStub : EventArgs { }

        private class Observer
        {
            [ObserveEvent]
            public void On(EventArgsStub args) {}
        }

        private EventBus m_EventBus;
        private Observer[] m_Observers;

        protected override int Order => 2;

        private void Start()
        {
            m_EventBus = new EventBus();
            m_Observers = new Observer[Iterations];

            Array.Fill(m_Observers, new Observer());
        }

        protected override void OnBeginSample()
        {
            Array.ForEach(m_Observers, observer => m_EventBus.Register(observer));
        }

        protected override void Sample(int i)
        {
            m_EventBus.Unregister(m_Observers[i]);
        }
    }
}
