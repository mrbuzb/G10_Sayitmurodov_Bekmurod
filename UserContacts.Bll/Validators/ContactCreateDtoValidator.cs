using FluentValidation;
using UserContacts.Bll.Dtos;

namespace UserContacts.Bll.Validators;

public class ContactCreateDtoValidator : AbstractValidator<ContactCreateDto>
{
    public ContactCreateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Name is requaired")
            .MaximumLength(100).WithMessage("Name must be less then 100");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Name must be less then 100");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is requaired")
            .Matches(@"^\+?\d{9,15}$").WithMessage("PhoneNumber is incorrect form");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email is incorrect form")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("Location must less then 200 chars");

    }
}
