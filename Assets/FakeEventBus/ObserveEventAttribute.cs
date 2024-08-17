using System;

namespace FakeEventBus
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ObserveEventAttribute : Attribute { }
}
