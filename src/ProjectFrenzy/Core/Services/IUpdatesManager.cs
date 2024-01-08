﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectFrenzy.Core.Services
{
  public interface IUpdatesManager
  {
    IObservable<Version> AvailableVersion { get; }
    IObservable<bool> NextVersionAvailable { get; }
    Task<Version> CheckForUpdatesAsync(CancellationToken ct = default);
    void Spawn();
  }
}