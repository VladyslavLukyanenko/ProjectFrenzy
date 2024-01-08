using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ProjectFrenzy.Core.Model;
using ProjectFrenzy.Core.Services;
using ProjectFrenzy.Core.Validators;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class EditAddressViewModel
    : ViewModelBase
  {
    public EditAddressViewModel(ICountriesService countriesService, CheckoutAddress address,
      CheckoutAddressValidator validator)
    {
      Countries = countriesService.Countries;
      var countriesCache = Countries.ToDictionary(_ => _.Id);
      if (!string.IsNullOrEmpty(address.CountryId) && countriesCache.ContainsKey(address.CountryId))
      {
        SelectedCountry = countriesCache[address.CountryId];
      }
      else
      {
        SelectedCountry = Countries[0];
      }

      Address = address;
      this.WhenAnyValue(_ => _.SelectedCountry)
        .Select(_ => _?.Id)
        .Subscribe(countryId =>
        {
          Address.CountryId = countryId;
          if (!string.IsNullOrEmpty(countryId) && countriesCache.ContainsKey(countryId))
          {
            SelectedState = SelectedCountry?.Provinces.FirstOrDefault(_ => _.Code == address.ProvinceCode);
          }
          else
          {
            SelectedState = SelectedCountry.Provinces.FirstOrDefault();
          }

          IsProvinceListVisible = SelectedCountry?.IsProvincesList ?? false;
          IsProvinceInputVisible = SelectedCountry?.IsProvincesText ?? false;
          IsProvinceLabelVisible = IsProvinceListVisible || IsProvinceInputVisible;
        });

      this.WhenAnyValue(_ => _.SelectedState)
        .Select(_ => _?.Code)
        .Subscribe(code => Address.ProvinceCode = code);

      this.WhenAnyValue(_ => _.SelectedProvinceText)
        .Subscribe(code => Address.ProvinceCode = code);

      Address.Changed.Throttle(TimeSpan.FromMilliseconds(200))
        .Select(a => validator.Validate(Address).IsValid)
        .ToPropertyEx(this, _ => _.IsValid);
    }

    public bool IsValid { [ObservableAsProperty] get; }
    [Reactive] public CheckoutAddress Address { get; private set; }

    [Reactive] public bool IsProvinceLabelVisible { get; private set; }
    [Reactive] public bool IsProvinceListVisible { get; private set; }
    [Reactive] public bool IsProvinceInputVisible { get; private set; }

    [Reactive] public string SelectedProvinceText { get; set; }
    public IReadOnlyList<Country> Countries { get; }
    [Reactive] public Country SelectedCountry { get; set; }
    [Reactive] public Province SelectedState { get; set; }
  }
}