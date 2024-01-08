using FluentValidation;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Validators
{
  public class FrenzyCheckoutTaskValidator : AbstractValidator<FrenzyCheckoutTask>
  {
    public FrenzyCheckoutTaskValidator()
    {
      RuleFor(_ => _.Flashsale).NotNull();
      RuleFor(_ => _.Mode).IsInEnum();
      RuleFor(_ => _.Product).NotNull();
      // RuleFor(_ => _.CheckoutDelay).GreaterThan(0); // todo: make sure e really need to specify delay
      RuleFor(_ => _.SelectedProfile).NotNull();
      RuleFor(_ => _.SelectedSizes).NotEmpty().When(_ => _.Mode != CheckoutMode.Random);
    }
  }
}