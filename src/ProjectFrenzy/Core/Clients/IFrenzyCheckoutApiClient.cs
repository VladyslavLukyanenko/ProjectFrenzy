using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Model.CheckoutPayload;
using ProjectFrenzy.Core.ViewModels;

namespace ProjectFrenzy.Core.Clients
{
  public interface IFrenzyCheckoutApiClient
  {
    Task<HttpResponseMessage> CheckoutAsync(CheckoutPayloadRoot payloadRoot, bool useProxies,
      CancellationToken ct = default);

    Task<Guid> ExtractCheckoutIdAsync(HttpResponseMessage rawCheckoutResponse);
    Task<CheckoutResult> ProcessCheckoutAsync(Guid checkoutId, bool useProxies, CancellationToken ct = default);
  }
}