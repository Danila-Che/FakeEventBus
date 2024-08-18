using System;
using NUnit.Framework;

namespace FakeEventBus.EditModeTests
{
    [TestFixture]
    public class SeveralCallbackTests
    {
        private class FirstEventArgsStub : EventArgs { }

        private class SecondsEventArgsStub : EventArgs { }

        private class ObserverWithIdenticalCallbacks
        {
            public int FirstCallbackCount = 0;
            public int SecondCallbackCount = 0;

            [ObserveEvent]
            public void FirstCallback(FirstEventArgsStub args)
            {
                FirstCallbackCount++;
            }
            
            [ObserveEvent]
            public void SecondCallback(FirstEventArgsStub args)
            {
                SecondCallbackCount++;
            }
        }

        private class ObserverWithDifferentCallbacks
        {
            public int FirstCallbackCount = 0;
            public int SecondCallbackCount = 0;

            [ObserveEvent]
            public void FirstCallback(FirstEventArgsStub args)
            {
                FirstCallbackCount++;
            }

            [ObserveEvent]
            public void SecondCallback(SecondsEventArgsStub args)
            {
                SecondCallbackCount++;
            }
        }

        private class ObserverWithValidAndOneInvalidCallback
        {
            [ObserveEvent]
            public void FirstCallback(FirstEventArgsStub validArgs) { }

            [ObserveEvent]
            public void SecondCallback(int invalidArgs) { }
        }

        [Test]
        public void Test_NotifyObserverWithIdenticalCallbacks()
        {
            var eventBus = new EventBus();
            var observer = new ObserverWithIdenticalCallbacks();
            eventBus.Register(observer);

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(0));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(0));

            eventBus.Notify(new FirstEventArgsStub());

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(1));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(1));
        }

        [Test]
        public void Test_NotifyObserverWithDifferentCallbacks()
        {
            var eventBus = new EventBus();
            var observer = new ObserverWithDifferentCallbacks();
            eventBus.Register(observer);

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(0));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(0));

            eventBus.Notify(new FirstEventArgsStub());

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(1));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(0));

            eventBus.Notify(new SecondsEventArgsStub());

            Assert.That(observer.FirstCallbackCount, Is.EqualTo(1));
            Assert.That(observer.SecondCallbackCount, Is.EqualTo(1));
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
            var observer = new ObserverWithValidAndOneInvalidCallback();

            Assert.Throws<InvalidCallbackException>(() => eventBus.Register(observer));
            Assert.That(eventBus.GetActiveObserverCount<FirstEventArgsStub>(), Is.EqualTo(0));
            Assert.That(eventBus.GetActiveObserverCount<SecondsEventArgsStub>(), Is.EqualTo(0));
        }
    }
}
