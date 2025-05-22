using FluentValidation;
using UserContacts.Bll.Dtos;
using UserContacts.Bll.Helpers;
using UserContacts.Bll.Services;
using UserContacts.Bll.Validators;

namespace UserContacts.Server.Configurations;

public static class DependecyInjectionsConfiguration
{
    public static void ConfigureDependecies(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
        services.AddScoped<IValidator<UserLoginDto>, UserLoginDtoValidator>();
        services.AddScoped<IValidator<ContactCreateDto>, ContactCreateDtoValidator>();
        services.AddScoped<IValidator<ContactDto>, ContactDtoValidator>();
    }
}
