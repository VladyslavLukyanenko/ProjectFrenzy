using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model.FlashSale;

namespace ProjectFrenzy.Core.Clients
{
    public interface IFlashsalesApiClient
    {
        Task<IList<Flashsale>> GetAllAsync(CancellationToken ct = default);
        Task<Flashsale> GetByPasswordAsync(string password, CancellationToken ct = default);
    }
}