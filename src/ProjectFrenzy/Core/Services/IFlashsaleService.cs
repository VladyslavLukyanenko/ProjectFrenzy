using System.Collections.Generic;

using DynamicData;

using ProjectFrenzy.Core.Model.FlashSale;

namespace ProjectFrenzy.Core.Services
{
  public interface IFlashsaleService
  {
    void AddOrUpdate(IEnumerable<Flashsale> flashsales);
    IObservableCache<Flashsale, long> Flashsales { get; }
    void AddOrUpdate(Flashsale item);
  }
}