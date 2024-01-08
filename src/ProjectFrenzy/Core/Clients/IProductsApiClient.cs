using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model.Product;

namespace ProjectFrenzy.Core.Clients
{
    public interface IProductsApiClient
    {
        Task<IList<Product>> GetProductByPasswordAsync(string pwd, CancellationToken ct = default);
    }
}