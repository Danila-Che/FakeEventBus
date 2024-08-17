using System;
using NUnit.Framework;

namespace FakeEventBus.EditModeTests
{
    [TestFixture]
    public class InheritedCallbackTests
    {
        private class EventArgsStub : EventArgs { }

        public class BaseObserver
        {
            public int BaseCallbackCount = 0;

            [ObserveEvent]
            private void Callback(EventArgsStub args)
            {
                BaseCallbackCount++;
            }
        }

        public class DerivedObserver : BaseObserver
        {
            public int DerivedCallbackCount = 0;

            [ObserveEvent]
            private void Callback(EventArgsStub args)
            {
                DerivedCallbackCount++;
            }
        }

        [Test]
        public void Test_NotifyObserverWithInheretedCallbacks()
        {
            var eventBus = new EventBus();
            var observer = new DerivedObserver();
            eventBus.Register(observer);

            Assert.That(observer.BaseCallbackCount, Is.EqualTo(0));
            Assert.That(observer.DerivedCallbackCount, Is.EqualTo(0));

            eventBus.Notify(new EventArgsStub());

            Assert.That(observer.BaseCallbackCount, Is.EqualTo(1));
            Assert.That(observer.DerivedCallbackCount, Is.EqualTo(1));
        }

    }
}
