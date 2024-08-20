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

            EventBusProxy.Clear();

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));

            EventBusProxy.RegisterSingle(observerGameObject);

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(1));
        }

        [Test]
        public void Test_EventBusDecorator_ObjectRegistration()
        {
            var observerGameObject = new GameObject();

            _ = observerGameObject.AddComponent<Observer>();
            _ = observerGameObject.AddComponent<Observer>();

            EventBusProxy.Clear();

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));

            EventBusProxy.RegisterObject(observerGameObject);

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(2));
        }

        [Test]
        public void Test_EventBusDecorator_RecursiveRegistration()
        {
            var observerGameObject = new GameObject();
            var nestedGameObject = new GameObject();

            nestedGameObject.transform.parent = observerGameObject.transform;
            
            _ = observerGameObject.AddComponent<Observer>();
            _ = nestedGameObject.AddComponent<Observer>();

            EventBusProxy.Clear();

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));

            EventBusProxy.RegisterRecursive(observerGameObject);

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(2));
        }

        [Test]
        public void Test_EventBusDecorator_SingleUnregistration()
        {
            var observerGameObject = new GameObject();
            _ = observerGameObject.AddComponent<Observer>();

            EventBusProxy.Clear();
            EventBusProxy.RegisterRecursive(observerGameObject);

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(1));

            EventBusProxy.UnregisterSingle(observerGameObject);

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }

        [Test]
        public void Test_EventBusDecorator_ObjectUnregistration()
        {
            var observerGameObject = new GameObject();

            _ = observerGameObject.AddComponent<Observer>();
            _ = observerGameObject.AddComponent<Observer>();

            EventBusProxy.Clear();
            EventBusProxy.RegisterObject(observerGameObject);

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(2));

            EventBusProxy.UnregisterObject(observerGameObject);

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }

        [Test]
        public void Test_EventBusDecorator_RecursiveUnregistration()
        {
            var observerGameObject = new GameObject();
            var nestedGameObject = new GameObject();

            nestedGameObject.transform.parent = observerGameObject.transform;
            
            _ = observerGameObject.AddComponent<Observer>();
            _ = nestedGameObject.AddComponent<Observer>();

            EventBusProxy.Clear();
            EventBusProxy.RegisterRecursive(observerGameObject);

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(2));

            EventBusProxy.UnregisterRecursive(observerGameObject);

            Assert.That(EventBusProxy.GetActiveObserverCount<EventArgsStub>(), Is.EqualTo(0));
        }
    }
}
