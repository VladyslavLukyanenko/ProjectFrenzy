using ProjectFrenzy.Core.Model.FlashSale;
using ReactiveUI.Fody.Helpers;

namespace ProjectFrenzy.Core.ViewModels
{
  public class ProductItemPickerViewModel
    : ViewModelBase
  {
    public ProductItemPickerViewModel(ProductDetail productDetail, string currency)
    {
      Picture = productDetail.DefaultPicture;
      Name = productDetail.Title;
      Item = productDetail;
      FormattedPriceRange = productDetail.PriceRange.ToString(currency);
    }

    public string Picture { get; }
    public string Name { get; }
    public string FormattedPriceRange { get; private set; }
    [Reactive] public ProductDetail Item { get; private set; }
  }
}