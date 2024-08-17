# FakeEventBus
A minimal event bus that is easy to use and provides simple navigation through the IDE.

## Getting Started
1. Create a class representing the arguments of the event. The event args class must inherit from System.EventArgs.
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
2. Create an observable callback in your class. The callback must have only one parameter of type event args and must have `ObserveEventAttribute`.
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

    [ObserveEvent]
    public void On(ExampleEventArgs args)
    {
        ...
    }
}
```
4. Raise an event.
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
5. Register an observer to event bus.
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
}
```
