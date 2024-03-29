﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectFrenzy.Core.EventHandlers
{
  public interface IApplicationEventHandler
  {
    Type SupportedEventType { get; }
    Task HandleAsync(object eventMessage, CancellationToken ct = default);
  }
}