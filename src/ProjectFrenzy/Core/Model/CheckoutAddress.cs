using System.Reactive.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ProjectFrenzy.Core.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.Model
{
  [DataContract]
  public class CheckoutAddress : ViewModelBase
  {
    public CheckoutAddress()
    {
      this.WhenAnyValue(_ => _.FirstName)
        .CombineLatest(this.WhenAnyValue(_ => _.LastName), (fname, lname) => $"{fname} {lname}")
        .DistinctUntilChanged()
        .ToPropertyEx(this, _ => _.FullName);
    }

    [IgnoreDataMember] public string FullName { [ObservableAsProperty] get; }

    [Reactive, DataMember(Name = "FirstName")] public string FirstName { get; set; }
    [Reactive, DataMember(Name = "LastName")] public string LastName { get; set; }
    [Reactive, DataMember(Name = "AddressLine1")] public string AddressLine1 { get; set; }
    [Reactive, DataMember(Name = "AddressLine2")] public string AddressLine2 { get; set; }
    [Reactive, DataMember(Name = "City")] public string City { get; set; }
    [Reactive, DataMember(Name = "ZipCode")] public string ZipCode { get; set; }
    [Reactive, DataMember(Name = "CountryId")] public string CountryId { get; set; }
    [Reactive, DataMember(Name = "PhoneNumber")] public string PhoneNumber { get; set; }
    [Reactive, DataMember(Name = "ProvinceCode")] public string ProvinceCode { get; set; }
  }
}