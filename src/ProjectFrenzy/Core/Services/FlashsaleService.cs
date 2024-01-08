using System.Collections.Generic;

using DynamicData;

using ProjectFrenzy.Core.Model.FlashSale;

namespace ProjectFrenzy.Core.Services
{
  public class FlashsaleService : IFlashsaleService
  {
    private ISourceCache<Flashsale, long> _cache = new SourceCache<Flashsale, long>(_ => _.Id);

    public FlashsaleService()
    {
      Flashsales = _cache;
    }

    public void AddOrUpdate(IEnumerable<Flashsale> flashsales)
    {
      _cache.AddOrUpdate(flashsales);
    }

    public IObservableCache<Flashsale, long> Flashsales { get; }

    public void AddOrUpdate(Flashsale item)
    {
      var items = new List<Flashsale>(_cache.Count + 1) { item };
      items.AddRange(_cache.Items);
      _cache.Clear();
      _cache.AddOrUpdate(items);
    }
  }
}