using DynamicData;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
  public class InMemoryTasksService : ITasksService
  {
    private readonly IEmailService _emailService;

    private readonly ISourceCache<FrenzyCheckoutTask, long> _tasks =
      new SourceCache<FrenzyCheckoutTask, long>(_ => _.Id);

    public InMemoryTasksService(IEmailService emailService)
    {
      _emailService = emailService;
      Tasks = _tasks;
    }

    public IObservableCache<FrenzyCheckoutTask, long> Tasks { get; }

    public void Add(FrenzyCheckoutTask task)
    {
      _tasks.AddOrUpdate(task);
    }

    public void Remove(FrenzyCheckoutTask task)
    {
      _tasks.Remove(task);
      _emailService.ReleaseEmail(task.AssignedEmail);
    }
  }
}