using FluentValidation;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Validators
{
  public class ProfileValidator : AbstractValidator<Profile>
  {
    public ProfileValidator(CheckoutAddressValidator addressValidator)
    {
      RuleFor(_ => _.ProfileName).NotEmpty();
      RuleFor(_ => _.BillingAddress).Must((p, a) => p.IsShippingSameAsBilling || addressValidator.Validate(a).IsValid);
      RuleFor(_ => _.ShippingAddress).SetValidator(addressValidator);
    }
  }
}