using System;
using NUnit.Framework;

namespace FakeEventBus.EditModeTests
{
    [TestFixture]
    public class SimpleClassCallbackTests
    {
        private class EventArgsStub : EventArgs { }

        private class ObserverWithoutCallback
        {
            public void NotCallback () { }
        }

        private class OnePublicCallbackStub
        {
            [ObserveEvent]
            public void Callback(EventArgsStub args) { }
        }

        private class OnePrivateCallbackStub
        {
            [ObserveEvent]
            private void Callback(EventArgsStub args) { }
        }

        private class OneInvalidCallbackStub
        {
            [ObserveEvent]
            private void Callback(int args) { }
        }

        private class OneCallbackStub
        {
            public int CallbackCount = 0;

            [ObserveEvent]
            private void Callback(EventArgsStub args)
            {
                CallbackCount++;
            }
        }
        
        /// <summary>
        /// Given an event bus without a observer
        /// When a observer without a callback tries to register on the event bus
        /// Then the event bus still has no observer
        /// </summary>
        [Test]
        public void Test_TryRegisterObserverWithoutCallback()
        {
            var eventBus = new EventBus();
            var observer = new ObserverWithoutCallback();
            eventBus.Register(observer);

            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }

        /// <summary>
        /// Given an event bus without a observer
        /// When a invalid callback observer tries to register on the event bus
        /// Then the event bus throws an exeption
        /// And the event bus still has no observer
        /// </summary>
        [Test]
        public void Test_TryRegisterObserverWithInvalidCallback()
        {
            var eventBus = new EventBus();
            var observer = new OneInvalidCallbackStub();

            Assert.Throws<InvalidCallbackException>(() => eventBus.Register(observer));
            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }

        /// <summary>
        /// Given an event bus without a observer
        /// When a public callback observer tries to register on the event bus
        /// Then the event bus has one observer
        /// </summary>
        [Test]
        public void Test_RegisterObserverWithOnePublicCallback()
        {
            var eventBus = new EventBus();
            var observer = new OnePublicCallbackStub();
            eventBus.Register(observer);

            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(1));
        }

        /// <summary>
        /// Given an event bus without a observer
        /// When a private callback observer tries to register on the event bus
        /// Then the event bus has one observer
        /// </summary>
        [Test]
        public void Test_RegisterObserverWithOnePrivateCallback()
        {
            var eventBus = new EventBus();
            var observer = new OnePrivateCallbackStub();
            eventBus.Register(observer);

            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(1));
        }

        /// <summary>
        /// Given an event bus with one observer
        /// When this observer tries to unregister from the event bus
        /// Then the event bus has no observer
        /// </summary>
        [Test]
        public void Test_UnregisterObserverWithOneCallback()
        {
            var eventBus = new EventBus();
            var observer = new OneCallbackStub();
            eventBus.Register(observer);

            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(1));

            eventBus.Unregister(observer);
            
            Assert.That(eventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }

        /// <summary>
        /// Given an event bus with one observer
        /// When the event bus raises an event
        /// Then the observer will perform a callback 
        /// </summary>
        [Test]
        public void Test_NotifyRegisteredObservers()
        {
            var eventBus = new EventBus();
            var observer = new OneCallbackStub();
            eventBus.Register(observer);
            eventBus.Notify(new EventArgsStub());

            Assert.That(observer.CallbackCount, Is.EqualTo(1));
        }
    }
}
