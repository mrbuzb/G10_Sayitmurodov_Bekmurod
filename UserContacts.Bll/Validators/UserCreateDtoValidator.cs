using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserContacts.Bll.Dtos;
using UserContacts.Dal;

namespace UserContacts.Bll.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    private readonly MainContext _context;
    public UserCreateDtoValidator(MainContext context)
    {
        _context = context;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.")
            .MustAsync(CheckUsername).WithMessage("This Username Already Exists");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(320).WithMessage("Email must not exceed 320 characters.")
            .EmailAddress().WithMessage("Email format is invalid.")
            .MustAsync(CheckEmail).WithMessage("This email already exists");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(15).WithMessage("Phone number must not exceed 15 characters.")
            .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number format is invalid (e.g., +998901234567).")
            .MustAsync(CheckPhoneNumber).WithMessage("This phonenumber already taken");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters.")
            .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Password must contain at least one number.");
    }

    private async Task<bool> CheckUsername(string username, CancellationToken cansTok)
    {
        var res = await _context.Users.AnyAsync(_ => _.UserName == username);
        return (!res);
    }
    private async Task<bool> CheckEmail(string email, CancellationToken cansTok)
    {
        var res = await _context.Users.AnyAsync(_ => _.UserName == email);
        return (!res);
    }
    private async Task<bool> CheckPhoneNumber(string phone, CancellationToken cansTok)
    {
        var res = await _context.Users.AnyAsync(_ => _.UserName == phone);
        return (!res);
    }
}
