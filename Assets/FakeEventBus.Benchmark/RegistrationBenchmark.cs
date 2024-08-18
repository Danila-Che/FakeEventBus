using System;
using FakeEventBus.Benchmark.Utilities;

namespace FakeEventBus.Benchmark
{
    public class RegistrationBenchmark : MonoProfiler
    {
        private class EventArgsStub : EventArgs { }

        private class Observer
        {
            [ObserveEvent]
            public void On(EventArgsStub args) {}
        }

        private EventBus m_EventBus;
        private Observer[] m_Observers;

        protected override int Order => 0;

        private void Start()
        {
            m_EventBus = new EventBus();
            m_Observers = new Observer[Iterations];

            Array.Fill(m_Observers, new Observer());
        }

        protected override void OnBeginSample()
        {
            m_EventBus.Clear();
        }

        protected override void Sample(int i)
        {
            m_EventBus.Register(m_Observers[i]);
        }
    }
}
