# FakeEventBus
A minimal event bus for Unity that is easy to use and provides simple navigation through the IDE.

## Installation

### Unity Package Manager
```
https://github.com/Danila-Che/FakeEventBus.git?path=/Assets/FakeEventBus
```

1. In Unity, open **Window** → **Package Manager**.
2. Press the **+** button, choose "**Add package from git URL...**"
3. Enter url above and press **Add**.

## Usage
Full-fledged code that uses the event bus:
```csharp
using System;

public class ExampleEventArgs : EventArgs { ... }

public class AthotherExampleEventArgs : EventArgs { ... }
```

```csharp
using FakeEventBus;
using System;

public class ExampleObserver : IDisposable
{
    private EventBus m_EventBus;

    public ExampleObserver(EventBus eventBus)
    {
        m_EventBus = eventBus;

        m_EventBus.Register(this);
    }

    public void Dispose()
    {
        m_EventBus.Unregister(this);
    }

    [ObserveEvent]
    public void On(ExampleEventArgs args)
    {
        ...
    }

    [ObserveEvent]
    public void On(AthotherExampleEventArgs args)
    {
        ...
    }
}
```

```csharp
using FakeEventBus;

public class ExampleEventHandler
{
    private EventBus m_EventBus;

    public OnRaiseEvent()
    {
        m_EventBus.Notify(new ExampleEventArgs(42));
    }
}
```

## Getting Started
1. Create a class representing the arguments of the event. The event args class must inherit from `System.EventArgs`.
```csharp
using System;

public class ExampleEventArgs : EventArgs
{
    private int m_ExampleData;

    public ExampleEventArgs(int exampleData)
    {
        m_ExampleData = exampleData;
    }

    public int ExampleData => m_ExampleData;
}
```
2. Create an observable callback in your class. The callback must have only one parameter of type `EventArgs` and must have `ObserveEventAttribute`.
```csharp
using FakeEventBus;

public class ExampleObserver
{
    [ObserveEvent]
    public void On(ExampleEventArgs args)
    {
        ...
    }
}
```
3. Register an observer to event bus.
```csharp
using FakeEventBus;

public class ExampleObserver
{
    public ExampleObserver(EventBus eventBus)
    {
        eventBus.Register(this);
    }
    
    ...
}
```
4. Raise an event with event arguments.
```csharp
using FakeEventBus;

public class ExampleEventHandler
{
    private EventBus m_EventBus;

    public OnRaiseEvent()
    {
        m_EventBus.Notify(new ExampleEventArgs(42));
    }
}
```
5. Unregister the observer from the event bus.
```csharp
using FakeEventBus;
using System;

public class ExampleObserver : IDisposable
{
    private EventBus m_EventBus;

    ...

    public void Dispose()
    {
        m_EventBus.Unregister(this);
    }

    ...
}
```

## EventBusProxy
`EventBusProxy` encapsulates an event bus using the singleton pattern. It has static methods implementing various registration and unregistration strategies for `GameObject`.

### RegisterSingle(GameObject)
### RegisterObject(GameObject)
### RegisterRecursive(GameObject)

### UnregisterSingle(GameObject)
### UnregisterObject(GameObject)
### UnregisterRecursive(GameObject)

Other static methods:
### Clear()
### Notify<T>(T)

## GameObjectSelfRegistration
Interacts with `EventBusProxy` to automatically register and unregister with selected strategy (Single, Object, and Recursive). Register `GameObject` with the `OnEnable` callback and uregister `GameObject` with the `OnDisable` callback.

## Dependency injection
`EventBus` implements the `IEventBus` interface. I use the Reflex framework to inject `EventBus`.

```csharp
using Reflex.Core;
using UnityEngine;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(typeof(EventBus), typeof(IEventBus));
    }
}
```

```csharp
using FakeEventBus;
using Reflex.Core;

public class ExampleObserver
{
    [Inject] private IEventBus m_EventBus;

    private void OnEnable()
    {
        m_EventBus.Register(this);
    }

    private void OnDisable()
    {
        m_EventBus.Unregister(this);
    }
    
    ...
}
```

## EventBus Component
### Register(object)
Register an observer with the `EventBus` if the observer has valid callbacks. A callback contains only one parameter inherited from `EventArgs`. The observer will not be registered if at least one callback is invalid.
#### Exceptions
`InvalidCallbackException`
Observer has at least one callback is invalid.

### Unregister(object)
Unregister an observer from `EventBus`.

### Notify<T>(T)
Raises an event with the specified arguments.
#### Parameter
`eventArgs` T
The event args to be sended to all registered observer callback.

### Clear()
Removes all callbacks (observers) from the `EventBus`.

### GetActiveObserverCount<T>()
Gets the number of registered callback for the specified `EventArgs` type. 
#### Return
int
The number of registered callback
