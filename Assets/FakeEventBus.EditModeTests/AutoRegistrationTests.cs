using System;
using NUnit.Framework;
using UnityEngine;

namespace FakeEventBus.EditModeTests
{
    [TestFixture]
    public class AutoRegistrationTests
    {
        private class EventArgsStub : EventArgs { }

        private class Observer : MonoBehaviour
        {
            public int CallbackCount = 0;

            [ObserveEvent]
            private void On(EventArgsStub args)
            {
                CallbackCount++;
            }
        }

        [Test]
        public void Test_EventBusDecorator_SingleRegistration()
        {
            var observerGameObject = new GameObject();
            _ = observerGameObject.AddComponent<Observer>();

            EventBusDecorator.Clear();

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));

            EventBusDecorator.RegisterSingle(observerGameObject);

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(1));
        }

        [Test]
        public void Test_EventBusDecorator_ObjectRegistration()
        {
            var observerGameObject = new GameObject();

            _ = observerGameObject.AddComponent<Observer>();
            _ = observerGameObject.AddComponent<Observer>();

            EventBusDecorator.Clear();

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));

            EventBusDecorator.RegisterObject(observerGameObject);

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(2));
        }

        [Test]
        public void Test_EventBusDecorator_RecursiveRegistration()
        {
            var observerGameObject = new GameObject();
            var nestedGameObject = new GameObject();

            nestedGameObject.transform.parent = observerGameObject.transform;
            
            _ = observerGameObject.AddComponent<Observer>();
            _ = nestedGameObject.AddComponent<Observer>();

            EventBusDecorator.Clear();

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));

            EventBusDecorator.RegisterRecursive(observerGameObject);

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(2));
        }

        [Test]
        public void Test_EventBusDecorator_SingleUnregistration()
        {
            var observerGameObject = new GameObject();
            _ = observerGameObject.AddComponent<Observer>();

            EventBusDecorator.Clear();
            EventBusDecorator.RegisterRecursive(observerGameObject);

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(1));

            EventBusDecorator.UnregisterSingle(observerGameObject);

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }

        [Test]
        public void Test_EventBusDecorator_ObjectUnregistration()
        {
            var observerGameObject = new GameObject();

            _ = observerGameObject.AddComponent<Observer>();
            _ = observerGameObject.AddComponent<Observer>();

            EventBusDecorator.Clear();
            EventBusDecorator.RegisterObject(observerGameObject);

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(2));

            EventBusDecorator.UnregisterObject(observerGameObject);

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }

        [Test]
        public void Test_EventBusDecorator_RecursiveUnregistration()
        {
            var observerGameObject = new GameObject();
            var nestedGameObject = new GameObject();

            nestedGameObject.transform.parent = observerGameObject.transform;
            
            _ = observerGameObject.AddComponent<Observer>();
            _ = nestedGameObject.AddComponent<Observer>();

            EventBusDecorator.Clear();
            EventBusDecorator.RegisterRecursive(observerGameObject);

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(2));

            EventBusDecorator.UnregisterRecursive(observerGameObject);

            Assert.That(EventBusDecorator.EventBus.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }
    }
}
