using FluentValidation;
using UserContacts.Bll.Dtos;

namespace UserContacts.Bll.Validators
{
    public class ContactDtoValidator : AbstractValidator<ContactDto>
    {
        public ContactDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be positie");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Name is requaired")
                .MaximumLength(100).WithMessage("Name should be less then a hundered chars");

            RuleFor(x => x.LastName)
                .MaximumLength(100).WithMessage("SourName should be less then a hundered chars");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumber is requaired")
                .Matches(@"^\+?\d{9,15}$").WithMessage("PhoneNumber is incorrect form");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email is incorrect form")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Adress must be less then 200 chars");
        }
    }
}
