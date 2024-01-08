using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public interface IFrenzyCheckoutTaskExecutor
  {
    Task ExecuteAsync(FrenzyCheckoutTask task, CancellationToken ct = default);
    void Cancel(FrenzyCheckoutTask task);
    void CancelAllTasks();
    Task ExecuteAsync(IEnumerable<FrenzyCheckoutTask> tasks, CancellationToken ct = default);
  }
}