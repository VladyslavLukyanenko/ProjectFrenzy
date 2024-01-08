using FluentValidation;
using ProjectFrenzy.Core.Model;

namespace ProjectFrenzy.Core.Validators
{
  public class CheckoutAddressValidator : AbstractValidator<CheckoutAddress>
  {
    public CheckoutAddressValidator()
    {
      RuleFor(_ => _.City).NotEmpty();
      RuleFor(_ => _.AddressLine1).NotEmpty();
      RuleFor(_ => _.CountryId).NotEmpty();
      RuleFor(_ => _.FirstName).NotEmpty();
      RuleFor(_ => _.LastName).NotEmpty();
      RuleFor(_ => _.PhoneNumber).NotEmpty();
      RuleFor(_ => _.ZipCode).NotEmpty();
    }
  }
}