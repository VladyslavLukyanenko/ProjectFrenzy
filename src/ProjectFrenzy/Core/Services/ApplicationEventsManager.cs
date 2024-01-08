using System;
using System.Collections.Generic;
using System.Reflection;
using ProjectFrenzy.Core.EventHandlers;
using ReactiveUI;

namespace ProjectFrenzy.Core.Services
{
  public class ApplicationEventsManager : IApplicationEventsManager
  {
    private readonly IMessageBus _messageBus;
    private readonly IList<IApplicationEventHandler> _eventHandlers;

    public ApplicationEventsManager(IMessageBus messageBus, IList<IApplicationEventHandler> eventHandlers)
    {
      _messageBus = messageBus;
      _eventHandlers = eventHandlers;
    }

    public void Spawn()
    {
      WriteLine("Registering application event listeners");
      var method = GetType().GetMethod(nameof(SubscribeHandler), BindingFlags.Instance | BindingFlags.NonPublic);
      foreach (var h in _eventHandlers)
      {
        // ReSharper disable once PossibleNullReferenceException
        var genericMethod = method.MakeGenericMethod(h.SupportedEventType);
        genericMethod.Invoke(this, new object[] {h});
      }

      WriteLine("Listeners are registered");
    }

    private void SubscribeHandler<T>(ApplicationEventHandlerBase<T> handler)
    {
      _messageBus.Listen<T>()
        .Subscribe(m => { handler.HandleAsync(m); });
      WriteLine($"Registered {handler.GetType().Name}");
    }

    private static void WriteLine(string message)
    {
#if DEBUG
      Console.WriteLine(message);
#endif
    }
  }
}