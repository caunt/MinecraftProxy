﻿namespace Void.Proxy.API.Events.Services;

public interface IEventService
{
    public ValueTask ThrowAsync<T>(CancellationToken cancellationToken = default) where T : IEvent, new();
    public ValueTask ThrowAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;

    public ValueTask<TResult?> ThrowWithResultAsync<TResult>(IEventWithResult<TResult> @event, CancellationToken cancellationToken = default);

    public ValueTask<TResult?> ThrowWithResultAsync<T, TResult>(CancellationToken cancellationToken = default) where T : IEventWithResult<TResult?>, new();

    public void RegisterListeners(params IEventListener[] listeners);
    public void UnregisterListeners(params IEventListener[] listeners);
}