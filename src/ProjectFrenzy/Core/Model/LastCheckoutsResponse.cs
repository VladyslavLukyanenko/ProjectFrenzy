using System.Collections.Generic;

namespace ProjectFrenzy.Core.Model
{
    public class LastCheckoutsResponse
    {
        public IList<CheckoutDetailsData> Success { get; set; } = new List<CheckoutDetailsData>();
        public IList<CheckoutDetailsData> Failed { get; set; } = new List<CheckoutDetailsData>();
    }
}