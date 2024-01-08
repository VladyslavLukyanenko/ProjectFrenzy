using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public interface ITasksService
  {
    IObservableCache<FrenzyCheckoutTask, long> Tasks { get; }
    void Add(FrenzyCheckoutTask task);
    void Remove(FrenzyCheckoutTask task);
  }
}