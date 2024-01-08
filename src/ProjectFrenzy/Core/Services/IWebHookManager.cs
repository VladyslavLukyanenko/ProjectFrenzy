using System.Threading.Tasks;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Services
{
    public interface IWebHookManager
    {
        void EnqueueWebhook(CheckoutResult checkoutResult, FrenzyCheckoutTask task);
        void Spawn();
        Task<bool> TestWebhook();
    }
}