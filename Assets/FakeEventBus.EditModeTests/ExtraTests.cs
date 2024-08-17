using System;
using NUnit.Framework;

namespace FakeEventBus.EditModeTests
{
    [TestFixture]
    public class ExtraTests
    {
        private class EventArgsStub : EventArgs { }
        
        private class ObserverStub
        {
            public int CallbackCount = 0;

            [ObserveEvent]
            private void Callback(EventArgsStub args)
            {
                CallbackCount++;
            }
        }

        [Test]
        public void Test_TryRegisterObserverSeveralTimes()
        {
            var eventBus = new EventBus();
            var observer = new ObserverStub();

            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));

            eventBus.Register(observer);
            eventBus.Register(observer);

            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(1));
        }

        [Test]
        public void Test_TryUnregisterObserverSeveralTimes()
        {
            var eventBus = new EventBus();
            var observer = new ObserverStub();

            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));

            eventBus.Register(observer);

            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(1));

            eventBus.Unregister(observer);
            eventBus.Unregister(observer);

            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }

        [Test]
        public void Test_TryNotifyEventSeveralTimes()
        {
            var eventBus = new EventBus();
            var observer = new ObserverStub();
            eventBus.Register(observer);

            Assert.That(observer.CallbackCount, Is.EqualTo(0));

            eventBus.Notify(new EventArgsStub());
            eventBus.Notify(new EventArgsStub());

            Assert.That(observer.CallbackCount, Is.EqualTo(2));
        }
    }
}
