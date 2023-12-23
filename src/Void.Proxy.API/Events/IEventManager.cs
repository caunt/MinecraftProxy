﻿using System.Reflection;

namespace Void.Proxy.API.Events;

public interface IEventManager
{
    public Task ThrowAsync<T>(T @event) where T : IEvent;
    public IEventListener[] RegisterListeners(Assembly assembly);
    public void UnregisterListeners(IEventListener[] listeners);
}
